using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("groups")]
public partial class Group
{
    [Key]
    [Column("groupId")]
    public long GroupId { get; set; }

    [Column("name", TypeName = "CHAR (3)")]
    public string? Name { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    [InverseProperty("Group")]
    public virtual ICollection<Teach> Teaches { get; set; } = new List<Teach>();
}
