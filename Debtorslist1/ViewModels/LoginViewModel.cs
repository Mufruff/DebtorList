using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Debtorslist1.Services;

namespace Debtorslist1.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _errorMessage = "";

        /// <summary>
        /// Сработает, если пароль верный.
        /// Устанавливается извне (из LoginWindow).
        /// </summary>
        public Action? OnLoginSuccess { get; set; }

        [RelayCommand]
        private void Login()
        {
            if (_authService.CheckPassword(Password))
            {
                ErrorMessage = "";
                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = "Неверный пароль";
            }
        }
    }
}
