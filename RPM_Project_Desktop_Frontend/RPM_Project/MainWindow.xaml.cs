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
using RPM_PR_LIB;

namespace RPM_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User currentUser;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(User currentUser)
        {
            this.currentUser = currentUser;
            InitializeComponent();
            HomeScrollViewerButton.IsSelected = true;
            ClientNameText.Text = currentUser.Nickname;
            BalanceText.Text = $"{currentUser.Balance}$";
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        
        private void DragPanel_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.Owner.Show();
        }

        private void HomeScrollViewerButton_OnSelected(object sender, RoutedEventArgs e)
        {
            HomeScrollViewer.Visibility = Visibility.Visible;
            LibraryScrollViewer.Visibility = Visibility.Collapsed;
            ProfileScrollViewer.Visibility = Visibility.Collapsed;
            SettingsScrollViewer.Visibility = Visibility.Collapsed;
        }

        private void LibraryScrollViewerButton_OnSelected(object sender, RoutedEventArgs e)
        {
            HomeScrollViewer.Visibility = Visibility.Collapsed;
            LibraryScrollViewer.Visibility = Visibility.Visible;
            ProfileScrollViewer.Visibility = Visibility.Collapsed;
            SettingsScrollViewer.Visibility = Visibility.Collapsed;
        }
        
        private void ProfileScrollViewerButton_OnSelected(object sender, RoutedEventArgs e)
        {
            HomeScrollViewer.Visibility = Visibility.Collapsed;
            LibraryScrollViewer.Visibility = Visibility.Collapsed;
            ProfileScrollViewer.Visibility = Visibility.Visible;
            SettingsScrollViewer.Visibility = Visibility.Collapsed;
        }
        
        private void SettingsScrollViewerButton_OnSelected(object sender, RoutedEventArgs e)
        {
            HomeScrollViewer.Visibility = Visibility.Collapsed;
            LibraryScrollViewer.Visibility = Visibility.Collapsed;
            ProfileScrollViewer.Visibility = Visibility.Collapsed;
            SettingsScrollViewer.Visibility = Visibility.Visible;
        }
    }
}