﻿// <auto-generated />
using System;
using FiskalApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FiskalApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210101133756_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Fiskal.Model.Artikli", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Cijena")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Sifra")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("SifraMjere")
                        .HasColumnType("int");

                    b.Property<int>("VrstaArtikla")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("artikli");
                });

            modelBuilder.Entity("Fiskal.Model.Racun", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("BrojRacuna")
                        .IsRequired()
                        .HasColumnType("varchar(60) CHARACTER SET utf8mb4")
                        .HasMaxLength(60);

                    b.Property<DateTime>("DatumRacuna")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Godina")
                        .HasColumnType("int");

                    b.Property<decimal>("Iznos")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("Jir")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("NacinPlacanja")
                        .HasColumnType("int");

                    b.Property<string>("Operater")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Zki")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("racun");
                });

            modelBuilder.Entity("Fiskal.Model.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("Certificate")
                        .HasColumnType("longblob");

                    b.Property<string>("CertificatePassword")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Godina")
                        .HasColumnType("int");

                    b.Property<string>("NaplatniUredjaj")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Naziv")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Oib")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("TipJedinica")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Vlasnik")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("settings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "test@dot.com",
                            Godina = 2020,
                            NaplatniUredjaj = "1",
                            Naziv = "Test",
                            Oib = "12345678901",
                            TipJedinica = "1",
                            Vlasnik = "Test"
                        });
                });

            modelBuilder.Entity("Fiskal.Model.StavkeRacuna", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ArtiklId")
                        .HasColumnType("int");

                    b.Property<decimal>("Cijena")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int>("Kolicina")
                        .HasColumnType("int");

                    b.Property<int>("RacunId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArtiklId");

                    b.HasIndex("RacunId");

                    b.ToTable("stavkeracuna");
                });

            modelBuilder.Entity("Fiskal.Model.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasColumnType("varchar(40) CHARACTER SET utf8mb4")
                        .HasMaxLength(40);

                    b.Property<string>("LastName")
                        .HasColumnType("varchar(40) CHARACTER SET utf8mb4")
                        .HasMaxLength(40);

                    b.Property<string>("Oib")
                        .IsRequired()
                        .HasColumnType("varchar(11) CHARACTER SET utf8mb4")
                        .HasMaxLength(11);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "Administrator",
                            LastName = "Admin",
                            Oib = "1234569871",
                            Password = "admin123",
                            UserName = "Admin"
                        });
                });

            modelBuilder.Entity("Fiskal.Model.Racun", b =>
                {
                    b.HasOne("Fiskal.Model.Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fiskal.Model.StavkeRacuna", b =>
                {
                    b.HasOne("Fiskal.Model.Artikli", "Artikl")
                        .WithMany()
                        .HasForeignKey("ArtiklId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fiskal.Model.Racun", "Racun")
                        .WithMany("StavkeRacuna")
                        .HasForeignKey("RacunId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
