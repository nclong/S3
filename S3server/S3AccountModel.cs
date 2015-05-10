
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class S3AccountModel : DbContext
{
    public S3AccountModel()
        : base( "name=S3AccountModel" )
    {
    }

    public DbSet<S3Accounts> S3Accounts { get; set; }

    protected override void OnModelCreating( DbModelBuilder modelBuilder )
    {
    }
}

