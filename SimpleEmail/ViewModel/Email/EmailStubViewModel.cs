using MailKit;

using SimpleEmail.Core.Model;

using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel.Email
{
    public class EmailStubViewModel : ViewModelBase
    {
        string _emailAddress;       // Account ID
        UniqueId _uid;
        string _folderId;
        string _from;
        string _subject;
        DateTime _date;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { this.RaiseAndSetIfChanged(ref _emailAddress, value); }
        }
        public UniqueId Uid
        {
            get { return _uid; }
            set { this.RaiseAndSetIfChanged(ref _uid, value); }
        }
        public string FolderId
        {
            get { return _folderId; }
            set { this.RaiseAndSetIfChanged(ref _folderId, value); }
        }
        public string From
        {
            get { return _from; }
            set { this.RaiseAndSetIfChanged(ref _from, value); }
        }
        public string Subject
        {
            get { return _subject; }
            set { this.RaiseAndSetIfChanged(ref _subject, value); }
        }
        public DateTime Date
        {
            get { return _date; }
            set { this.RaiseAndSetIfChanged(ref _date, value); }
        }

        public EmailStubViewModel()
        {
            this.EmailAddress = string.Empty;
            this.Uid = new UniqueId();
            this.FolderId = string.Empty;
            this.From = string.Empty;
            this.Subject = string.Empty;
            this.Date = DateTime.MinValue;
        }
        public EmailStubViewModel(EmailStub stub)
        {
            this.EmailAddress = stub.EmailAddress.ToString();
            this.Uid = stub.Uid;
            this.FolderId = stub.FolderId;
            this.From = stub.From;
            this.Subject = stub.Subject;
            this.Date = stub.Date;
        }
    }
}
