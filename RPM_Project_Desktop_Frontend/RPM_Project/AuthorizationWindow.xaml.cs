using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RPM_PR_LIB;

namespace RPM_Project;

public partial class AuthorizationWindow : Window
{
    public AuthorizationWindow()
    {
        InitializeComponent();
    }
    
    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
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

    private void LoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        using var httpClient = new HttpClient();
        var response = httpClient.GetAsync("http://localhost:5000/api/users/").Result;
        var users = response.Content.ReadFromJsonAsync<List<User>>().Result;
        var user = users!.Find(u => u.Login == LoginTextBox.Text && u.Password == PasswordTextBox.Text)!;
        if (user != null!)
        {
            var mainWindow = new MainWindow(user);
            mainWindow.Owner = this;
            this.Hide();
            mainWindow.Show();
        }
    }

    private void RegistrationButton_OnClick(object sender, RoutedEventArgs e)
    {
        var registrationWindow = new RegistrationWindow();
        registrationWindow.Owner = this;
        this.Hide();
        registrationWindow.Show();
    }
}