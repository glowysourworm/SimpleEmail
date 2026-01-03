using MailKit;

using SimpleEmail.Core.Model;

using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel.Email
{
    public class EmailStubViewModel : ViewModelBase
    {
        UniqueId _uid;
        string _from;
        string _subject;
        DateTime _date;

        public UniqueId Uid
        {
            get { return _uid; }
            set { this.RaiseAndSetIfChanged(ref _uid, value); }
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
            this.Uid = new UniqueId();
            this.From = string.Empty;
            this.Subject = string.Empty;
            this.Date = DateTime.MinValue;
        }
        public EmailStubViewModel(EmailStub stub)
        {
            this.Uid = stub.Uid;
            this.From = stub.From;
            this.Subject = stub.Subject;
            this.Date = stub.Date;
        }
    }
}
