using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Principal.AutoGens;

[Table("classrooms")]
public partial class Classroom
{
    [Key]
    [Column("classroomId")]
    [Required]
    public int ClassroomId { get; set; }

    [Column("name", TypeName = "VARCHAR (40)")]
    [Required]
    [StringLength(40)]
    public string? Name { get; set; }

    [Column("clave", TypeName = "VARCHAR (8)")]
    [Required]
    [StringLength(8)]
    public string? Clave { get; set; }

    [Column("divisionId")]
    [Required]
    public byte? DivisionId { get; set; }

    [ForeignKey("DivisionId")]
    [InverseProperty("Classroom")]
    public virtual Division? Division { get; set; }

    [InverseProperty("Classroom")]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    [InverseProperty("Classroom")]
    public virtual ICollection<Teach> Teaches { get; set; } = new List<Teach>();
}
