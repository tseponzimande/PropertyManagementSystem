namespace PropertyManagementSystem.UI.Components.Pages.Chat
{
    [Authorize]
    public partial class ChatPage
    {
        #region Dependencies

        [Inject]
        private IChatService ChatService { get; set; } = null!;

        [Inject]
        private IUserService UserService { get; set; } = null!;

        [Inject]
        private AuthStateService AuthState { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthProvider { get; set; } = null!;

        [Inject]
        private Radzen.NotificationService NotificationService { get; set; } = null!;

        [Inject]
        private IJSRuntime JS { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        #endregion

        #region Fields and Properties

        private List<ConversationPreviewDto> convos = new();

        private List<ConversationPreviewDto> filteredConvos = new();

        private List<MessageDto> messages = new();

        private List<UserDto> allUsers = new();

        private Guid currentUserId = Guid.Empty;

        private Guid activeOtherUserId = Guid.Empty;

        private string activeUserName = string.Empty;

        private string activeUserRole = string.Empty;

        private string messageText = string.Empty;

        private string search = string.Empty;

        private bool loadingConvos = true;

        private bool loadingMessages = false;

        private bool sending = false;

        private bool showSidebar = true;

        private bool isOtherUserTyping = false;

        private IBrowserFile? pendingFile = null;   

        private List<UserDto> newPickerUsers = new();

        private bool showPickerPanel = false;

        private Guid pickedUserId = Guid.Empty;

        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            var state = await AuthProvider.GetAuthenticationStateAsync();
            currentUserId = state.User.GetUserId();

            await LoadConversations();
            allUsers = (await UserService.GetAllUsersAsync()).ToList();
        }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender || !messages.Any())
                return;

            try
            {
                await ScrollToBottom();
            }
            catch
            {
                
            }
        }

        #endregion

        #region Methods


        private async Task LoadConversations()
        {
            loadingConvos = true;

            try
            {
                var result = await ChatService.GetUserConversationsAsync(currentUserId);
                convos = result?.Conversations?.ToList() ?? new();
                SearchConversations(search);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                loadingConvos = false;

                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task SelectConversation(ConversationPreviewDto convo)
        {
            activeOtherUserId = convo.UserId;
            activeUserName = convo.UserName;
            activeUserRole = allUsers.FirstOrDefault(u => u.Id == convo.UserId)?.Role ?? string.Empty;
            isOtherUserTyping = false;
            showSidebar = false;
            loadingMessages = true;
            StateHasChanged();

            try
            {
                var thread = await ChatService.GetConversationAsync(currentUserId, convo.UserId);
                messages = thread?.Messages?.ToList() ?? new();
                await ChatService.MarkConversationAsReadAsync(currentUserId, convo.UserId);

                await LoadConversations();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                loadingMessages = false;
            }

            await ScrollToBottom();
            await JS.InvokeVoidAsync("focusElement", "chat-input");
        }

        private Task CloseThread()
        {
            activeOtherUserId = Guid.Empty;
            showSidebar = true;
            return Task.CompletedTask;
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(messageText) && pendingFile is null)
                return;

            sending = true;

            try
            {
                var dto = new MessageDto
                {
                    SenderId = currentUserId,
                    ReceiverId = activeOtherUserId,
                    Content = messageText.Trim(),
                    IsRead = false
                };

                MessageDto? sent;

                if (pendingFile is not null)
                {
                    var stream = pendingFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                    var wrapper = new FormFileWrapper(stream, pendingFile.Name, pendingFile.ContentType, pendingFile.Size);
                    sent = await ChatService.SendMessageWithAttachmentAsync(dto, wrapper);
                    pendingFile = null;
                }
                else
                {
                    sent = await ChatService.SendMessageAsync(dto);
                }

                if (sent is not null)
                {
                    messageText = string.Empty;

                    if (!messages.Any(m => m.Id == sent.Id))
                        messages.Add(sent);

                    await ScrollToBottom();
                    await LoadConversations();
                }
                else NotificationService.Notify(NotificationSeverity.Error, "Error", "Message failed to send.");
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Send failed.");
                throw new Exception(ex.Message);
            }
            finally
            {
                sending = false;
            }
        }

        private async Task OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !e.ShiftKey)
                await SendMessage();
        }

        private Task OnTyping(ChangeEventArgs _)
        {
            return Task.CompletedTask;
        }

        private void OnFileSelected(InputFileChangeEventArgs e) => pendingFile = e.File;

        private void SearchConversations(string? term)
        {
            search = term ?? string.Empty;

            filteredConvos = string.IsNullOrWhiteSpace(search)
                ? convos.ToList()
                : convos.Where(c => c.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private async Task OpenNewConversationPicker()
        {
            var existingIds = convos.Select(c => c.UserId).ToHashSet();

            newPickerUsers = allUsers
                .Where(u => u.Id != currentUserId && !existingIds.Contains(u.Id))
                .ToList();
            if (!newPickerUsers.Any())
            {
                NotificationService.Notify(NotificationSeverity.Info, "Info", "No new users available to message.");
                return;
            }
            pickedUserId = Guid.Empty;
            showPickerPanel = true;
        }

        private async Task StartNewConversation()
        {
            if (pickedUserId == Guid.Empty)
                return;

            showPickerPanel = false;

            var user = allUsers.First(u => u.Id == pickedUserId);

            var fake = new ConversationPreviewDto
            {
                UserId = user.Id,
                UserName = user.Name,
                UnreadCount = 0
            };
            await SelectConversation(fake);
        }

        private async Task ScrollToBottom()
            => await JS.InvokeVoidAsync("scrollToBottom", "chat-messages-scroll");

        private static string FirstLetter(string? name)
            => string.IsNullOrEmpty(name) ? "?" : name[0].ToString().ToUpper();

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024} KB";
            else
                return $"{bytes / (1024 * 1024):F1} MB";
        }

        private static string FormatTime(DateTime dt)
        {
            var now = DateTime.Now;

            return dt.Date == now.Date ? dt.ToString("HH:mm")
                 : dt.Date == now.Date.AddDays(-1) ? "Yesterday"
                 : dt.ToString("dd MMM");
        }

        #endregion
    }
}