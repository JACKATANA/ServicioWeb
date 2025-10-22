using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConfigServiceAPI.Models
{
    public class Enviroments
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }

        [Required]
        public DateTimeOffset createdAt { get; set; }

        [Required]
        public DateTimeOffset updatedAt { get; set; }

    }
}
