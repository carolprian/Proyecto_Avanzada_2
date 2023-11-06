--
-- File generated with SQLiteStudio v3.4.4 on jue. nov. 2 10:39:45 2023
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
INSERT INTO academies (academyId, name) VALUES (1, 'Computaci�n');
INSERT INTO academies (academyId, name) VALUES (2, 'Inform�tica');
INSERT INTO academies (academyId, name) VALUES (3, 'Sistemas electr�nicos');
INSERT INTO academies (academyId, name) VALUES (4, 'Sistemas digitales');
INSERT INTO academies (academyId, name) VALUES (5, 'Infraestructura y tecnolog�as de la informaci�n');

-- Table: areas
CREATE TABLE IF NOT EXISTS areas (
    areaId INTEGER      PRIMARY KEY AUTOINCREMENT,
    name   VARCHAR (40) 
);
INSERT INTO areas (areaId, name) VALUES (1, 'Osciloscopios');
INSERT INTO areas (areaId, name) VALUES (2, 'Fuentes de alimentaci�n');
INSERT INTO areas (areaId, name) VALUES (3, 'Mult�metros');
INSERT INTO areas (areaId, name) VALUES (4, 'Generadores');
INSERT INTO areas (areaId, name) VALUES (5, 'Frecuenc�metros');
INSERT INTO areas (areaId, name) VALUES (6, 'Programadores');
INSERT INTO areas (areaId, name) VALUES (7, 'Kit de fibra �ptica');
INSERT INTO areas (areaId, name) VALUES (8, 'Herramientas para redes');
INSERT INTO areas (areaId, name) VALUES (9, 'Motores');
INSERT INTO areas (areaId, name) VALUES (10, 'Tarjetas de desarrollo');
INSERT INTO areas (areaId, name) VALUES (11, 'Brazos robot');
INSERT INTO areas (areaId, name) VALUES (12, 'Herramientas para cableado');
INSERT INTO areas (areaId, name) VALUES (13, 'Herramientas para soldar');

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
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (1, 'Laboratorio de c�mputo E', 'F:LABE', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (2, 'Laboratorio de c�mputo D', 'F:LABD', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (3, 'Laboratorio de c�mputo C', 'F:LABC', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (4, 'Laboratorio de c�mputo B', 'F:LABB', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (5, 'Laboratorio de c�mputo A', 'F:LABA', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (6, 'Aula F-215 Interactiva', 'F:I215', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (7, 'Laboratorio de software 1', 'F:SL1', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (8, 'Laboratorio de software libre 3', 'F:SL3', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (9, 'Laboratorio de software libre 2', 'F:SL2', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (10, 'Taller de electr�nica B', 'F-ELECB', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (11, 'Taller de electr�nica C', 'F-ELECC', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (12, 'Taller de electr�nica A', 'F-ELECA', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (13, 'Taller de sistemas digitales II', 'F-LSDIG2', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (14, 'Taller de sistemas digitales I', 'F-LSDIG1', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (15, 'Laboratorio de Redes I', 'F:LRED1', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (16, 'Laboratorio de Redes II', 'F:LRED2', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (17, 'Aula', 'F-204A', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (18, 'Aula', 'F-204B', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (19, 'Aula', 'F-203A', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (20, 'Aula 203B', 'F-203B', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (21, 'Aula 202', 'F-202', 1);
INSERT INTO classrooms (classroomId, name, clave, divisionId) VALUES (22, 'Taller de mantenimiento', 'F-MTO', 1);

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
INSERT INTO divisions (divisionId, name) VALUES (1, 'Desarrollo de software');

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
INSERT INTO maintenanceTypes (maintenanceTypeId, name) VALUES (1, 'Preventivo');
INSERT INTO maintenanceTypes (maintenanceTypeId, name) VALUES (2, 'Correctivo');
INSERT INTO maintenanceTypes (maintenanceTypeId, name) VALUES (3, 'Predictivo');

-- Table: professors
CREATE TABLE IF NOT EXISTS professors (
    professorId CHAR (10)    PRIMARY KEY,
    name        VARCHAR (30),
    lastNameP   VARCHAR (30),
    lastNameM   VARCHAR (30),
    nip         VARCHAR (4),
    password    VARCHAR (50) 
);

INSERT INTO professors (professorId, name, lastNameP, lastNameM, nip, password)
VALUES ('1234567890', 'Alejandra', 'Alcaraz', 'Torres', 'NIP1', 'Colomos23');
VALUES ('1010101010', 'Nancy del Carmen', 'Benavides', 'Medina', 'NIP2', 'Colomos23');
VALUES ('0987654321', 'Diana Marisol', 'Figueroa', 'Flores', 'NIP3', 'Colomos23');
VALUES ('1111111111', 'Clara Gabriela', 'Garcia', 'Duran', 'NIP4', 'Colomos23');
VALUES ('1231231231', 'Carlos', 'Molina', 'Martinez', 'NIP5', 'Colomos23');


-- Table: requestDetails
CREATE TABLE IF NOT EXISTS requestDetails (
    requestDetailsId INTEGER      PRIMARY KEY AUTOINCREMENT,
    requestId        INTEGER      REFERENCES requests (requestId),
    equipmentId      VARCHAR (15) REFERENCES equipments (equipmentId), //se llama id y es int 
    quantity         INTEGER,
    statusId         INTEGER      REFERENCES status (statusId),
    professorNIP     VARCHAR (4),
    dispatchTime     TIME,
    returnTime       TIME,
    requestedDate    DATE,
    currentDate      DATE
);

INSERT INTO requestDetails (requestId, equipmentId, quantity, statusId, professorNIP, dispatchTime, returnTime, requestedDate, currentDate)
VALUES(1, 'multim12345', 2, 1, 'NIP1', '07:00:00', '08:40:00', '2023-11-10', '2023-11-05');
VALUES(1, 'oscilo12345', 1, 1, 'NIP1', '07:00:00', '08:40:00', '2023-11-10', '2023-11-05');
VALUES(2, 'multi1234567', 3, 1, NULL, '10:20:00', '14:30:00', '2023-09-07', '2023-09-05');
VALUES(3, 'mp123123123', 1, 1, 'NIP3', '11:10:00', '12:50:00', '2023-11-08', '2023-11-07');
VALUES(3, 'multim12345', 2, 1, 'NIP3', '11:10:00', '12:50:00', '2023-11-08', '2023-11-07');


-- Table: requests
CREATE TABLE IF NOT EXISTS requests (
    requestId   INTEGER   PRIMARY KEY AUTOINCREMENT,
    classroomId INTEGER   REFERENCES classrooms (classroomId),
    professorId CHAR (10) REFERENCES professors (professorId),
    studentId   CHAR (8)  REFERENCES students (studentId),
    storerId    CHAR (10) REFERENCES storers (storerId),
    subjectId   VARCHAR (13)   REFERENCES subjects (subjectId) 
);

INSERT INTO requests (requestId, classroomId, professorId, studentId, storerId, subjectId)
VALUES (1, 3, '123456789', '20300679', '1', '18MPBDSO101');
VALUES (2, 7, '1010101010', '20300698', '1', '18MPBDSO202');
VALUES (3, 12, '0987654321', '20300826', '1', '18MPBDSO411');

-- Table: schedules
CREATE TABLE IF NOT EXISTS schedules (
    scheduleId INTEGER     PRIMARY KEY AUTOINCREMENT,
    initTime   TIME,
    weekDay    VARCHAR (9) 
);
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (1, '7:00', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (2, '7:50', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (3, '8:40', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (4, '9:30', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (5, '10:20', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (6, '11:10', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (7, '12:00', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (8, '12:50', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (9, '13:40', 'Monday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (10, '7:00', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (11, '7:50', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (12, '8:40', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (13, '9:30', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (14, '10:20', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (15, '11:10', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (16, '12:00', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (17, '12:50', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (18, '13:40', 'Tuesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (19, '7:00', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (20, '7:50', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (21, '8:40', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (22, '9:30', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (23, '10:20', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (24, '11:10', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (25, '12:00', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (26, '12:50', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (27, '13:40', 'Wednesday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (28, '7:00', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (29, '7:50', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (30, '8:40', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (31, '9:30', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (32, '10:20', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (33, '11:10', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (34, '12:00', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (35, '12:50', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (36, '13:40', 'Thursday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (37, '7:00', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (38, '7:50', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (39, '8:40', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (40, '9:30', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (41, '10:20', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (42, '11:10', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (43, '12:00', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (44, '12:50', 'Friday');
INSERT INTO schedules (scheduleId, initTime, weekDay) VALUES (45, '13:40', 'Friday');

-- Table: status
CREATE TABLE IF NOT EXISTS status (
    statusId INTEGER      PRIMARY KEY AUTOINCREMENT,
    value    VARCHAR (15) 
);
INSERT INTO status (statusId, value) VALUES (1, 'Available');
INSERT INTO status (statusId, value) VALUES (2, 'In use');
INSERT INTO status (statusId, value) VALUES (3, 'Lost');
INSERT INTO status (statusId, value) VALUES (4, 'Damaged');
INSERT INTO status (statusId, value) VALUES (5, 'Under Maintenance');

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
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0101', 'Fundamentos de Programaci�n', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0202', 'Fundamentos de Electr�nica I', 3);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0203', 'Mantenimiento de Tecnolog�as de Informaci�n I', 5);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0204', 'Programaci�n I', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0305', 'Mantenimiento de Tecnolog�as de Informaci�n II', 5);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0306', 'Programaci�n II', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0307', 'Sistemas Digitales I', 4);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0308', 'Fundamentos de Electr�nica II', 3);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0409', 'Sistemas Digitales II', 4);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0410', 'An�lisis y Dise�o de Sistemas', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0411', 'Infraestructura de Redes Locales', 5);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0412', 'Programaci�n Orientada a Objetos', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0413', 'Temas de Electr�nica I', 3);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0514', 'Arquitectura y Organizaci�n de Computadoras', 4);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0515', 'Bases de Datos I', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0517', 'Temas de Electr�nica II', 3);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPBDS0518', 'Estructuras de Datos', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0516', 'Enrutamiento de Redes', 5);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0619', 'Bases de Datos II', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0620', 'Interfaces', 3);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0621', 'Programaci�n M�vil I', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0622', 'Sistemas Operativos', 5);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0623', 'Sistemas Embebidos I', 4);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0729', 'Sistemas Embebidos II', 4);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0730', 'Sistemas de Medici�n y Control', 3);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0727', 'Proyecto Integrador de Desarrollo de Software I', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0724', 'Programaci�n M�vil II', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0726', 'Programaci�n Web I', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0728', 'Servicios de Red y C�mputo Nube', 5);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0725', 'Programaci�n Avanzada I', 1);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0835', 'Seguridad en Infraestructura de Tecnolog�as de Informaci�n', 5);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0836', 'Sistemas Inteligentes', 4);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDSS063', 'Proyecto Integrador de Desarrollo de Software II', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS833', 'Seguridad en Software', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0832', 'Programaci�n Web II', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0834', 'Sistemas Multimedia', 2);
INSERT INTO subjects (subjectId, name, academyId) VALUES ('18MPEDS0831', 'Programaci�n Avanzada II', 1);

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
