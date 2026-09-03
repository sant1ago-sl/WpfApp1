using System.Collections.ObjectModel;
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
        CargarCommand = new RelayCommand(_ => Cargar());
        BuscarCommand = new RelayCommand(_ => Buscar());
    }

    public void Cargar()
    {
        Reservas.Clear();
        foreach (var r in _db.GetReservasList())
            Reservas.Add(r);
    }

    private void Buscar()
    {
        Reservas.Clear();
        foreach (var r in _db.BuscarReservasPorFecha(FechaBusqueda))
            Reservas.Add(r);
    }
}