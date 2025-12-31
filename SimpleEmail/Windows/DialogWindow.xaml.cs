using System.Windows;

namespace SimpleEmail.Windows
{
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // DialogResult:  There could be settings for accepting only partial data.. Mostly, there'd be a cancel
            //                if the data is invalid.
            this.DialogResult = true;
        }
    }
}
