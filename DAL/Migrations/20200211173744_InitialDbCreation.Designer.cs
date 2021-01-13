﻿// <auto-generated />
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200211173744_InitialDbCreation")]
    partial class InitialDbCreation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("Domain.GameState", b =>
                {
                    b.Property<int>("GameStateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BoardHeight")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BoardString")
                        .HasColumnType("TEXT");

                    b.Property<int>("BoardWidth")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("GameLost")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("GameWon")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinesAmount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MinesString")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<bool>("PlayerZeroMove")
                        .HasColumnType("INTEGER");

                    b.HasKey("GameStateId");

                    b.ToTable("GameStates");
                });
#pragma warning restore 612, 618
        }
    }
}
