namespace PropertyManagementSystem.UI.Service;

public sealed class LoadingService
{
    public bool IsLoading { get; private set; }
    public string? Message { get; private set; }

    public event Action? OnChange;

    public void Show(string? message = null)
    {
        IsLoading = true;
        Message = message;
        OnChange?.Invoke();
    }

    public void Hide()
    {
        IsLoading = false;
        Message = null;
        OnChange?.Invoke();
    }
}
