using System.Windows.Input;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private object? _currentView;
    private string _usuarioActual = string.Empty;

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public string UsuarioActual
    {
        get => _usuarioActual;
        set => SetProperty(ref _usuarioActual, value);
    }

    public AulasDataTableViewModel AulasDataTableVM { get; }
    public AulasObjetosViewModel AulasObjetosVM { get; }
    public ReservasDataTableViewModel ReservasDataTableVM { get; }
    public ReservasObjetosViewModel ReservasObjetosVM { get; }
    public NuevaReservaViewModel NuevaReservaVM { get; }

    public ICommand VerAulasDataTableCommand { get; }
    public ICommand VerAulasObjetosCommand { get; }
    public ICommand VerReservasDataTableCommand { get; }
    public ICommand VerReservasObjetosCommand { get; }
    public ICommand VerNuevaReservaCommand { get; }

    public MainViewModel(DatabaseService db, int usuarioId, string nombreUsuario)
    {
        _db = db;
        UsuarioActual = $"Bienvenido, {nombreUsuario}";

        AulasDataTableVM    = new AulasDataTableViewModel(db);
        AulasObjetosVM      = new AulasObjetosViewModel(db);
        ReservasDataTableVM = new ReservasDataTableViewModel(db);
        ReservasObjetosVM   = new ReservasObjetosViewModel(db);
        NuevaReservaVM      = new NuevaReservaViewModel(db, usuarioId);

        NuevaReservaVM.ReservaCreada += (_, _) => CurrentView = null;

        // Muestra la vista de inmediato y carga los datos en segundo plano (sin congelar UI)
        VerAulasDataTableCommand    = new AsyncRelayCommand(async _ => { CurrentView = AulasDataTableVM;    await AulasDataTableVM.CargarAsync(); });
        VerAulasObjetosCommand      = new AsyncRelayCommand(async _ => { CurrentView = AulasObjetosVM;      await AulasObjetosVM.CargarAsync(); });
        VerReservasDataTableCommand = new AsyncRelayCommand(async _ => { CurrentView = ReservasDataTableVM; await ReservasDataTableVM.CargarAsync(); });
        VerReservasObjetosCommand   = new AsyncRelayCommand(async _ => { CurrentView = ReservasObjetosVM;   await ReservasObjetosVM.CargarAsync(); });
        VerNuevaReservaCommand      = new RelayCommand(_ => CurrentView = NuevaReservaVM);
    }
}
