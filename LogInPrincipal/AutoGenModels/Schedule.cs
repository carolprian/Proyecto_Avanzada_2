using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("schedules")]
public partial class Schedule
{
    [Key]
    [Column("scheduleId")]
    public long ScheduleId { get; set; }

    [Column("initTime", TypeName = "TIME")]
    public byte[]? InitTime { get; set; }

    [Column("endTime", TypeName = "TIME")]
    public byte[]? EndTime { get; set; }

    [Column("weekDay", TypeName = "VARCHAR (9)")]
    public string? WeekDay { get; set; }

    [InverseProperty("Schedule")]
    public virtual ICollection<Teach> Teaches { get; set; } = new List<Teach>();
}
