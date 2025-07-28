
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;



namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<TransactionSplit> TransactionSplits { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions");

                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)")   
                    .HasMaxLength(36)               
                    .IsRequired();

                entity.Property(t => t.BeneficiaryName)
                    .HasColumnName("beneficiary-name")
                    .HasMaxLength(100);

                entity.Property(t => t.Date)
                    .HasColumnName("date");

                

                var dirConverter = new ValueConverter<Direction, string>(
                    v => v == Direction.Credit ? "c" : "d",
                    s => s == "c"
                         ? Direction.Credit
                         : Direction.Debit);

                entity.Property(t => t.Direction)
                    .HasColumnName("direction")
                    .HasColumnType("varchar(1)")
                    .HasConversion(dirConverter)
                    .HasMaxLength(1);

                entity.Property(t => t.Amount)
                    .HasColumnName("amount");
                    

                entity.Property(t => t.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(t => t.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(10);

                entity.Property(t => t.Mcc)
                    .HasColumnName("mcc")
                    .HasColumnType("int")      
                    .IsRequired(false);        


                var kindConverter = new ValueConverter<TransactionKind, string>(
                            v => v.ToString().ToLowerInvariant(),
                            s => Enum.Parse<TransactionKind>(s, true)        
                        );

                entity.Property(t => t.Kind)
                    .HasColumnName("kind")
                    .HasColumnType("varchar(50)")
                    .HasConversion(kindConverter)
                    .HasMaxLength(50);

                entity.Property(t => t.CatCode)
                   .HasMaxLength(50)
                   .IsRequired(false);

                entity.HasOne(t => t.Category)
                   .WithMany(c => c.Transactions)
                   .HasForeignKey(t => t.CatCode)
                   .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Code);
                entity.Property(t => t.Code)
                    .HasColumnName("code")
                    .HasColumnType("varchar(250)")  
                    .HasMaxLength(250)               
                    .IsRequired();

                entity.Property(t => t.ParentCode)
                    .HasColumnName("parent-code")
                    .HasColumnType("varchar(250)")   
                    .HasMaxLength(250);         
                    

                entity.Property(t => t.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(250)")   
                    .HasMaxLength(250)              
                    .IsRequired();
            });

            modelBuilder.Entity<TransactionSplit>(b =>
            {
                b.ToTable("transactionsplit");

                
                b.HasKey(s => s.Id);
                b.Property(s => s.Id)
                 .ValueGeneratedOnAdd();

                
                b.Property(s => s.TransactionId)
                 .IsRequired()
                 .HasMaxLength(50); 

                
                b.Property(s => s.CatCode)
                 .IsRequired()
                 .HasMaxLength(50);

                
                b.Property(s => s.Amount)
                 .IsRequired()
                 .HasColumnType("decimal(18,2)");

                
                b.HasOne(s => s.Transaction)
                 .WithMany(t => t.Splits)
                 .HasForeignKey(s => s.TransactionId)
                 .OnDelete(DeleteBehavior.Cascade);

                
                b.HasOne(s => s.Category)
                 .WithMany()
                 .HasForeignKey(s => s.CatCode)
                 .HasPrincipalKey(c => c.Code);
            });



            base.OnModelCreating(modelBuilder);
        }

        
    }
}
