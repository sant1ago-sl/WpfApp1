using System.Collections.ObjectModel;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class NuevaReservaViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private readonly int _usuarioId;
    private Aula? _aulaSeleccionada;
    private DateTime _fecha = DateTime.Today;
    private TimeSpan _hora = DateTime.Now.TimeOfDay;
    private string _motivo = string.Empty;
    private string _mensaje = string.Empty;

    public ObservableCollection<Aula> Aulas { get; } = new();

    public Aula? AulaSeleccionada
    {
        get => _aulaSeleccionada;
        set => SetProperty(ref _aulaSeleccionada, value);
    }

    public DateTime Fecha
    {
        get => _fecha;
        set => SetProperty(ref _fecha, value);
    }

    public TimeSpan Hora
    {
        get => _hora;
        set => SetProperty(ref _hora, value);
    }

    public string HoraTexto
    {
        get => _hora.ToString(@"hh\:mm");
        set
        {
            if (TimeSpan.TryParse(value, out var hora))
                Hora = hora;
            SetProperty(ref _hora, _hora);
            OnPropertyChanged();
        }
    }

    public string Motivo
    {
        get => _motivo;
        set => SetProperty(ref _motivo, value);
    }

    public string Mensaje
    {
        get => _mensaje;
        set => SetProperty(ref _mensaje, value);
    }

    public ICommand ReservarCommand { get; }

    public event EventHandler? ReservaCreada;

    public NuevaReservaViewModel(DatabaseService db, int usuarioId)
    {
        _db = db;
        _usuarioId = usuarioId;
        ReservarCommand = new RelayCommand(EjecutarReservar, _ => AulaSeleccionada is not null);
        CargarAulas();
    }

    private void CargarAulas()
    {
        Aulas.Clear();
        foreach (var a in _db.GetAulasCombo())
            Aulas.Add(a);
    }

    private void EjecutarReservar(object? parametro)
    {
        Mensaje = string.Empty;

        if (AulaSeleccionada is null || string.IsNullOrWhiteSpace(Motivo))
        {
            Mensaje = "Complete todos los campos.";
            return;
        }

        if (_db.ExisteReserva(AulaSeleccionada.AulaId, Fecha, Hora))
        {
            Mensaje = "Ya existe una reserva con la misma aula, fecha y hora.";
            return;
        }

        _db.InsertarReserva(AulaSeleccionada.AulaId, _usuarioId, Fecha, Hora, Motivo);
        Mensaje = "Reserva creada exitosamente.";
        ReservaCreada?.Invoke(this, EventArgs.Empty);
    }
}