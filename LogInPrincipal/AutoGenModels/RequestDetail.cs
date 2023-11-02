using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("requestDetails")]
public partial class RequestDetail
{
    [Key]
    [Column("requestDetailsId")]
    public long RequestDetailsId { get; set; }

    [Column("requestId")]
    public long? RequestId { get; set; }

    [Column("equipmentId", TypeName = "VARCHAR (15)")]
    public string? EquipmentId { get; set; }

    [Column("quantity", TypeName = "SMALLINT (4)")]
    public long? Quantity { get; set; }

    [Column("status")]
    public long? Status { get; set; }

    [Column("ProfessorNIP", TypeName = "VARCHAR (4)")]
    public string? ProfessorNip { get; set; }

    [Column("dispatchTime", TypeName = "TIME")]
    public byte[]? DispatchTime { get; set; }

    [Column("returnTime", TypeName = "TIME")]
    public byte[]? ReturnTime { get; set; }

    [Column("requestedDate", TypeName = "DATE")]
    public byte[]? RequestedDate { get; set; }

    [Column("currentDate", TypeName = "DATE")]
    public byte[]? CurrentDate { get; set; }

    [ForeignKey("EquipmentId")]
    [InverseProperty("RequestDetails")]
    public virtual Equipment? Equipment { get; set; }

    [ForeignKey("Status")]
    [InverseProperty("RequestDetails")]
    public virtual Status? StatusNavigation { get; set; }
}
