using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Principal.AutoGens;

[Table("areas")]
public partial class Area
{
    [Key]
    [Column("areaId")]
    [Required]
    public int AreaId { get; set; }

    [Column("name", TypeName = "VARCHAR (40)")]
    [Required]
    [StringLength(40)]
    public string? Name { get; set; }

    [InverseProperty("Area")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
