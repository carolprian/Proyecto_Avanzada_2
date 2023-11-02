using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("classrooms")]
public partial class Classroom
{
    [Key]
    [Column("classroomId")]
    public long ClassroomId { get; set; }

    [Column("name", TypeName = "VARCHAR (40)")]
    public string? Name { get; set; }

    [Column("clave", TypeName = "VARCHAR (8)")]
    public string? Clave { get; set; }

    [Column("divisionId", TypeName = "INT (4)")]
    public long? DivisionId { get; set; }

    [ForeignKey("DivisionId")]
    [InverseProperty("Classrooms")]
    public virtual Division? Division { get; set; }

    [InverseProperty("Classroom")]
    public virtual ICollection<Teach> Teaches { get; set; } = new List<Teach>();
}
