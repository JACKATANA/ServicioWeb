using System.ComponentModel;

namespace ConfigServiceAPI.DTOs
{
    public class PatchVariableDTO
    {
        [DefaultValue("")]
        public string? name { get; set; } = "";
        [DefaultValue("")]
        public string? description { get; set; } = "";
        [DefaultValue("")]
        public string? value { get; set; } = "";
        [DefaultValue(false)]
        public bool isSensitive { get; set; } = false;
    }
}
