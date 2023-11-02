﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Principal.AutoGens;

[Table("status")]
public partial class Status
{
    [Key]
    [Column("statusId")]
    [Required]
    public byte StatusId { get; set; }

    [Column("value", TypeName = "VARCHAR (15)")]
    [Required]
    [MaxLenght(15)]
    public string? Value { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<DyLequipment> DyLequipments { get; set; } = new List<DyLequipment>();

    [InverseProperty("Status")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

    [InverseProperty("Status")]
    public virtual ICollection<RequestDetail> RequestDetails { get; set; } = new List<RequestDetail>();
}
