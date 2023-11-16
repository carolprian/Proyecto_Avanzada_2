using AutoGens;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public interface IAll:IDisposable
    {
        DbSet<Academy> Academies {get;}
        DbSet<Areas> Areas {get;}
        DbSet<Classroom> Classrooms {get;}
        DbSet<Coordinator> Coordinators { get; }
        DbSet<Division> Divisions { get; }
        DbSet<DyLequipment> DyLequipments { get;  }
        DbSet<Equipment> Equipments { get; }
        DbSet<Group> Groups { get; }
        DbSet<Maintain> Maintain { get;  }
        DbSet<MaintenanceRegister> MaintenanceRegisters { get; }
        DbSet<MaintenanceType> MaintenanceTypes { get; }
        DbSet<Petition> Petitions { get; }
        DbSet<PetitionDetail> PetitionDetails { get;  }
        DbSet<Professor> Professors { get; }
        DbSet<Request> Requests { get;  }
        DbSet<RequestDetail> RequestDetails { get;}
        DbSet<Schedule> Schedules { get; }
        DbSet<Status> Statuses { get; }
        DbSet<Storer> Storers { get;  }
        DbSet<Student> Students { get; }
        DbSet<Subject> Subjects { get; }
        DbSet<Teach> Teaches { get;}

         int SaveChanges();
    }
}