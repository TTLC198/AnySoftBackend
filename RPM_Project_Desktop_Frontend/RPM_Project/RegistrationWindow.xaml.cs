using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RPM_PR_LIB;

namespace RPM_Project;

public partial class RegistrationWindow : Window
{
    public RegistrationWindow()
    {
        InitializeComponent();
    }
    
    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.Hide();
        this.Owner.Show();
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

    private void RestoreButton_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void RegistrationButton_OnClick(object sender, RoutedEventArgs e)
    {
        using var httpClient = new HttpClient();
        var user = new User()
        {
            Nickname = LoginTextBox.Text,
            Password = PasswordTextBox.Text,
            Balance = 0
        };
        var requestBodyStream = new MemoryStream();
        JsonSerializer.Serialize(requestBodyStream, user);
        var response = httpClient.PostAsync("http://localhost:5000/api/users/", new StreamContent(requestBodyStream)).Result;
        
        var mainWindow = new MainWindow(user);
        mainWindow.Owner = this;
        this.Hide();
        mainWindow.Show();
    }
}