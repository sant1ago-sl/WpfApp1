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
        ReservarCommand = new AsyncRelayCommand(EjecutarReservarAsync, _ => AulaSeleccionada is not null);
        // Fire-and-forget controlado: carga las aulas al inicializar el VM
        _ = CargarAulasAsync();
    }

    private async Task CargarAulasAsync()
    {
        try
        {
            var aulas = await _db.GetAulasComboAsync();
            Aulas.Clear();
            foreach (var a in aulas)
                Aulas.Add(a);
        }
        catch (Exception)
        {
            Mensaje = "No se pudieron cargar las aulas. Verifique la conexión a la base de datos.";
        }
    }

    private async Task EjecutarReservarAsync(object? parametro)
    {
        Mensaje = string.Empty;

        if (AulaSeleccionada is null || string.IsNullOrWhiteSpace(Motivo))
        {
            Mensaje = "Complete todos los campos.";
            return;
        }

        try
        {
            if (await _db.ExisteReservaAsync(AulaSeleccionada.AulaId, Fecha, Hora))
            {
                Mensaje = "Ya existe una reserva con la misma aula, fecha y hora.";
                return;
            }

            await _db.InsertarReservaAsync(AulaSeleccionada.AulaId, _usuarioId, Fecha, Hora, Motivo);
            Mensaje = "Reserva creada exitosamente.";
            ReservaCreada?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            Mensaje = "No se pudo guardar la reserva. Verifique la conexión a la base de datos.";
        }
    }
}
