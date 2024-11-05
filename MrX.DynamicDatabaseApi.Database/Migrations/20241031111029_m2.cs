﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrX.DynamicDatabaseApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserLastUpdate",
                table: "LoginsTable",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserLastUpdate",
                table: "LoginsTable");
        }
    }
}
