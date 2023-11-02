using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("storers")]
public partial class Storers
{
    [Key]
    [Column("storerId", TypeName = "CHAR (10)")]
    public string StorerId { get; set; } = null!;

    [Column("name", TypeName = "VARCHAR (30)")]
    public string? Name { get; set; }

    [Column("lastNameP", TypeName = "VARCHAR (30)")]
    public string? LastNameP { get; set; }

    [Column("lastNameM", TypeName = "VARCHAR (30)")]
    public string? LastNameM { get; set; }

    [Column("password", TypeName = "VARCHAR (50)")]
    public string? Password { get; set; }

    [InverseProperty("Storer")]
    public virtual ICollection<MaintenanceRegister> MaintenanceRegisters { get; set; } = new List<MaintenanceRegister>();
}
