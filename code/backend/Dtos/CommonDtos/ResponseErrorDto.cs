namespace backend.Dtos.CommonDtos
{
    public class ResponseErrorDto
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}