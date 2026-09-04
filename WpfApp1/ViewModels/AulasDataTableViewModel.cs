using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
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
        try
        {
            AulasTable = _db.GetAulasDataTable();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudieron cargar las aulas.\n\nDetalle: {ex.Message}",
                "Error al cargar datos",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}