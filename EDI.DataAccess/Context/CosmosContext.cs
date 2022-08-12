﻿
using EDI.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace EDI.DataAccess.Context
{
    public class CosmosContext : DbContext
    {

        public CosmosContext(DbContextOptions<CosmosContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                "https://mytutorialapp.documents.azure.com:443/",
                "hm1l0k5AMlg6gn09SDQN9hmLtYkziLIQZwiR59Um559dGAnvjsRl1WhWIywJwU6iujkVMRsZUDIyh3PS1ZcWuQ==",
                "Tasks"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .ToContainer("X12_315")
                .HasOne(item => item.Id);
        }
    }
}
