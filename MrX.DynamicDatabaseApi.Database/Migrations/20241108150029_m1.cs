using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MrX.DynamicDatabaseApi.Database.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoginsTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expire = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    UserLastUpdate = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdate = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginsTable", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FieldsTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Auto = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Show = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Disable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Null = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsUnique = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IfOneRole = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TableId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdate = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsTable", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FieldsTableRolesTable",
                columns: table => new
                {
                    AddRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FieldsAddRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsTableRolesTable", x => new { x.AddRolesId, x.FieldsAddRoleId });
                    table.ForeignKey(
                        name: "FK_FieldsTableRolesTable_FieldsTable_FieldsAddRoleId",
                        column: x => x.FieldsAddRoleId,
                        principalTable: "FieldsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FieldsTableRolesTable1",
                columns: table => new
                {
                    DeleteRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FieldsDeleteRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsTableRolesTable1", x => new { x.DeleteRolesId, x.FieldsDeleteRoleId });
                    table.ForeignKey(
                        name: "FK_FieldsTableRolesTable1_FieldsTable_FieldsDeleteRoleId",
                        column: x => x.FieldsDeleteRoleId,
                        principalTable: "FieldsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FieldsTableRolesTable2",
                columns: table => new
                {
                    FieldsReadRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReadRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsTableRolesTable2", x => new { x.FieldsReadRoleId, x.ReadRolesId });
                    table.ForeignKey(
                        name: "FK_FieldsTableRolesTable2_FieldsTable_FieldsReadRoleId",
                        column: x => x.FieldsReadRoleId,
                        principalTable: "FieldsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FieldsTableRolesTable3",
                columns: table => new
                {
                    FieldsUpdateRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdateRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsTableRolesTable3", x => new { x.FieldsUpdateRoleId, x.UpdateRolesId });
                    table.ForeignKey(
                        name: "FK_FieldsTableRolesTable3_FieldsTable_FieldsUpdateRoleId",
                        column: x => x.FieldsUpdateRoleId,
                        principalTable: "FieldsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolesTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PathsRole = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdate = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTable", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolesTableTablesTable",
                columns: table => new
                {
                    AddRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TabelsAddRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTableTablesTable", x => new { x.AddRolesId, x.TabelsAddRoleId });
                    table.ForeignKey(
                        name: "FK_RolesTableTablesTable_RolesTable_AddRolesId",
                        column: x => x.AddRolesId,
                        principalTable: "RolesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolesTableTablesTable1",
                columns: table => new
                {
                    DeleteRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TabelsDeleteRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTableTablesTable1", x => new { x.DeleteRolesId, x.TabelsDeleteRoleId });
                    table.ForeignKey(
                        name: "FK_RolesTableTablesTable1_RolesTable_DeleteRolesId",
                        column: x => x.DeleteRolesId,
                        principalTable: "RolesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolesTableTablesTable2",
                columns: table => new
                {
                    ReadRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TabelsReadRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTableTablesTable2", x => new { x.ReadRolesId, x.TabelsReadRoleId });
                    table.ForeignKey(
                        name: "FK_RolesTableTablesTable2_RolesTable_ReadRolesId",
                        column: x => x.ReadRolesId,
                        principalTable: "RolesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolesTableTablesTable3",
                columns: table => new
                {
                    TabelsUpdateRoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpdateRolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTableTablesTable3", x => new { x.TabelsUpdateRoleId, x.UpdateRolesId });
                    table.ForeignKey(
                        name: "FK_RolesTableTablesTable3_RolesTable_UpdateRolesId",
                        column: x => x.UpdateRolesId,
                        principalTable: "RolesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolesTableUsersTable",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UsersId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTableUsersTable", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RolesTableUsersTable_RolesTable_RolesId",
                        column: x => x.RolesId,
                        principalTable: "RolesTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TablesTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdate = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TablesTable", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UsersTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailAddress = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Data = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TablesTableId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdate = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersTable_TablesTable_TablesTableId",
                        column: x => x.TablesTableId,
                        principalTable: "TablesTable",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsersTable_UsersTable_ParentId",
                        column: x => x.ParentId,
                        principalTable: "UsersTable",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsTable_TableId",
                table: "FieldsTable",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsTableRolesTable_FieldsAddRoleId",
                table: "FieldsTableRolesTable",
                column: "FieldsAddRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsTableRolesTable1_FieldsDeleteRoleId",
                table: "FieldsTableRolesTable1",
                column: "FieldsDeleteRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsTableRolesTable2_ReadRolesId",
                table: "FieldsTableRolesTable2",
                column: "ReadRolesId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsTableRolesTable3_UpdateRolesId",
                table: "FieldsTableRolesTable3",
                column: "UpdateRolesId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesTable_CreatorId",
                table: "RolesTable",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesTable_Name",
                table: "RolesTable",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolesTableTablesTable_TabelsAddRoleId",
                table: "RolesTableTablesTable",
                column: "TabelsAddRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesTableTablesTable1_TabelsDeleteRoleId",
                table: "RolesTableTablesTable1",
                column: "TabelsDeleteRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesTableTablesTable2_TabelsReadRoleId",
                table: "RolesTableTablesTable2",
                column: "TabelsReadRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesTableTablesTable3_UpdateRolesId",
                table: "RolesTableTablesTable3",
                column: "UpdateRolesId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesTableUsersTable_UsersId",
                table: "RolesTableUsersTable",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_TablesTable_CreatorId",
                table: "TablesTable",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TablesTable_Name",
                table: "TablesTable",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersTable_ParentId",
                table: "UsersTable",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersTable_TablesTableId",
                table: "UsersTable",
                column: "TablesTableId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersTable_UserName",
                table: "UsersTable",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldsTable_TablesTable_TableId",
                table: "FieldsTable",
                column: "TableId",
                principalTable: "TablesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldsTableRolesTable_RolesTable_AddRolesId",
                table: "FieldsTableRolesTable",
                column: "AddRolesId",
                principalTable: "RolesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldsTableRolesTable1_RolesTable_DeleteRolesId",
                table: "FieldsTableRolesTable1",
                column: "DeleteRolesId",
                principalTable: "RolesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldsTableRolesTable2_RolesTable_ReadRolesId",
                table: "FieldsTableRolesTable2",
                column: "ReadRolesId",
                principalTable: "RolesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldsTableRolesTable3_RolesTable_UpdateRolesId",
                table: "FieldsTableRolesTable3",
                column: "UpdateRolesId",
                principalTable: "RolesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolesTable_UsersTable_CreatorId",
                table: "RolesTable",
                column: "CreatorId",
                principalTable: "UsersTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolesTableTablesTable_TablesTable_TabelsAddRoleId",
                table: "RolesTableTablesTable",
                column: "TabelsAddRoleId",
                principalTable: "TablesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolesTableTablesTable1_TablesTable_TabelsDeleteRoleId",
                table: "RolesTableTablesTable1",
                column: "TabelsDeleteRoleId",
                principalTable: "TablesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolesTableTablesTable2_TablesTable_TabelsReadRoleId",
                table: "RolesTableTablesTable2",
                column: "TabelsReadRoleId",
                principalTable: "TablesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolesTableTablesTable3_TablesTable_TabelsUpdateRoleId",
                table: "RolesTableTablesTable3",
                column: "TabelsUpdateRoleId",
                principalTable: "TablesTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolesTableUsersTable_UsersTable_UsersId",
                table: "RolesTableUsersTable",
                column: "UsersId",
                principalTable: "UsersTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TablesTable_UsersTable_CreatorId",
                table: "TablesTable",
                column: "CreatorId",
                principalTable: "UsersTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTable_TablesTable_TablesTableId",
                table: "UsersTable");

            migrationBuilder.DropTable(
                name: "FieldsTableRolesTable");

            migrationBuilder.DropTable(
                name: "FieldsTableRolesTable1");

            migrationBuilder.DropTable(
                name: "FieldsTableRolesTable2");

            migrationBuilder.DropTable(
                name: "FieldsTableRolesTable3");

            migrationBuilder.DropTable(
                name: "LoginsTable");

            migrationBuilder.DropTable(
                name: "RolesTableTablesTable");

            migrationBuilder.DropTable(
                name: "RolesTableTablesTable1");

            migrationBuilder.DropTable(
                name: "RolesTableTablesTable2");

            migrationBuilder.DropTable(
                name: "RolesTableTablesTable3");

            migrationBuilder.DropTable(
                name: "RolesTableUsersTable");

            migrationBuilder.DropTable(
                name: "FieldsTable");

            migrationBuilder.DropTable(
                name: "RolesTable");

            migrationBuilder.DropTable(
                name: "TablesTable");

            migrationBuilder.DropTable(
                name: "UsersTable");
        }
    }
}
