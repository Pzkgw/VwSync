using System;
using System.Windows;
using System.Windows.Input;

namespace VwSyncSever
{
    /// <summary>
    /// Interaction logic for ConnectionMessage.xaml
    /// </summary>
    public partial class ConnectionMessage : Window
    {
        public delegate void OkClickEventHandler(object sender, EventArgs e);
        public event OkClickEventHandler OkClick;

        public string username, password;
        public ConnectionMessage()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (OkClick != null)
            {
                //OkClick(string.IsNullOrEmpty(textBoxUser.Text) ? null : (string.Format("{0},{1}", textBoxUser.Text, textBoxPassword.Text)), e);

                username = textBoxUser.Text;
                password = textBoxPassword.Text;

                OkClick(sender, e);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //OkClick = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
