namespace SimpleEmail.Core.Component.Model
{
    public class EmailClientConfiguration
    {
        public string ServerAddress { get; set; }
        public ushort ServerPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
