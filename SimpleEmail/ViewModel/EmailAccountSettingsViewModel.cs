using SimpleEmail.Core.Component.Model;

using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel
{
    public class EmailAccountSettingsViewModel : ViewModelBase
    {
        public string ServerAddress { get; set; }
        public ushort ServerPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public EmailAccountSettingsViewModel()
        {
            this.ServerAddress = string.Empty;
            this.User = string.Empty;
            this.Password = string.Empty;
            this.ClientId = string.Empty;
            this.ClientSecret = string.Empty;
        }

        public EmailAccountSettingsViewModel(EmailClientConfiguration configuration)
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
