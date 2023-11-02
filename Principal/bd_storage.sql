--
-- File generated with SQLiteStudio v3.4.4 on lun. oct. 30 19:26:09 2023
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: academies
CREATE TABLE IF NOT EXISTS academies (
    academyId INTEGER      PRIMARY KEY AUTOINCREMENT
                           NOT NULL,
    name      VARCHAR (40) 
);


-- Table: areas
CREATE TABLE IF NOT EXISTS areas (
    areaId INTEGER      PRIMARY KEY AUTOINCREMENT,
    name   VARCHAR (40) 
);


-- Table: classrooms
CREATE TABLE IF NOT EXISTS classrooms (
    classroomId INTEGER      PRIMARY KEY AUTOINCREMENT,
    name        VARCHAR (40),
    clave       VARCHAR (8),
    divisionId  INTEGER      REFERENCES divisions (divisionId),
    FOREIGN KEY (
        divisionId
    )
    REFERENCES divisions (divisionId) 
);


-- Table: coordinators
CREATE TABLE IF NOT EXISTS coordinators (
    coordinatorId CHAR (10)    PRIMARY KEY,
    name          VARCHAR (30),
    lastNameP     VARCHAR (30),
    lastNameM     VARCHAR (30),
    password      VARCHAR (50) 
);


-- Table: divisions
CREATE TABLE IF NOT EXISTS divisions (
    divisionId INTEGER      PRIMARY KEY AUTOINCREMENT,
    name       VARCHAR (40) 
);


-- Table: DyLEquipments
CREATE TABLE IF NOT EXISTS DyLEquipments (
    DyLEquipmentId INTEGER       PRIMARY KEY AUTOINCREMENT,
    statusId       INTEGER,
    equipmentId    VARCHAR (15)  REFERENCES equipments (equipmentId),
    description    VARCHAR (200),
    dateOfEvent    DATE,
    studentId      CHAR (8)      REFERENCES students (studentId),
    coordinatorId  CHAR (10)      REFERENCES coordinators (coordinatorId),
    FOREIGN KEY (
        statusId
    )
    REFERENCES status (statusId),
    FOREIGN KEY (
        equipmentId
    )
    REFERENCES equipments (equipmentId),
    FOREIGN KEY (
        studentId
    )
    REFERENCES students (studentId),
    FOREIGN KEY (
        coordinatorId
    )
    REFERENCES coordinators (coordinatorId) 
);


-- Table: equipments
CREATE TABLE IF NOT EXISTS equipments (
    equipmentId   VARCHAR (15)  PRIMARY KEY,
    name          VARCHAR (40),
    areaId        INTEGER,
    description   VARCHAR (200),
    year          INTEGER,
    statusId      INTEGER,
    controlNumber VARCHAR (20),
    coordinatorId CHAR (10),
    FOREIGN KEY (
        areaId
    )
    REFERENCES areas (areaId),
    FOREIGN KEY (
        statusId
    )
    REFERENCES status (statusId),
    FOREIGN KEY (
        coordinatorId
    )
    REFERENCES coordinators (coordinatorId) 
);


-- Table: groups
CREATE TABLE IF NOT EXISTS groups (
    groupId INTEGER  PRIMARY KEY AUTOINCREMENT,
    name    CHAR (3) 
);


-- Table: maintain
CREATE TABLE IF NOT EXISTS maintain (
    maintainId    INTEGER      PRIMARY KEY AUTOINCREMENT,
    maintenanceId INTEGER      REFERENCES maintenanceRegister (maintenanceId),
    equipmentId   VARCHAR (15) REFERENCES equipments (equipmentId) 
);


-- Table: maintenanceRegister
CREATE TABLE IF NOT EXISTS maintenanceRegister (
    maintenanceId                   INTEGER       PRIMARY KEY AUTOINCREMENT,
    maintenanceTypeId               INTEGER,
    maintenanceInstructions         VARCHAR (255),
    programmedDate                  DATE,
    exitDate                        DATE,
    maintenanceDescription          VARCHAR (255),
    storerId                        CHAR (10),
    maintenanceMaterialsDescription VARCHAR (100),
    FOREIGN KEY (
        maintenanceTypeId
    )
    REFERENCES maintenanceTypes (maintenanceTypeId),
    FOREIGN KEY (
        storerId
    )
    REFERENCES storers (storerId) 
);


-- Table: maintenanceTypes
CREATE TABLE IF NOT EXISTS maintenanceTypes (
    maintenanceTypeId INTEGER      PRIMARY KEY AUTOINCREMENT,
    name              VARCHAR (10) 
);


-- Table: professors
CREATE TABLE IF NOT EXISTS professors (
    professorId CHAR (10)    PRIMARY KEY,
    name        VARCHAR (30),
    lastNameP   VARCHAR (30),
    lastNameM   VARCHAR (30),
    nip         VARCHAR (4),
    password    VARCHAR (50) 
);


-- Table: requestDetails
CREATE TABLE IF NOT EXISTS requestDetails (
    requestDetailsId INTEGER      PRIMARY KEY AUTOINCREMENT,
    requestId        INTEGER      REFERENCES requests (requestId),
    equipmentId      VARCHAR (15) REFERENCES equipments (equipmentId),
    quantity         INTEGER,
    statusId         INTEGER      REFERENCES status (statusId),
    professorNIP     VARCHAR (4),
    dispatchTime     TIME,
    returnTime       TIME,
    requestedDate    DATE,
    currentDate      DATE
);


-- Table: requests
CREATE TABLE IF NOT EXISTS requests (
    requestId   INTEGER   PRIMARY KEY AUTOINCREMENT,
    classroomId INTEGER   REFERENCES classrooms (classroomId),
    professorId CHAR (10) REFERENCES professors (professorId),
    studentId   CHAR (8)  REFERENCES students (studentId),
    storerId    CHAR (10) REFERENCES storers (storerId),
    subjectId   VARCHAR (13)   REFERENCES subjects (subjectId) 
);


-- Table: schedules
CREATE TABLE IF NOT EXISTS schedules (
    scheduleId INTEGER     PRIMARY KEY AUTOINCREMENT,
    initTime   TIME,
    endTime    TIME,
    weekDay    VARCHAR (9) 
);


-- Table: status
CREATE TABLE IF NOT EXISTS status (
    statusId INTEGER      PRIMARY KEY AUTOINCREMENT,
    value    VARCHAR (15) 
);


-- Table: storers
CREATE TABLE IF NOT EXISTS storers (
    storerId  CHAR (10)    PRIMARY KEY,
    name      VARCHAR (30),
    lastNameP VARCHAR (30),
    lastNameM VARCHAR (30),
    password  VARCHAR (50) 
);


-- Table: students
CREATE TABLE IF NOT EXISTS students (
    studentId CHAR (8)     PRIMARY KEY,
    name      VARCHAR (30),
    lastNameP VARCHAR (30),
    lastNameM VARCHAR (30),
    password  VARCHAR (50),
    groupId   INTEGER,
    FOREIGN KEY (
        groupId
    )
    REFERENCES groups (groupId) 
);


-- Table: subjects
CREATE TABLE IF NOT EXISTS subjects (
    subjectId VARCHAR (13) PRIMARY KEY,
    name      VARCHAR (55),
    academyId INTEGER,
    FOREIGN KEY (
        academyId
    )
    REFERENCES academies (academyId) 
);


-- Table: teaches
CREATE TABLE IF NOT EXISTS teaches (
    teachId     INTEGER      PRIMARY KEY AUTOINCREMENT,
    classroomId INTEGER          REFERENCES classrooms (classroomId),
    groupId     INTEGER     REFERENCES groups (groupId),
    professorId CHAR (10)    REFERENCES professors (professorId),
    subjectId   VARCHAR (13) REFERENCES subjects (subjectId),
    scheduleId  INTEGER          REFERENCES schedules (scheduleId) 
);


COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
