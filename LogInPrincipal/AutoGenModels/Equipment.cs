using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("equipments")]
public partial class Equipment
{
    [Key]
    [Column("equipmentId", TypeName = "VARCHAR (15)")]
    public string EquipmentId { get; set; } = null!;

    [Column("name", TypeName = "VARCHAR (40)")]
    public string? Name { get; set; }

    [Column("areaId", TypeName = "TINYINT (4)")]
    public long? AreaId { get; set; }

    [Column("description", TypeName = "VARCHAR (200)")]
    public string? Description { get; set; }

    [Column("year", TypeName = "SMALLINT (4)")]
    public long? Year { get; set; }

    [Column("statusId", TypeName = "INT (4)")]
    public long? StatusId { get; set; }

    [Column("controlNumber", TypeName = "VARCHAR (20)")]
    public string? ControlNumber { get; set; }

    [Column("coordinatorId", TypeName = "CHAR (10)")]
    public string? CoordinatorId { get; set; }

    [ForeignKey("AreaId")]
    [InverseProperty("Equipment")]
    public virtual Area? Area { get; set; }

    [ForeignKey("CoordinatorId")]
    [InverseProperty("Equipment")]
    public virtual Coordinator? Coordinator { get; set; }

    [InverseProperty("Equipment")]
    public virtual ICollection<DyLequipment> DyLequipments { get; set; } = new List<DyLequipment>();

    [InverseProperty("Equipment")]
    public virtual ICollection<Maintain> Maintains { get; set; } = new List<Maintain>();

    [InverseProperty("Equipment")]
    public virtual ICollection<RequestDetail> RequestDetails { get; set; } = new List<RequestDetail>();

    [ForeignKey("StatusId")]
    [InverseProperty("Equipment")]
    public virtual Status? Status { get; set; }
}
