using System.ComponentModel;

namespace ConfigServiceAPI.DTOs
{
    public class PatchEnviromentDTO
    {
        [DefaultValue("")]
        public string? name { get; set; } = "";
        [DefaultValue("")]
        public string? description { get; set; } = "";

    }
}
