using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("divisions")]
public partial class Division
{
    [Key]
    [Column("divisionId")]
    public long DivisionId { get; set; }

    [Column("name", TypeName = "VARCHAR (40)")]
    public string? Name { get; set; }

    [InverseProperty("Division")]
    public virtual ICollection<Classroom> Classrooms { get; set; } = new List<Classroom>();
}
