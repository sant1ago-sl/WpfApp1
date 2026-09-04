using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels;

public class AulasObjetosViewModel : ViewModelBase
{
    private readonly DatabaseService _db;
    private string _busqueda = string.Empty;

    public ObservableCollection<Aula> Aulas { get; } = new();

    public string Busqueda
    {
        get => _busqueda;
        set => SetProperty(ref _busqueda, value);
    }

    public ICommand CargarCommand { get; }
    public ICommand BuscarCommand { get; }

    public AulasObjetosViewModel(DatabaseService db)
    {
        _db = db;
        CargarCommand = new RelayCommand(_ => Cargar());
        BuscarCommand = new RelayCommand(_ => Buscar());
    }

    public void Cargar()
    {
        try
        {
            Aulas.Clear();
            foreach (var a in _db.GetAulasList())
                Aulas.Add(a);
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

    private void Buscar()
    {
        try
        {
            Aulas.Clear();
            foreach (var a in _db.BuscarAulas(Busqueda))
                Aulas.Add(a);
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