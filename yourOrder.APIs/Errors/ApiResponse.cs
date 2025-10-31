namespace yourOrder.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request – الطلب غير صحيح.",
                401 => "Unauthorized – غير مصرح بالدخول.",
                403 => "Forbidden – لا تملك صلاحية الوصول.",
                404 => "Not Found – العنوان المطلوب غير موجود.",
                405 => "Method Not Allowed – نوع الطلب غير مسموح به.",
                406 => "Not Acceptable – صيغة الطلب غير مقبولة.",
                408 => "Request Timeout – انتهى وقت الطلب.",
                409 => "Conflict – يوجد تعارض في الطلب.",
                415 => "Unsupported Media Type – نوع البيانات غير مدعوم.",
                422 => "Unprocessable Entity – لا يمكن معالجة البيانات.",
                429 => "Too Many Requests – عدد كبير من الطلبات.",
                500 => "Internal Server Error – خطأ داخلي في الخادم.",
                502 => "Bad Gateway – خطأ في البوابة.",
                503 => "Service Unavailable – الخدمة غير متوفرة حالياً.",
                504 => "Gateway Timeout – انتهى وقت استجابة البوابة.",
                _ => "حدث خطأ غير متوقع."
                
            };
        }
    }
}
