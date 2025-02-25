using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Models;

namespace ToDoListApp.Data
{
    public class AppDbContext : DbContext
    { 
        public DbSet<TaskItem> Task { get; set; }   //Tabela Task

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=..\..\..\tasks.db"); //Nazwa pliku bazy danych
        }
    }
}
