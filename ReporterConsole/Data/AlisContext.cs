using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ReporterConsole.Utils;

namespace ReporterConsole.Data
{
    public class AlisContext : DbContext
    {
        private readonly string _environment;

        protected AlisContext(string environment = "Production")
        {
            _environment = environment;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var connString = AppConfig.Configuration[$"ConnectionStrings:{_environment}"];
            if (connString == null)
            {
                Environment.ExitCode = 1;
                throw new Exception("Configuration File Is Missing or Corrupted! check appconfig.json");
            }
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Encrypter.DecryptString(connString, Encrypter.Key));
            }           
        }
    }
}
