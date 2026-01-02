using System.Collections.ObjectModel;

using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel.Email
{
    public class EmailAccountViewModel : ViewModelBase
    {
        string _emailAddress;
        ObservableCollection<EmailFolderViewModel> _emailFolders;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { this.RaiseAndSetIfChanged(ref _emailAddress, value); }
        }
        public ObservableCollection<EmailFolderViewModel> EmailFolders
        {
            get { return _emailFolders; }
            set { this.RaiseAndSetIfChanged(ref _emailFolders, value); }
        }

        public EmailAccountViewModel()
        {
            this.EmailAddress = "newaccount@hotmail.com";
            this.EmailFolders = new ObservableCollection<EmailFolderViewModel>();
        }
    }
}
