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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VwSyncSever
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            Orchestrate();
        }

        private void Orchestrate()
        {

            // SETTINGS
            string
                firstLocation = @"\\10.10.10.47\video\gi test\demo\",
                secondLocation = @"c:\\_\";


            // FORM UPDATE

            textBox2.Text = firstLocation;
            atextBox2.Text = secondLocation;


            //ORCHESTRATE-IT
            Orchestrator o = new VwSyncSever.Orchestrator();
            o.SetInfoShowPodium(infoLbl);
            o.Start(firstLocation, secondLocation);
            o.DoEeet();
        }

        private void btnSaveEditedText_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
