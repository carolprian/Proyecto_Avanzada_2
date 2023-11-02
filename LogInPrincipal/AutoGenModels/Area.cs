using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("areas")]
public partial class Area
{
    [Key]
    [Column("areaId")]
    public long AreaId { get; set; }

    [Column("name", TypeName = "VARCHAR (40)")]
    public string? Name { get; set; }

    [InverseProperty("Area")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
