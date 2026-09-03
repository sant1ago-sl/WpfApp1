using System.Data;
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
        CargarCommand = new RelayCommand(_ => Cargar());
    }

    public void Cargar()
    {
        ReservasTable = _db.GetReservasDataTable();
    }
}