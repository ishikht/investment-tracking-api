// <auto-generated />
using System;
using InvestmentTracking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InvestmentTracking.Data.Migrations
{
    [DbContext(typeof(SqlDbContext))]
    partial class SqlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InvestmentTracking.Core.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Balance")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("BrokerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BrokerId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("InvestmentTracking.Core.Entities.Broker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Brokers");
                });

            modelBuilder.Entity("InvestmentTracking.Core.Entities.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Commission")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Transactions");

                    b.HasDiscriminator<string>("TransactionType").HasValue("Transaction");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("InvestmentTracking.Core.Entities.AccountTransaction", b =>
                {
                    b.HasBaseType("InvestmentTracking.Core.Entities.Transaction");

                    b.HasDiscriminator().HasValue("Account");
                });

            modelBuilder.Entity("InvestmentTracking.Core.Entities.IncomeTransaction", b =>
                {
                    b.HasBaseType("InvestmentTracking.Core.Entities.Transaction");

                    b.HasDiscriminator().HasValue("Income");
                });

            modelBuilder.Entity("InvestmentTracking.Core.Entities.StockTransaction", b =>
                {
                    b.HasBaseType("InvestmentTracking.Core.Entities.Transaction");

                    b.Property<int>("Shares")
                        .HasColumnType("int");

                    b.Property<string>("Ticker")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Stock");
                });

            modelBuilder.Entity("InvestmentTracking.Core.Entities.Account", b =>
                {
                    b.HasOne("InvestmentTracking.Core.Entities.Broker", "Broker")
                        .WithMany()
                        .HasForeignKey("BrokerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Broker");
                });

            modelBuilder.Entity("InvestmentTracking.Core.Entities.Transaction", b =>
                {
                    b.HasOne("InvestmentTracking.Core.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });
#pragma warning restore 612, 618
        }
    }
}
