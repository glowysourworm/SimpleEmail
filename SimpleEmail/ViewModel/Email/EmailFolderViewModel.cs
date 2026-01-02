using System.Collections.ObjectModel;

using MailKit;

using SimpleEmail.Core.Model;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.ViewModel;

namespace SimpleEmail.ViewModel.Email
{
    public class EmailFolderViewModel : ViewModelBase
    {
        string _id;
        string _parentId;
        string _name;
        int _messageCount;
        int _messageUnreadCount;
        MessageFlags _acceptedFlags;

        public ObservableCollection<EmailFolderViewModel> SubFolders { get; private set; }

        public string Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string ParentId
        {
            get { return _parentId; }
            set { this.RaiseAndSetIfChanged(ref _parentId, value); }
        }
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public int MessageCount
        {
            get { return _messageCount; }
            set { this.RaiseAndSetIfChanged(ref _messageCount, value); }
        }
        public int MessageUnreadCount
        {
            get { return _messageUnreadCount; }
            set { this.RaiseAndSetIfChanged(ref _messageUnreadCount, value); }
        }
        public MessageFlags AcceptedFlags
        {
            get { return _acceptedFlags; }
            set { this.RaiseAndSetIfChanged(ref _acceptedFlags, value); }
        }

        public EmailFolderViewModel()
        {
            this.Id = string.Empty;
            this.ParentId = string.Empty;
            this.Name = string.Empty;
            this.MessageCount = 0;
            this.MessageUnreadCount = 0;
            this.AcceptedFlags = MessageFlags.None;

            this.SubFolders = new ObservableCollection<EmailFolderViewModel>();
        }
        public EmailFolderViewModel(EmailFolder folder)
        {
            this.Id = folder.Id;
            this.ParentId = folder.ParentId ?? string.Empty;
            this.Name = folder.Name;
            this.MessageCount = folder.MessageCount;
            this.MessageCount = folder.MessageUnreadCount;
            this.AcceptedFlags = folder.AcceptedFlags;

            // Recursively add sub-folders
            this.SubFolders = new ObservableCollection<EmailFolderViewModel>(folder.SubFolders.Select(x => new EmailFolderViewModel(x)).Actualize());
        }
    }
}
