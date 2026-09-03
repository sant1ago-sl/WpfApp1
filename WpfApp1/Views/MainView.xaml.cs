using System.Windows;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views;

public partial class MainView : Window
{
    public MainView(DatabaseService db, int usuarioId, string nombreUsuario)
    {
        InitializeComponent();
        DataContext = new MainViewModel(db, usuarioId, nombreUsuario);
    }
}