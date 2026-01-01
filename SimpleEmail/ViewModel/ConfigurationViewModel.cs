using System.Collections.ObjectModel;

using SimpleEmail.Core.Component.Model;
using SimpleEmail.Core.Model;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.Utilities;
using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        public ObservableCollection<EmailAccountSettingsViewModel> EmailAccountSettings { get; set; }

        public ConfigurationViewModel()
        {
            this.EmailAccountSettings = new ObservableCollection<EmailAccountSettingsViewModel>();
        }

        /// <summary>
        /// Creates configuration view model, from model, from file
        /// </summary>
        public ConfigurationViewModel(PrimaryConfiguration configuration)
        {
            var emailAccounts = configuration.EmailAccounts.Select(account =>
            {
                return BasicHelpers.Map<EmailClientConfiguration, EmailAccountSettingsViewModel>(account);

            }).Actualize();

            this.EmailAccountSettings = new ObservableCollection<EmailAccountSettingsViewModel>(emailAccounts);
        }

        /// <summary>
        /// Maps the view model back to the PrimaryConfiguration object for saving to disk
        /// </summary>
        public PrimaryConfiguration CreateConfiguration()
        {
            var configuration = new PrimaryConfiguration();

            // Email Accounts
            configuration.EmailAccounts.AddRange(this.EmailAccountSettings.Select(account =>
            {
                return BasicHelpers.Map<EmailAccountSettingsViewModel, EmailClientConfiguration>(account);
            }));

            return configuration;
        }
    }
}
