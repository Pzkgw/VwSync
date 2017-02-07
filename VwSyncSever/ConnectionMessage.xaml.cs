using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VwSyncSever
{
    /// <summary>
    /// Interaction logic for ConnectionMessage.xaml
    /// </summary>
    public partial class ConnectionMessage : Window
    {
        readonly MainWindow ParentWindow;
        public ConnectionMessage(MainWindow windowCaller)
        {
            InitializeComponent();
            ParentWindow = windowCaller;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow != null)
            {
                if (!String.IsNullOrEmpty(textBoxUser.Text))
                {
                    ParentWindow.user = textBoxUser.Text;
                    ParentWindow.pass = textBoxPassword.Text;
                    Close();
                    ParentWindow.btnSync_Click(null, null);
                }
                else
                {
                    ParentWindow.infoLbl.Content = "New username and password should be used";
                    Close();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
