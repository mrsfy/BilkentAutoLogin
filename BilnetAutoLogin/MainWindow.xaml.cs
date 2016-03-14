using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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

namespace BilnetAutoLogin
{

    public partial class MainWindow : MetroWindow
    {
        private Logic _logic;

        public MainWindow()
        {
            _logic = new Logic();
            InitializeComponent();
            BindSettings();
        }

        private void BindSettings()
        {
             usernameText.Text = _logic.Username;
             passwordText.Password = _logic.Password;
             startupCheckBox.IsChecked = _logic.IsStartup;
        }

        private void Username_Changed(object sender, TextChangedEventArgs e)
        {
            _logic.Username = (sender as TextBox).Text;
            successfullChanges();
        }

        private void Password_Changed(object sender, RoutedEventArgs e)
        {
            _logic.Password = (sender as PasswordBox).Password;
            successfullChanges();

        }
        private void successfullChanges()
        {
            resultLabel.Foreground = new SolidColorBrush(Colors.Green);
            resultLabel.Content = "Changes applied successfully.";
        }

        private void manual_button_Click(object sender, RoutedEventArgs e)
        {
            _logic.OnNetworkStatusChanged(sender, e);
        }
        private void startupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _logic.IsStartup = true;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to close Bilnet Auto Login Completely?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }

        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Normal:
                    break;
                case WindowState.Minimized:
                    this.Hide();
                    break;
                case WindowState.Maximized:
                    break;
                default:
                    break;
            }
        }

        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        private void startupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _logic.IsStartup = false;
        }
    }

}
