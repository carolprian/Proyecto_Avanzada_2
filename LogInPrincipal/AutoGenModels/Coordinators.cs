using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("coordinators")]
public partial class Coordinators
{
    [Key]
    [Column("coordinatorId", TypeName = "CHAR (10)")]
    public string CoordinatorId { get; set; } = null!;

    [Column("name", TypeName = "VARCHAR (30)")]
    public string? Name { get; set; }

    [Column("lastNameP", TypeName = "VARCHAR (30)")]
    public string? LastNameP { get; set; }

    [Column("lastNameM", TypeName = "VARCHAR (30)")]
    public string? LastNameM { get; set; }

    [Column("password", TypeName = "VARCHAR (50)")]
    public string? Password { get; set; }

    [InverseProperty("Coordinator")]
    public virtual ICollection<DyLequipment> DyLequipments { get; set; } = new List<DyLequipment>();

    [InverseProperty("Coordinator")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
