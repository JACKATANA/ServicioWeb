using System.ComponentModel;

namespace ConfigServiceAPI.DTOs
{
    public class CreateVariablesDTO
    {
        public string name { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        [DefaultValue(false)]
        public bool is_sensitive { get; set; } = false;
    }
}
