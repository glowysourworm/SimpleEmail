using System.Windows;

using SimpleEmail.ViewModel.Configuration;

using SimpleWpf.IocFramework.EventAggregation;
using SimpleWpf.ViewModel;

namespace SimpleEmail.Event
{
    public enum DialogView
    {
        None,
        EditorView
    }
    public enum DialogEditorView
    {
        None,
        Configuration,
        NewEmailAccount
    }

    public class DialogEventData
    {
        private static readonly int DialogDefaultHeight = 400;
        private static readonly int DialogDefaultWidth = 600;

        /// <summary>
        /// Signals that the dialog is to be displayed to the user. This does NOT imply that the
        /// dialog window will be dismissed by the user. For this, the message box buttons must
        /// be set; and only for certain dialog views.
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// User Dismissal Mode:  Set in the event that the dialog window is used as a send / response
        ///                       modal-interactive popup.
        /// </summary>
        public bool UserDismissalMode { get; private set; }

        /// <summary>
        /// Title for the dialog window. This will be shown centered on the banner; or the banner will be
        /// hidden.
        /// </summary>
        public string DialogTitle { get; set; }

        /// <summary>
        /// Height of the dialog window
        /// </summary>
        public int DialogHeight { get; set; }

        /// <summary>
        /// Width of the dialog window
        /// </summary>
        public int DialogWidth { get; set; }

        /// <summary>
        /// View for the content of the dialog window
        /// </summary>
        public DialogView View { get; set; }

        /// <summary>
        /// View for the content of the dialog window (for editor mode)
        /// </summary>
        public DialogEditorView EditorView { get; set; }

        /// <summary>
        /// Returns true if the dialog view is set to EditorView (implies editor mode)
        /// </summary>
        public bool IsEditorMode { get { return this.View == DialogView.EditorView; } }

        /// <summary>
        /// User Dismissal Mode:  Show message box buttons to the user and wait for a dialog response. This 
        ///                       should operate like a modal window. The dialog result will be completed for
        ///                       this case; and a flag will be set to show that this mode was intended.
        /// </summary>
        public MessageBoxButton MessageBoxButtons { get; set; }

        /// <summary>
        /// Data context for the dialog event:  (see AudioStation.Event.EventViewModel)
        /// </summary>
        public ViewModelBase DataContext { get; set; }

        // Could use a better way to set this (globally)
        static DialogEventData()
        {
            DialogDefaultHeight = (int)(2 * SystemParameters.MaximizedPrimaryScreenHeight / 3);
            DialogDefaultWidth = (int)(2 * SystemParameters.MaximizedPrimaryScreenWidth / 3);
        }

        public DialogEventData(bool showDialog = false)
            : this(showDialog, false, string.Empty, DialogDefaultWidth, DialogDefaultHeight, MessageBoxButton.OK, DialogView.None, DialogEditorView.None, null)
        { }

        public DialogEventData(DialogView dialogView)
            : this(true, false, string.Empty, DialogDefaultWidth, DialogDefaultHeight, MessageBoxButton.OK, dialogView, DialogEditorView.None, null)
        { }

        /// <summary>
        /// Constructor for creating a configuration dialog
        /// </summary>
        public DialogEventData(ConfigurationViewModel viewDataContext)
            : this(true, false, string.Empty, DialogDefaultWidth, DialogDefaultHeight, MessageBoxButton.OK, DialogView.EditorView, DialogEditorView.Configuration, viewDataContext)
        { }

        private DialogEventData(bool showDialog,
                                bool userDismissal,
                                string dialogTitle,
                                int dialogWidth,
                                int dialogHeight,
                                MessageBoxButton userDismissalButtons,
                                DialogView eventView,
                                DialogEditorView eventEditorView,
                                ViewModelBase viewDataContext)
        {
            this.UserDismissalMode = userDismissal;

            if (userDismissal)
            {
                // Make sure we're supporting the dismissal mode
                //
                switch (userDismissalButtons)
                {
                    case MessageBoxButton.OK:
                        this.MessageBoxButtons = userDismissalButtons;
                        break;
                    case MessageBoxButton.OKCancel:
                        this.MessageBoxButtons = userDismissalButtons;
                        break;
                    case MessageBoxButton.YesNoCancel:
                    case MessageBoxButton.YesNo:
                    default:
                        throw new Exception("Unhandled MessageBoxButton:  DialogEvent.cs");
                }
            }

            this.Show = showDialog;
            this.View = eventView;
            this.EditorView = eventEditorView;
            this.DataContext = viewDataContext;
            this.DialogTitle = dialogTitle;
            this.DialogWidth = dialogWidth;
            this.DialogHeight = dialogHeight;
        }

        /// <summary>
        /// Creates a dialog editor with the specified view
        /// </summary>
        public static DialogEventData ShowConfigurationEditor(ViewModelBase dataContext)
        {
            return new DialogEventData(true,
                                       true,
                                       "Configuration",
                                       DialogDefaultWidth,
                                       DialogDefaultHeight,
                                       MessageBoxButton.OKCancel,
                                       DialogView.EditorView,
                                       DialogEditorView.Configuration,
                                       dataContext);
        }

        /// <summary>
        /// Creates a dialog editor with the specified view
        /// </summary>
        public static DialogEventData ShowNewEmailAccountEditor(ViewModelBase dataContext)
        {
            return new DialogEventData(true,
                                       true,
                                       "Email Account Settings",
                                       DialogDefaultWidth,
                                       DialogDefaultHeight,
                                       MessageBoxButton.OKCancel,
                                       DialogView.EditorView,
                                       DialogEditorView.NewEmailAccount,
                                       dataContext);
        }
    }

    /// <summary>
    /// Primary loading event corresponding to the MainViewModel.Loading indicator boolean. This will
    /// be utilized to hide / show loading UI and cursor.
    /// </summary>
    public class DialogEvent : IocEvent<DialogEventData>
    {
    }
}
