
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

public partial class S3Accounts
{
    [Key]
    [StringLength(50)]
    public string UserName { get; set; }

    public int passwordHash { get; set; }
}
