using System;
using Domain;
using GameEngine;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {

        public DbSet<GameState> GameStates { get; set; } = default!;
        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=/Users/siimmelles/CSharp2019/icd0008-2019f/Minesweeper"
                                     + "/ConsoleApp/minesweeper.db");
        }
    }
}