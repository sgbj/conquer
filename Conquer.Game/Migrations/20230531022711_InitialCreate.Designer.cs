﻿// <auto-generated />
using Conquer.Game.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Conquer.Game.Migrations
{
    [DbContext(typeof(GameDbContext))]
    [Migration("20230531022711_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.4.23259.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Conquer.Game.Models.Player", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1000001L);

                    b.Property<int>("AttributePoints")
                        .HasColumnType("int");

                    b.Property<int>("Avatar")
                        .HasColumnType("int");

                    b.Property<int>("Dexterity")
                        .HasColumnType("int");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<decimal>("Experience")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Hair")
                        .HasColumnType("int");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<byte>("Level")
                        .HasColumnType("tinyint");

                    b.Property<int>("Magic")
                        .HasColumnType("int");

                    b.Property<int>("Mana")
                        .HasColumnType("int");

                    b.Property<long>("MapId")
                        .HasColumnType("bigint");

                    b.Property<long>("Model")
                        .HasColumnType("bigint");

                    b.Property<long>("Money")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PkMode")
                        .HasColumnType("int");

                    b.Property<short>("PkPoints")
                        .HasColumnType("smallint");

                    b.Property<int>("Profession")
                        .HasColumnType("int");

                    b.Property<byte>("Rebirths")
                        .HasColumnType("tinyint");

                    b.Property<long?>("SpouseId")
                        .HasColumnType("bigint");

                    b.Property<int>("Strength")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Vitality")
                        .HasColumnType("int");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SpouseId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Conquer.Game.Models.Player", b =>
                {
                    b.HasOne("Conquer.Game.Models.Player", "Spouse")
                        .WithMany()
                        .HasForeignKey("SpouseId");

                    b.Navigation("Spouse");
                });
#pragma warning restore 612, 618
        }
    }
}
