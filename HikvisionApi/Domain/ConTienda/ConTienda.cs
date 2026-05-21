namespace Domain.ConTienda;

public class ConTienda
{
    public int Id { get; set; }
    public int? IdStoreHQ { get; set; }
    public string StoreName { get; set; }
    public string Ip { get; set; }
    public string DbName { get; set; }
    public string Username { get; set; }
    public string Pass { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public int UsuarioCreacion { get; set; }
    public int? UsuarioModificacion { get; set; }
    public int? Enabled { get; set; }
    public int? IdServer { get; set; }
    public string Port { get; set; }
}