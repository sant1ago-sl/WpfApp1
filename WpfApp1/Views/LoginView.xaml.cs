using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views;

public partial class LoginView : Window
{
    public LoginView(DatabaseService db)
    {
        InitializeComponent();
        var vm = new LoginViewModel(db);
        vm.LoginSucceeded += (_, usuario) =>
        {
            var main = new MainView(db, usuario.UsuarioId, usuario.NombreCompleto);
            main.Show();
            Close();
        };
        DataContext = vm;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }
}