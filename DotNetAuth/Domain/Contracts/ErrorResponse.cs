using System.Text.Json.Serialization;

namespace DotNetAuth.Domain.Contracts
{
    public class ErrorResponse
    {
        //[JsonPropertyName("title")]
        //public string Title { get; set; }

        //[JsonPropertyName("statusCode")]
        //public int StatusCode { get; set; }

        //[JsonPropertyName("message")]
        //public string Message { get; set; }

        public  string Title { get; set; }
        public  int StatusCode { get; set; }
        public  string Message { get; set; }
    }
}
