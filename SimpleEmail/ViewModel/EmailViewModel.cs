using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel
{
    public class EmailViewModel : ViewModelBase
    {
        string _from;
        string _subject;
        string _body;
        DateTime _timestamp;

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
        public string Body
        {
            get { return _body; }
            set { this.RaiseAndSetIfChanged(ref _body, value); }
        }
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { this.RaiseAndSetIfChanged(ref _timestamp, value); }
        }
    }
}
