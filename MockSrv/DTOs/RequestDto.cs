namespace MockSrv.DTOs
{
    public class RequestDto
    {
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? Headers { get; set; }
        public string? QueryString { get; set; }
        public string? Body { get; set; }
    }
}
