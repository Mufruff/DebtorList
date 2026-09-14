using Avalonia.Controls;
using Debtorslist1.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Debtorslist1.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    public LoginWindow(LoginViewModel vm) : this()
    {
        DataContext = vm;

        vm.OnLoginSuccess = () =>
        {
            var mainWindow = new MainWindow
            {
                DataContext = App.Services!.GetRequiredService<MainViewModel>()
            };
            mainWindow.Show();

            Close();
        };
    }
}