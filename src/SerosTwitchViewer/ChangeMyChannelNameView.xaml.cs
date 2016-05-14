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

namespace SerosTwitchViewer
{
    /// <summary>
    /// Interaction logic for ChangeMyChannelNameView.xaml
    /// </summary>
    public partial class ChangeMyChannelNameView : Window
    {
        public ChangeMyChannelNameView()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if(txtbox_channelname.Text.Trim() != "")
            {
                MainWindow.savedChannelName = txtbox_channelname.Text;
                this.Close();
            } else
            {
                MessageBox.Show("No name given. Could not change!");
            }
        }
    }
}
