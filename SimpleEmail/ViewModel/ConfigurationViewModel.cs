using System.Collections.ObjectModel;

using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        public ObservableCollection<EmailAccountViewModel> EmailAccounts { get; set; }

        public ConfigurationViewModel()
        {
            this.EmailAccounts = new ObservableCollection<EmailAccountViewModel>();
        }
    }
}
