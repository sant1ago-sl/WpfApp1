using Microsoft.Data.SqlClient;
using System.Data;
using WpfApp1.Models;

namespace WpfApp1.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    // ========== LOGIN (Conectado) ==========
    public Usuario? ValidarLogin(string username, string password)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT UsuarioId, Username, Password, NombreCompleto FROM Usuarios WHERE Username = @u AND Password = @p",
            conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Usuario
            {
                UsuarioId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                NombreCompleto = reader.GetString(3)
            };
        }
        return null;
    }

    // ========== AULAS - DataTable (Desconectado) ==========
    public DataTable GetAulasDataTable()
    {
        var dt = new DataTable();
        using var conn = GetConnection();
        conn.Open();
        using var adapter = new SqlDataAdapter("SELECT AulaId, Nombre, Capacidad FROM Aulas", conn);
        adapter.Fill(dt);
        return dt;
    }

    // ========== AULAS - Lista de objetos (Conectado) ==========
    public List<Aula> GetAulasList()
    {
        var list = new List<Aula>();
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT AulaId, Nombre, Capacidad FROM Aulas", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Aula
            {
                AulaId = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Capacidad = reader.GetInt32(2)
            });
        }
        return list;
    }

    // ========== AULAS - Busqueda por nombre (Conectado) ==========
    public List<Aula> BuscarAulas(string nombre)
    {
        var list = new List<Aula>();
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT AulaId, Nombre, Capacidad FROM Aulas WHERE Nombre LIKE @n", conn);
        cmd.Parameters.AddWithValue("@n", $"%{nombre}%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Aula
            {
                AulaId = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Capacidad = reader.GetInt32(2)
            });
        }
        return list;
    }

    // ========== RESERVAS - DataTable (Desconectado) ==========
    public DataTable GetReservasDataTable()
    {
        var dt = new DataTable();
        using var conn = GetConnection();
        conn.Open();
        using var adapter = new SqlDataAdapter(
            @"SELECT r.ReservaId, r.Fecha, r.Hora, r.Motivo,
                     a.Nombre AS Aula, u.NombreCompleto AS Usuario
              FROM Reservas r
              INNER JOIN Aulas a ON r.AulaId = a.AulaId
              INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId", conn);
        adapter.Fill(dt);
        return dt;
    }

    // ========== RESERVAS - Lista de objetos (Conectado) ==========
    public List<Reserva> GetReservasList()
    {
        var list = new List<Reserva>();
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            @"SELECT r.ReservaId, r.AulaId, r.UsuarioId, r.Fecha, r.Hora, r.Motivo,
                     a.Nombre, u.NombreCompleto
              FROM Reservas r
              INNER JOIN Aulas a ON r.AulaId = a.AulaId
              INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Reserva
            {
                ReservaId = reader.GetInt32(0),
                AulaId = reader.GetInt32(1),
                UsuarioId = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                Hora = (TimeSpan)reader.GetValue(4),
                Motivo = reader.GetString(5),
                NombreAula = reader.GetString(6),
                NombreUsuario = reader.GetString(7)
            });
        }
        return list;
    }

    // ========== RESERVAS - Busqueda por fecha (Conectado) ==========
    public List<Reserva> BuscarReservasPorFecha(DateTime fecha)
    {
        var list = new List<Reserva>();
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            @"SELECT r.ReservaId, r.AulaId, r.UsuarioId, r.Fecha, r.Hora, r.Motivo,
                     a.Nombre, u.NombreCompleto
              FROM Reservas r
              INNER JOIN Aulas a ON r.AulaId = a.AulaId
              INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId
              WHERE r.Fecha = @f", conn);
        cmd.Parameters.AddWithValue("@f", fecha);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Reserva
            {
                ReservaId = reader.GetInt32(0),
                AulaId = reader.GetInt32(1),
                UsuarioId = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                Hora = (TimeSpan)reader.GetValue(4),
                Motivo = reader.GetString(5),
                NombreAula = reader.GetString(6),
                NombreUsuario = reader.GetString(7)
            });
        }
        return list;
    }

    // ========== NUEVA RESERVA - Validar disponibilidad (Conectado) ==========
    public bool ExisteReserva(int aulaId, DateTime fecha, TimeSpan hora)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Reservas WHERE AulaId = @a AND Fecha = @f AND Hora = @h", conn);
        cmd.Parameters.AddWithValue("@a", aulaId);
        cmd.Parameters.AddWithValue("@f", fecha);
        cmd.Parameters.AddWithValue("@h", hora);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    // ========== NUEVA RESERVA - Insertar (Conectado) ==========
    public void InsertarReserva(int aulaId, int usuarioId, DateTime fecha, TimeSpan hora, string motivo)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(
            "INSERT INTO Reservas (AulaId, UsuarioId, Fecha, Hora, Motivo) VALUES (@a, @u, @f, @h, @m)", conn);
        cmd.Parameters.AddWithValue("@a", aulaId);
        cmd.Parameters.AddWithValue("@u", usuarioId);
        cmd.Parameters.AddWithValue("@f", fecha);
        cmd.Parameters.AddWithValue("@h", hora);
        cmd.Parameters.AddWithValue("@m", motivo);
        cmd.ExecuteNonQuery();
    }

    // ========== AULAS - Lista para combo (Conectado) ==========
    public List<Aula> GetAulasCombo()
    {
        var list = new List<Aula>();
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT AulaId, Nombre FROM Aulas", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Aula
            {
                AulaId = reader.GetInt32(0),
                Nombre = reader.GetString(1)
            });
        }
        return list;
    }

    // ========== USUARIOS - Lista para combo ==========
    public List<Usuario> GetUsuariosCombo()
    {
        var list = new List<Usuario>();
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT UsuarioId, NombreCompleto FROM Usuarios", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Usuario
            {
                UsuarioId = reader.GetInt32(0),
                NombreCompleto = reader.GetString(1)
            });
        }
        return list;
    }

    // ==========================================================
    // ==================== VERSIONES ASYNC ====================
    // ==========================================================

    public async Task<Usuario?> ValidarLoginAsync(string username, string password)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(
            "SELECT UsuarioId, Username, Password, NombreCompleto FROM Usuarios WHERE Username = @u AND Password = @p",
            conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Usuario
            {
                UsuarioId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                NombreCompleto = reader.GetString(3)
            };
        }
        return null;
    }

    public async Task<DataTable> GetAulasDataTableAsync()
    {
        var dt = new DataTable();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT AulaId, Nombre, Capacidad FROM Aulas", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        dt.Load(reader);
        return dt;
    }

    public async Task<List<Aula>> GetAulasListAsync()
    {
        var list = new List<Aula>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT AulaId, Nombre, Capacidad FROM Aulas", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Aula
            {
                AulaId = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Capacidad = reader.GetInt32(2)
            });
        }
        return list;
    }

    public async Task<List<Aula>> BuscarAulasAsync(string nombre)
    {
        var list = new List<Aula>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(
            "SELECT AulaId, Nombre, Capacidad FROM Aulas WHERE Nombre LIKE @n", conn);
        cmd.Parameters.AddWithValue("@n", $"%{nombre}%");
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Aula
            {
                AulaId = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Capacidad = reader.GetInt32(2)
            });
        }
        return list;
    }

    public async Task<DataTable> GetReservasDataTableAsync()
    {
        var dt = new DataTable();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(
            @"SELECT r.ReservaId, r.Fecha, r.Hora, r.Motivo,
                     a.Nombre AS Aula, u.NombreCompleto AS Usuario
              FROM Reservas r
              INNER JOIN Aulas a ON r.AulaId = a.AulaId
              INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        dt.Load(reader);
        return dt;
    }

    public async Task<List<Reserva>> GetReservasListAsync()
    {
        var list = new List<Reserva>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(
            @"SELECT r.ReservaId, r.AulaId, r.UsuarioId, r.Fecha, r.Hora, r.Motivo,
                     a.Nombre, u.NombreCompleto
              FROM Reservas r
              INNER JOIN Aulas a ON r.AulaId = a.AulaId
              INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Reserva
            {
                ReservaId = reader.GetInt32(0),
                AulaId = reader.GetInt32(1),
                UsuarioId = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                Hora = (TimeSpan)reader.GetValue(4),
                Motivo = reader.GetString(5),
                NombreAula = reader.GetString(6),
                NombreUsuario = reader.GetString(7)
            });
        }
        return list;
    }

    public async Task<List<Reserva>> BuscarReservasPorFechaAsync(DateTime fecha)
    {
        var list = new List<Reserva>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(
            @"SELECT r.ReservaId, r.AulaId, r.UsuarioId, r.Fecha, r.Hora, r.Motivo,
                     a.Nombre, u.NombreCompleto
              FROM Reservas r
              INNER JOIN Aulas a ON r.AulaId = a.AulaId
              INNER JOIN Usuarios u ON r.UsuarioId = u.UsuarioId
              WHERE r.Fecha = @f", conn);
        cmd.Parameters.AddWithValue("@f", fecha);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Reserva
            {
                ReservaId = reader.GetInt32(0),
                AulaId = reader.GetInt32(1),
                UsuarioId = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                Hora = (TimeSpan)reader.GetValue(4),
                Motivo = reader.GetString(5),
                NombreAula = reader.GetString(6),
                NombreUsuario = reader.GetString(7)
            });
        }
        return list;
    }

    public async Task<bool> ExisteReservaAsync(int aulaId, DateTime fecha, TimeSpan hora)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Reservas WHERE AulaId = @a AND Fecha = @f AND Hora = @h", conn);
        cmd.Parameters.AddWithValue("@a", aulaId);
        cmd.Parameters.AddWithValue("@f", fecha);
        cmd.Parameters.AddWithValue("@h", hora);
        return (int)(await cmd.ExecuteScalarAsync())! > 0;
    }

    public async Task InsertarReservaAsync(int aulaId, int usuarioId, DateTime fecha, TimeSpan hora, string motivo)
    {
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand(
            "INSERT INTO Reservas (AulaId, UsuarioId, Fecha, Hora, Motivo) VALUES (@a, @u, @f, @h, @m)", conn);
        cmd.Parameters.AddWithValue("@a", aulaId);
        cmd.Parameters.AddWithValue("@u", usuarioId);
        cmd.Parameters.AddWithValue("@f", fecha);
        cmd.Parameters.AddWithValue("@h", hora);
        cmd.Parameters.AddWithValue("@m", motivo);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Aula>> GetAulasComboAsync()
    {
        var list = new List<Aula>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT AulaId, Nombre FROM Aulas", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Aula
            {
                AulaId = reader.GetInt32(0),
                Nombre = reader.GetString(1)
            });
        }
        return list;
    }

    public async Task<List<Usuario>> GetUsuariosComboAsync()
    {
        var list = new List<Usuario>();
        using var conn = GetConnection();
        await conn.OpenAsync();
        using var cmd = new SqlCommand("SELECT UsuarioId, NombreCompleto FROM Usuarios", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Usuario
            {
                UsuarioId = reader.GetInt32(0),
                NombreCompleto = reader.GetString(1)
            });
        }
        return list;
    }
}
