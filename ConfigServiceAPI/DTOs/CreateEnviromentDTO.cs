using System.ComponentModel.DataAnnotations;

namespace ConfigServiceAPI.DTOs
{
    public class CreateEnviromentDTO
    {
        public string name { get; set; }
        public string description { get; set; }

    }
}
