namespace WebAPI.Models
{
    public class ValidationErrorResponse
    {
        public required string Title { get; set; }
        public int Status { get; set; }           
        public required IDictionary<string, string[]> Errors { get; set; }
    }
}
