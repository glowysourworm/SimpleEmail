using System.Collections.ObjectModel;

using SimpleEmail.Core.Component.Model;

namespace SimpleEmail.ViewModel
{
    public class MainViewModel
    {
        public ConfigurationViewModel Configuration { get; set; }

        public ObservableCollection<EmailViewModel> PrimaryMail { get; private set; }

        public MainViewModel()
        {
            this.PrimaryMail = new ObservableCollection<EmailViewModel>();

            var configuration = new EmailClientConfiguration()
            {
                // LOAD FROM JSON
            };

            this.Configuration = new ConfigurationViewModel(configuration);
        }
    }
}
