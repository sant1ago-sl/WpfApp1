using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class ReservasObjetosViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private DateTime _fechaBusqueda = DateTime.Today;

    public ObservableCollection<Reserva> Reservas { get; } = new();

    public DateTime FechaBusqueda
    {
        get => _fechaBusqueda;
        set => SetProperty(ref _fechaBusqueda, value);
    }

    public ICommand CargarCommand { get; }
    public ICommand BuscarCommand { get; }

    public ReservasObjetosViewModel(DatabaseService db)
    {
        _db = db;
        CargarCommand = new AsyncRelayCommand(_ => CargarAsync());
        BuscarCommand = new AsyncRelayCommand(_ => BuscarAsync());
    }

    public async Task CargarAsync()
    {
        try
        {
            var lista = await _db.GetReservasListAsync();
            Reservas.Clear();
            foreach (var r in lista)
                Reservas.Add(r);
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

    private async Task BuscarAsync()
    {
        try
        {
            var lista = await _db.BuscarReservasPorFechaAsync(FechaBusqueda);
            Reservas.Clear();
            foreach (var r in lista)
                Reservas.Add(r);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo realizar la búsqueda.\n\nDetalle: {ex.Message}",
                "Error al buscar",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
