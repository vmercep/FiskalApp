using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FiskalApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "artikli",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(nullable: false),
                    Sifra = table.Column<string>(nullable: true),
                    Cijena = table.Column<decimal>(nullable: false),
                    SifraMjere = table.Column<int>(nullable: false),
                    VrstaArtikla = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artikli", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(nullable: true),
                    Vlasnik = table.Column<string>(nullable: true),
                    Oib = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    TipJedinica = table.Column<string>(nullable: true),
                    NaplatniUredjaj = table.Column<string>(nullable: true),
                    Godina = table.Column<int>(nullable: false),
                    Certificate = table.Column<byte[]>(nullable: true),
                    CertificatePassword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 40, nullable: true),
                    LastName = table.Column<string>(maxLength: 40, nullable: true),
                    UserName = table.Column<string>(maxLength: 10, nullable: false),
                    Oib = table.Column<string>(maxLength: 11, nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "racun",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BrojRacuna = table.Column<string>(maxLength: 60, nullable: false),
                    Godina = table.Column<int>(nullable: false),
                    DatumRacuna = table.Column<DateTime>(nullable: false),
                    NacinPlacanja = table.Column<int>(nullable: false),
                    Iznos = table.Column<decimal>(nullable: false),
                    Zki = table.Column<string>(nullable: true),
                    Jir = table.Column<string>(nullable: true),
                    Operater = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_racun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_racun_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stavkeracuna",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Kolicina = table.Column<int>(nullable: false),
                    Cijena = table.Column<decimal>(nullable: false),
                    ArtiklId = table.Column<int>(nullable: false),
                    RacunId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stavkeracuna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stavkeracuna_artikli_ArtiklId",
                        column: x => x.ArtiklId,
                        principalTable: "artikli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stavkeracuna_racun_RacunId",
                        column: x => x.RacunId,
                        principalTable: "racun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "Id", "Certificate", "CertificatePassword", "Email", "Godina", "NaplatniUredjaj", "Naziv", "Oib", "TipJedinica", "Vlasnik" },
                values: new object[] { 1, null, null, "test@dot.com", 2020, "1", "Test", "12345678901", "1", "Test" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "FirstName", "LastName", "Oib", "Password", "UserName" },
                values: new object[] { 1, "Administrator", "Admin", "1234569871", "admin123", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_racun_UserId",
                table: "racun",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_stavkeracuna_ArtiklId",
                table: "stavkeracuna",
                column: "ArtiklId");

            migrationBuilder.CreateIndex(
                name: "IX_stavkeracuna_RacunId",
                table: "stavkeracuna",
                column: "RacunId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "stavkeracuna");

            migrationBuilder.DropTable(
                name: "artikli");

            migrationBuilder.DropTable(
                name: "racun");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
