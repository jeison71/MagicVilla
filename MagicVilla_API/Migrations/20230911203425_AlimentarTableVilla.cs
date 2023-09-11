using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTableVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "", new DateTime(2023, 9, 11, 15, 34, 25, 508, DateTimeKind.Local).AddTicks(1266), new DateTime(2023, 9, 11, 15, 34, 25, 508, DateTimeKind.Local).AddTicks(1280), "", 50, "Villa Real", 5, 200.0 },
                    { 2, "", "", new DateTime(2023, 9, 11, 15, 34, 25, 508, DateTimeKind.Local).AddTicks(1284), new DateTime(2023, 9, 11, 15, 34, 25, 508, DateTimeKind.Local).AddTicks(1285), "", 60, "Villa Kiara", 6, 500.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
