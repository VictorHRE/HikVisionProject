using Domain.Catalogo;
using Domain.ConTienda;
using Domain.Employee;
using Domain.EventLog;
using Domain.Usuario;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AmPmCentralHubContext(DbContextOptions<AmPmCentralHubContext> options)
	: DbContext(options)
{
	public DbSet<ConTienda> ConTiendas { get; set; }
	public DbSet<EmployeeHub> EmployeeHubs { get; set; }

	public DbSet<EventLog> EventLogs { get; set; }
	public DbSet<Usuario> Usuarios { get; set; }
	public DbSet<Catalogo> Catalogos { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Usuario>()
				.HasKey(u => u.idUsuario);

		modelBuilder.Entity<ConTienda>(b =>
		{
			b.ToTable("ConTienda");
			b.HasKey(c => c.Id);
		});


		modelBuilder.Entity<EmployeeHub>()
				.HasKey(em => em.Id);

		modelBuilder.Entity<EventLog>()
				.HasKey(ev => ev.Id);
		modelBuilder.Entity<Catalogo>()
				.HasKey(c => c.Id);
	}
}