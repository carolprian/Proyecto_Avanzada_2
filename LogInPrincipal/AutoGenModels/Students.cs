using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("students")]
public partial class Students
{
    [Key]
    [Column("studentId", TypeName = "CHAR (8)")]
    public string StudentId { get; set; } = null!;

    [Column("name", TypeName = "VARCHAR (30)")]
    public string? Name { get; set; }

    [Column("lastNameP", TypeName = "VARCHAR (30)")]
    public string? LastNameP { get; set; }

    [Column("lastNameM", TypeName = "VARCHAR (30)")]
    public string? LastNameM { get; set; }

    [Column("password", TypeName = "VARCHAR (50)")]
    public string? Password { get; set; }

    [Column("groupId", TypeName = "SMALLINT")]
    public long? GroupId { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<DyLequipment> DyLequipments { get; set; } = new List<DyLequipment>();

    [ForeignKey("GroupId")]
    [InverseProperty("Students")]
    public virtual Group? Group { get; set; }
}
