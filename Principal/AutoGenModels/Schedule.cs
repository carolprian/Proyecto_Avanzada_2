using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Principal.AutoGens;

[Table("schedules")]
public partial class Schedule
{
    [Key]
    [Column("scheduleId")]
    [Required]
    public short ScheduleId { get; set; }

    [Column("initTime", TypeName = "TIME")]
    [Required]
    public DateTime InitTime { get; set; }

    [Column("endTime", TypeName = "TIME")]
    [Required]
    public DateTime EndTime { get; set; }

    [Column("weekDay", TypeName = "VARCHAR (9)")]
    [Required]
    [MaxLenght(9)]
    public string? WeekDay { get; set; }

    [InverseProperty("Schedule")]
    public virtual ICollection<Teach> Teaches { get; set; } = new List<Teach>();
}
