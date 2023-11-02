using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("professors")]
public partial class Professors
{
    [Key]
    [Column("professorId", TypeName = "CHAR (10)")]
    public string ProfessorId { get; set; } = null!;

    [Column("name", TypeName = "VARCHAR (30)")]
    public string? Name { get; set; }

    [Column("lastNameP", TypeName = "VARCHAR (30)")]
    public string? LastNameP { get; set; }

    [Column("lastNameM", TypeName = "VARCHAR (30)")]
    public string? LastNameM { get; set; }

    [Column("password", TypeName = "VARCHAR (50)")]
    public string? Password { get; set; }

    [Column("NIP", TypeName = "VARCHAR (4)")]
    public string? Nip { get; set; }

    [InverseProperty("Professor")]
    public virtual ICollection<Teach> Teaches { get; set; } = new List<Teach>();
}
