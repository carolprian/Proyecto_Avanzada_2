using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("subjects")]
public partial class Subject
{
    [Key]
    [Column("subjectId", TypeName = "VARCHAR (13)")]
    public string SubjectId { get; set; } = null!;

    [Column("name", TypeName = "VARCHAR (55)")]
    public string? Name { get; set; }

    [Column("academyId", TypeName = "INTERGER")]
    public long? AcademyId { get; set; }

    [ForeignKey("AcademyId")]
    [InverseProperty("Subjects")]
    public virtual Academy? Academy { get; set; }

    [InverseProperty("Subject")]
    public virtual ICollection<Teach> Teaches { get; set; } = new List<Teach>();
}
