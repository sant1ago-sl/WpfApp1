namespace WpfApp1.Models;

public class Usuario
{
    public int UsuarioId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
}