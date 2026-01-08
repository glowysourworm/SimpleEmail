using SimpleEmail.Core.Model;

using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel.Email
{
    public class EmailSummaryViewModel : ViewModelBase
    {
        string _emailAddress;       // Account ID
        string _id;
        string _folderId;
        string _from;
        string _subject;
        DateTime _date;

        public string EmailAddress
        {
            get { return _emailAddress; }
            set { this.RaiseAndSetIfChanged(ref _emailAddress, value); }
        }
        public string Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
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

        public EmailSummaryViewModel()
        {
            this.EmailAddress = string.Empty;
            this.Id = string.Empty;
            this.FolderId = string.Empty;
            this.From = string.Empty;
            this.Subject = string.Empty;
            this.Date = DateTime.MinValue;
        }
        public EmailSummaryViewModel(EmailSummary summary)
        {
            this.EmailAddress = summary.EmailAddress.ToString();
            this.Id = summary.Id.ToString();
            this.FolderId = summary.FolderId;
            this.From = summary.From;
            this.Subject = summary.Subject;
            this.Date = summary.Date;
        }
    }
}
