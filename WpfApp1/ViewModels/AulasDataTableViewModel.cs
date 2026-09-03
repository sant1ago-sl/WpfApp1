using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class AulasDataTableViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private DataTable _aulasTable = new();

    public DataTable AulasTable
    {
        get => _aulasTable;
        set => SetProperty(ref _aulasTable, value);
    }

    public ICommand CargarCommand { get; }

    public AulasDataTableViewModel(DatabaseService db)
    {
        _db = db;
        CargarCommand = new RelayCommand(_ => Cargar());
    }

    public void Cargar()
    {
        AulasTable = _db.GetAulasDataTable();
    }
}