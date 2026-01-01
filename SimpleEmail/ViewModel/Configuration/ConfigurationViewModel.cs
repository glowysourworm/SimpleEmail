using System.Collections.ObjectModel;

using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.Utilities;
using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel.Configuration
{
    public class ConfigurationViewModel : ViewModelBase
    {
        public ObservableCollection<EmailAccountConfigurationViewModel> EmailAccountConfigurations { get; set; }

        public ConfigurationViewModel()
        {
            this.EmailAccountConfigurations = new ObservableCollection<EmailAccountConfigurationViewModel>();
        }

        /// <summary>
        /// Creates configuration view model, from model, from file
        /// </summary>
        public ConfigurationViewModel(PrimaryConfiguration configuration)
        {
            var emailAccounts = configuration.EmailAccounts.Select(account =>
            {
                return BasicHelpers.Map<EmailAccountConfiguration, EmailAccountConfigurationViewModel>(account);

            }).Actualize();

            this.EmailAccountConfigurations = new ObservableCollection<EmailAccountConfigurationViewModel>(emailAccounts);
        }

        /// <summary>
        /// Maps the view model back to the PrimaryConfiguration object for saving to disk
        /// </summary>
        public PrimaryConfiguration CreateConfiguration()
        {
            var configuration = new PrimaryConfiguration();

            // Email Accounts
            configuration.EmailAccounts.AddRange(this.EmailAccountConfigurations.Select(account =>
            {
                return BasicHelpers.Map<EmailAccountConfigurationViewModel, EmailAccountConfiguration>(account);
            }));

            return configuration;
        }
    }
}
