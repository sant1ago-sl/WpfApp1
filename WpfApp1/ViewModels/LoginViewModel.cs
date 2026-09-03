using System.ComponentModel;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand LoginCommand { get; }

    public event EventHandler<Usuario>? LoginSucceeded;

    public LoginViewModel(DatabaseService db)
    {
        _db = db;
        LoginCommand = new RelayCommand(EjecutarLogin);
    }

    private void EjecutarLogin(object? parametro)
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Ingrese usuario y contraseña.";
            return;
        }

        var usuario = _db.ValidarLogin(Username, Password);
        if (usuario is null)
        {
            ErrorMessage = "Usuario o contraseña incorrectos.";
            return;
        }

        LoginSucceeded?.Invoke(this, usuario);
    }
}