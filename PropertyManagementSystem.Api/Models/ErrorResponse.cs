namespace PropertyManagementSystem.Api.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public string Details { get; set; } = null!;

        public ErrorResponse(int statusCode, string? message, string? details = null)
        {
            StatusCode = statusCode;
            
            Message = message;

            Details = details;
        }


    }
}
