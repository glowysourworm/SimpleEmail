using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel.Configuration
{
    public class EmailAccountConfigurationViewModel : ViewModelBase
    {
        public string ServerAddress { get; set; }
        public ushort ServerPort { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public EmailAccountConfigurationViewModel()
        {
            this.ServerAddress = string.Empty;
            this.EmailAddress = string.Empty;
            this.Password = string.Empty;
            this.ClientId = string.Empty;
            this.ClientSecret = string.Empty;
        }

        public EmailAccountConfigurationViewModel(EmailAccountConfiguration configuration)
        {
            this.ServerAddress = configuration.ServerAddress;
            this.ServerPort = configuration.ServerPort;
            this.EmailAddress = configuration.EmailAddress;
            this.Password = configuration.Password;
            this.ClientId = configuration.ClientId;
            this.ClientSecret = configuration.ClientSecret;
        }
    }
}
