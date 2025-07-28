namespace WebAPI.Models
{
    public class DefaultErrorResponse
    {
        public required string Title { get; set; }
        public int Status { get; set; }             
        public required string Detail { get; set; }
    }
}
