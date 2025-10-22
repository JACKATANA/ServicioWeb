namespace ConfigServiceAPI.DTOs
{
    public class VariableDTO
    {
        public string name { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public DateTimeOffset createdAt { get; set; }
        public DateTimeOffset updatedAt { get; set; }
        public bool isSensitive { get; set; } = false;
    }
}
