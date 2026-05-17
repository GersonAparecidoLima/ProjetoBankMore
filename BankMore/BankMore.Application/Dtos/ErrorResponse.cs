namespace BankMore.Application.Dtos
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Type { get; set; }

        public ErrorResponse(string message, string type)
        {
            Message = message;
            Type = type;
        }
    }
}