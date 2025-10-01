namespace yourOrder.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }











        private string GetDefaultMessageForStatusCode(int statusCode)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return statusCode switch
            {
                400 => "A bad request, you have made",
                401 => "Authorized, you are not",
                404 => "Resource found, it was not",
                500 => "Errors are the path to the dark side. Errors lead to anger. Anger leads to hate. Hate leads to career change.",
                _ => null // Return null for unhandled status codes
            };
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
