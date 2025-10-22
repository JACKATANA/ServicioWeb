using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfigServiceAPI.Models
{
    public class Variables
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string value { get; set; }
        [Required]
        public string description { get; set; }

        [Required]
        public DateTimeOffset createdAt { get; set; }

        [Required]
        public DateTimeOffset updatedAt { get; set; }

        [Required]
        public bool isSensitive { get; set; }
        [Required]
        public Guid EnviromentId { get; set; }

        [ForeignKey("EnviromentId")]
        public virtual Enviroments Enviroment { get; set; }

    }
}
