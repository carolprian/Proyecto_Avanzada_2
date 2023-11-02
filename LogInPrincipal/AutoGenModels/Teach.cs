using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LogInPrincipal.AutoGens;

[Table("teaches")]
public partial class Teach
{
    [Key]
    [Column("teachId")]
    public long TeachId { get; set; }

    [Column("classroomId", TypeName = "INT")]
    public long? ClassroomId { get; set; }

    [Column("groupId", TypeName = "SMALLINT")]
    public long? GroupId { get; set; }

    [Column("professorId", TypeName = "CHAR (10)")]
    public string? ProfessorId { get; set; }

    [Column("subjectId", TypeName = "VARCHAR (13)")]
    public string? SubjectId { get; set; }

    [Column("scheduleId", TypeName = "INT")]
    public long? ScheduleId { get; set; }

    [ForeignKey("ClassroomId")]
    [InverseProperty("Teaches")]
    public virtual Classroom? Classroom { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("Teaches")]
    public virtual Group? Group { get; set; }

    [ForeignKey("ProfessorId")]
    [InverseProperty("Teaches")]
    public virtual Professor? Professor { get; set; }

    [ForeignKey("ScheduleId")]
    [InverseProperty("Teaches")]
    public virtual Schedule? Schedule { get; set; }

    [ForeignKey("SubjectId")]
    [InverseProperty("Teaches")]
    public virtual Subject? Subject { get; set; }
}
