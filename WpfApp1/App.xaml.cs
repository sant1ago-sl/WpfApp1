using System.Windows;
using Microsoft.Data.SqlClient;
using WpfApp1.Services;
using WpfApp1.Views;

namespace WpfApp1;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=ReservasDB;Trusted_Connection=True;TrustServerCertificate=True";

            var db = new DatabaseService(connectionString);

            var login = new LoginView(db);
            login.Show();
        }
        catch (SqlException ex)
        {
            MessageBox.Show(
                $"No se pudo conectar a la base de datos.\n\nVerifique que el servicio SQL Server LocalDB esté activo y que haya ejecutado el script ScriptDB.sql.\n\nDetalle: {ex.Message}",
                "Error de conexión",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error inesperado al iniciar la aplicación.\n\nDetalle: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }
}