﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MrX.DynamicDatabaseApi.Database;

#nullable disable

namespace MrX.DynamicDatabaseApi.Database.Migrations
{
    [DbContext(typeof(SQLDBContext))]
    partial class SQLDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("FieldsTableRolesTable", b =>
                {
                    b.Property<Guid>("AddRolesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FieldsAddRoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("AddRolesId", "FieldsAddRoleId");

                    b.HasIndex("FieldsAddRoleId");

                    b.ToTable("FieldsTableRolesTable");
                });

            modelBuilder.Entity("FieldsTableRolesTable1", b =>
                {
                    b.Property<Guid>("DeleteRolesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FieldsDeleteRoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("DeleteRolesId", "FieldsDeleteRoleId");

                    b.HasIndex("FieldsDeleteRoleId");

                    b.ToTable("FieldsTableRolesTable1");
                });

            modelBuilder.Entity("FieldsTableRolesTable2", b =>
                {
                    b.Property<Guid>("FieldsReadRoleId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ReadRolesId")
                        .HasColumnType("char(36)");

                    b.HasKey("FieldsReadRoleId", "ReadRolesId");

                    b.HasIndex("ReadRolesId");

                    b.ToTable("FieldsTableRolesTable2");
                });

            modelBuilder.Entity("FieldsTableRolesTable3", b =>
                {
                    b.Property<Guid>("FieldsUpdateRoleId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UpdateRolesId")
                        .HasColumnType("char(36)");

                    b.HasKey("FieldsUpdateRoleId", "UpdateRolesId");

                    b.HasIndex("UpdateRolesId");

                    b.ToTable("FieldsTableRolesTable3");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.InMemory.LoginsTables", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("Expire")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("LastUpdate")
                        .HasColumnType("char(36)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("UserLastUpdate")
                        .HasColumnType("char(36)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("LoginsTable");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.FieldsTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Auto")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Disable")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IfOneRole")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsUnique")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("LastUpdate")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Null")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Show")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("TableId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("TableId");

                    b.ToTable("FieldsTable");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("LastUpdate")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PathsRole")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("RolesTable");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("LastUpdate")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("TablesTable");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.UsersTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("LastUpdate")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("TablesTableId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("TablesTableId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("UsersTable");
                });

            modelBuilder.Entity("RolesTableTablesTable", b =>
                {
                    b.Property<Guid>("AddRolesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TabelsAddRoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("AddRolesId", "TabelsAddRoleId");

                    b.HasIndex("TabelsAddRoleId");

                    b.ToTable("RolesTableTablesTable");
                });

            modelBuilder.Entity("RolesTableTablesTable1", b =>
                {
                    b.Property<Guid>("DeleteRolesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TabelsDeleteRoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("DeleteRolesId", "TabelsDeleteRoleId");

                    b.HasIndex("TabelsDeleteRoleId");

                    b.ToTable("RolesTableTablesTable1");
                });

            modelBuilder.Entity("RolesTableTablesTable2", b =>
                {
                    b.Property<Guid>("ReadRolesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TabelsReadRoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("ReadRolesId", "TabelsReadRoleId");

                    b.HasIndex("TabelsReadRoleId");

                    b.ToTable("RolesTableTablesTable2");
                });

            modelBuilder.Entity("RolesTableTablesTable3", b =>
                {
                    b.Property<Guid>("TabelsUpdateRoleId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UpdateRolesId")
                        .HasColumnType("char(36)");

                    b.HasKey("TabelsUpdateRoleId", "UpdateRolesId");

                    b.HasIndex("UpdateRolesId");

                    b.ToTable("RolesTableTablesTable3");
                });

            modelBuilder.Entity("RolesTableUsersTable", b =>
                {
                    b.Property<Guid>("RolesId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("char(36)");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("RolesTableUsersTable");
                });

            modelBuilder.Entity("FieldsTableRolesTable", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("AddRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.FieldsTable", null)
                        .WithMany()
                        .HasForeignKey("FieldsAddRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FieldsTableRolesTable1", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("DeleteRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.FieldsTable", null)
                        .WithMany()
                        .HasForeignKey("FieldsDeleteRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FieldsTableRolesTable2", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.FieldsTable", null)
                        .WithMany()
                        .HasForeignKey("FieldsReadRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("ReadRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FieldsTableRolesTable3", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.FieldsTable", null)
                        .WithMany()
                        .HasForeignKey("FieldsUpdateRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("UpdateRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.FieldsTable", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", "Table")
                        .WithMany("Filds")
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Table");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.UsersTable", "Creator")
                        .WithMany("CreateRoles")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.UsersTable", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.UsersTable", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.UsersTable", "Parent")
                        .WithMany("Childs")
                        .HasForeignKey("ParentId");

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", null)
                        .WithMany("Editors")
                        .HasForeignKey("TablesTableId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("RolesTableTablesTable", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("AddRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", null)
                        .WithMany()
                        .HasForeignKey("TabelsAddRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RolesTableTablesTable1", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("DeleteRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", null)
                        .WithMany()
                        .HasForeignKey("TabelsDeleteRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RolesTableTablesTable2", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("ReadRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", null)
                        .WithMany()
                        .HasForeignKey("TabelsReadRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RolesTableTablesTable3", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", null)
                        .WithMany()
                        .HasForeignKey("TabelsUpdateRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("UpdateRolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RolesTableUsersTable", b =>
                {
                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.RolesTable", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrX.DynamicDatabaseApi.Database.Table.SQL.UsersTable", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.TablesTable", b =>
                {
                    b.Navigation("Editors");

                    b.Navigation("Filds");
                });

            modelBuilder.Entity("MrX.DynamicDatabaseApi.Database.Table.SQL.UsersTable", b =>
                {
                    b.Navigation("Childs");

                    b.Navigation("CreateRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
