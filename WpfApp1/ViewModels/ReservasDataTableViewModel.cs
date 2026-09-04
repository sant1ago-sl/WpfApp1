using System.Data;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class ReservasDataTableViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private DataTable _reservasTable = new();

    public DataTable ReservasTable
    {
        get => _reservasTable;
        set => SetProperty(ref _reservasTable, value);
    }

    public ICommand CargarCommand { get; }

    public ReservasDataTableViewModel(DatabaseService db)
    {
        _db = db;
        CargarCommand = new AsyncRelayCommand(_ => CargarAsync());
    }

    public async Task CargarAsync()
    {
        try
        {
            ReservasTable = await _db.GetReservasDataTableAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudieron cargar las reservas.\n\nDetalle: {ex.Message}",
                "Error al cargar datos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
