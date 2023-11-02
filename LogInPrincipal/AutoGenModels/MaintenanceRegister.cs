using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("maintenanceRegister")]
public partial class MaintenanceRegister
{
    [Key]
    [Column("maintenanceId")]
    public long MaintenanceId { get; set; }

    [Column("maintenanceTypeId")]
    public long? MaintenanceTypeId { get; set; }

    [Column("maintenanceInstructions", TypeName = "VARCHAR (255)")]
    public string? MaintenanceInstructions { get; set; }

    [Column("programmedDate", TypeName = "DATE")]
    public byte[]? ProgrammedDate { get; set; }

    [Column("exitDate", TypeName = "DATE")]
    public byte[]? ExitDate { get; set; }

    [Column("maintenanceDescription", TypeName = "VARCHAR (255)")]
    public string? MaintenanceDescription { get; set; }

    [Column("storerId", TypeName = "CHAR (10)")]
    public string? StorerId { get; set; }

    [Column("maintenanceMaterialsDescription", TypeName = "VARCHAR (100)")]
    public string? MaintenanceMaterialsDescription { get; set; }

    [InverseProperty("Maintenance")]
    public virtual ICollection<Maintain> Maintains { get; set; } = new List<Maintain>();

    [ForeignKey("MaintenanceTypeId")]
    [InverseProperty("MaintenanceRegisters")]
    public virtual MaintenanceType? MaintenanceType { get; set; }

    [ForeignKey("StorerId")]
    [InverseProperty("MaintenanceRegisters")]
    public virtual Storer? Storer { get; set; }
}
