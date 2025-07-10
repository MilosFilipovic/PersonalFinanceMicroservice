
using Domain.Entities;
using Microsoft.EntityFrameworkCore;




namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions"); // nazivi tabela u bazi su lowercase po MySQL konvenciji

                entity.HasKey(t => t.Id);

                entity.Property(t => t.BeneficiaryName)
                    .HasColumnName("beneficiary-name")
                    .HasMaxLength(100);

                entity.Property(t => t.Date)
                    .HasColumnName("date");

                entity.Property(t => t.Direction)
                    .HasColumnName("direction")
                    .HasMaxLength(50);

                entity.Property(t => t.Amount)
                    .HasColumnName("amount");
                    

                entity.Property(t => t.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(t => t.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(10);

                entity.Property(t => t.Mcc)
                    .HasColumnName("mcc");

                entity.Property(t => t.Kind)
                    .HasColumnName("kind")
                    .HasMaxLength(50);
            });
        }
    }
}
