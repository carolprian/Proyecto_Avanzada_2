using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("DyLEquipments")]
public partial class DyLequipment
{
    [Key]
    [Column("DyLEquipmentId")]
    public long DyLequipmentId { get; set; }

    [Column("statusId")]
    public long? StatusId { get; set; }

    [Column("equipmentId", TypeName = "VARCHAR (15)")]
    public string? EquipmentId { get; set; }

    [Column("description", TypeName = "VARCHAR (200)")]
    public string? Description { get; set; }

    [Column("dateOfEvent", TypeName = "DATE")]
    public byte[]? DateOfEvent { get; set; }

    [Column("studentId", TypeName = "CHAR (8)")]
    public string? StudentId { get; set; }

    [Column("coordinatorId", TypeName = "CHAR (8)")]
    public string? CoordinatorId { get; set; }

    [ForeignKey("CoordinatorId")]
    [InverseProperty("DyLequipments")]
    public virtual Coordinator? Coordinator { get; set; }

    [ForeignKey("EquipmentId")]
    [InverseProperty("DyLequipments")]
    public virtual Equipment? Equipment { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("DyLequipments")]
    public virtual Status? Status { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("DyLequipments")]
    public virtual Student? Student { get; set; }
}
