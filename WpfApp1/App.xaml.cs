using System.Windows;
using WpfApp1.Services;
using WpfApp1.Views;

namespace WpfApp1;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=ReservasDB;Trusted_Connection=True;TrustServerCertificate=True";

        var db = new DatabaseService(connectionString);

        var login = new LoginView(db);
        login.Show();
    }
}