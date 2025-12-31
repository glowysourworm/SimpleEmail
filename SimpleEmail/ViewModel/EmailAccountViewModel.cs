using SimpleEmail.Core.Component.Model;

namespace SimpleEmail.ViewModel
{
    public class EmailAccountViewModel
    {
        public string ServerAddress { get; set; }
        public ushort ServerPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public EmailAccountViewModel()
        { }

        public EmailAccountViewModel(EmailClientConfiguration configuration)
        {
            this.ServerAddress = configuration.ServerAddress;
            this.ServerPort = configuration.ServerPort;
            this.User = configuration.User;
            this.Password = configuration.Password;
            this.ClientId = configuration.ClientId;
            this.ClientSecret = configuration.ClientSecret;
        }
    }
}
