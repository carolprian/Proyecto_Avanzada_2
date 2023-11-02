using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("maintain")]
public partial class Maintain
{
    [Key]
    [Column("maintainId")]
    public long MaintainId { get; set; }

    [Column("maintenanceId")]
    public long? MaintenanceId { get; set; }

    [Column("equipmentId", TypeName = "VARCHAR (15)")]
    public string? EquipmentId { get; set; }

    [ForeignKey("EquipmentId")]
    [InverseProperty("Maintains")]
    public virtual Equipment? Equipment { get; set; }

    [ForeignKey("MaintenanceId")]
    [InverseProperty("Maintains")]
    public virtual MaintenanceRegister? Maintenance { get; set; }
}
