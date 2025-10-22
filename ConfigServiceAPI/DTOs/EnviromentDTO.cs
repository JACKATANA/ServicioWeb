using System.ComponentModel.DataAnnotations;

namespace ConfigServiceAPI.DTOs
{
    public class EnviromentDTO
    {
        public string name { get; set; }
        public string description { get; set; }
        public DateTimeOffset createdAt { get; set; }
        public DateTimeOffset updatedAt { get; set; }
    }
}
