using SimpleEmail.Event;

namespace SimpleEmail.Controller.Interface
{
    public interface IDialogController : IDisposable
    {
        string ShowSelectFile();
        string ShowSelectFolder();
        string ShowSaveFile();
        bool ShowConfirmation(string caption, params string[] messageLines);
        void ShowAlert(string caption, params string[] messageLines);
        void ShowConfigurationAlert();

        /// <summary>
        /// Shows dialog window synchronously. This represents a parallel usage to the event aggregator! So,
        /// use this when a dialog window is needed to be waited on; and the results returned immediately.
        /// </summary>
        bool ShowDialogWindowSync(DialogEventData eventData);
    }
}
