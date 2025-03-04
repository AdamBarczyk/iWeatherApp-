﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IWeatherApp.Views;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IWeatherApp
{
    class LoginViewModel : MainViewModelBase
    {
        private LoginService _loginService = null;

        #region Properties
        private string _email;
        private string _password;
        private string _errorMessage;

        public string Email
        {
            get { return _email; }
            set { _email = value.ToLower(); OnPropertyChanged(); }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        #endregion

        #region Navigation
        public ICommand CancelButtonClickedCommand { get; }
        public ICommand GoBackButtonClickedCommand { get; }
        public ICommand LoginButtonClickedCommand { get; }
        public ICommand ResetPasswordButtonClickedCommand { get; }

        private void CancelRegistration()
        {
            Frame navigationFrame = Window.Current.Content as Frame;
            navigationFrame.Navigate(typeof(RegistrationPage));
        }

        private void GoBackToStartPage()
        {
            Frame navigationFrame = Window.Current.Content as Frame;
            navigationFrame.Navigate(typeof(StartPage));
        }

        private void GoToResetPasswordPage()
        {
            Frame navigationFrame = Window.Current.Content as Frame;
            navigationFrame.Navigate(typeof(ResetPasswordPage));
        }
        #endregion

        public LoginViewModel()
        {
            CancelButtonClickedCommand = new DelegateCommand(CancelRegistration);
            GoBackButtonClickedCommand = new DelegateCommand(GoBackToStartPage);
            LoginButtonClickedCommand = new DelegateCommand(SignInUser);
            ResetPasswordButtonClickedCommand = new DelegateCommand(GoToResetPasswordPage);
        }

        private async void SignInUser()
        {
            _loginService = new LoginService(Email, Password);

            // start login process
            await _loginService.SignInUserAsync();

            if (_loginService.IsSignedIn)
            {
                Frame navigationFrame = Window.Current.Content as Frame;
                navigationFrame.Navigate(typeof(WeatherForecastPage));
            }
            else
            {
                ErrorMessage = _loginService.ShowError();
            }
        }
    }
}
