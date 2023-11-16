namespace UnitTests;
using Microsoft.Data.Sqlite;
using console;
//using Moq;

//new
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using AutoGens;
using FunctionsName;
using Data;
using System.Collections.Generic;
using System.Linq;

public class UnitTest1
{

    // configuration of the new mockDbContext Status
    private Mock<IAll> ConfigureMockDbContextStatus()
    {
        List<Status> statuses = new List<Status>()
        {
            // Multímetro
            new Status
            {
                StatusId = 1,
                Value = "Available"
            }
        };

        var mockDbSet = new Mock<DbSet<Status>>();
        var queryable = statuses.AsQueryable();
        mockDbSet.As<IQueryable<Status>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Status>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Status>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Status>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Statuses).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    private Mock<IAll> ConfigureMockDbContextClassrooms()
    {
        List<Classroom> classrooms = new List<Classroom>()
        {
            // Multímetro
            new Classroom
            {
                ClassroomId = 1,
                Name = "Laboratorio de computo C",
                Clave = "LAB-C"
            }
        };

        var mockDbSet = new Mock<DbSet<Classroom>>();
        var queryable = classrooms.AsQueryable();
        mockDbSet.As<IQueryable<Classroom>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Classroom>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Classroom>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Classroom>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Classrooms).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    // configuration of the new mockDbContext Equipments
    private Mock<IAll> ConfigureMockDbContextEquipments()
    {
        List<Equipment> equipments = new List<Equipment>()
        {
            // Multímetro
            new Equipment
            {
                EquipmentId = "MM123",
                Name = "Multímetro Digital",
                AreaId = 1,
                Description = "Medición de voltaje, corriente y resistencia",
                Year = 2020,
                StatusId = 1,
                ControlNumber = "111111111111111",
                CoordinatorId = Functions.EncryptPass("1234567890")
            }
        };


        var mockDbSet = new Mock<DbSet<Equipment>>();
        var queryable = equipments.AsQueryable();
        mockDbSet.As<IQueryable<Equipment>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Equipment>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Equipment>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Equipment>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Equipments).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    // configuration of the new mockDbContext Students
    private Mock<IAll> ConfigureMockDbContextStudents()
    {
        List<Student> students = new List<Student>()
        {
            new Student
            {
                StudentId = "20300679",
                Name = "Valeria",
                LastNameP = "Gallegos",
                LastNameM = "Goncalves",
                GroupId = 1,
                Password = Functions.EncryptPass("Colomos23")
            }
        };


        var mockDbSet = new Mock<DbSet<Student>>();
        var queryable = students.AsQueryable();
        mockDbSet.As<IQueryable<Student>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Student>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Student>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Student>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Students).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    // configuration of the new mockDbContext Professors
    private Mock<IAll> ConfigureMockDbContextProfessors()
    {
        List<Professor> professors = new List<Professor>()
        {
            new Professor
            {
                ProfessorId = "1010101010",
                Name = "Nancy",
                LastNameP = "Benavides",
                LastNameM = "Medina",
                Password = Program.EncryptPass("Colomos23"),
                Nip = "2000"
            }
        };

        var mockDbSet = new Mock<DbSet<Professor>>();
        var queryableProfessors = professors.AsQueryable();
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.Provider).Returns(queryableProfessors.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.Expression).Returns(queryableProfessors.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.ElementType).Returns(queryableProfessors.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.GetEnumerator()).Returns(() => professors.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Professors).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    private Mock<IAll> ConfigureMockDbContextSeveralProffesor()
    {
        List<Professor> professors = new List<Professor>()
        {
            new Professor
            {
                ProfessorId = "1010101010",
                Name = "Carlos",
                LastNameP = "Molina",
                LastNameM = "Medina",
                Password = Program.EncryptPass("Colomos23"),
                Nip = "2000"
            },
            new Professor
            {
                ProfessorId ="1234567890",
                Name = "Carlos Alberto",
                LastNameP ="Medina",
                LastNameM = "Gutierrez",
                Password = Program.EncryptPass("Colomos23"),
                Nip= "4000"
            }
        };

        var mockDbSet = new Mock<DbSet<Professor>>();
        var queryableProfessors = professors.AsQueryable();
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.Provider).Returns(queryableProfessors.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.Expression).Returns(queryableProfessors.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.ElementType).Returns(queryableProfessors.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Professor>>().Setup(x => x.GetEnumerator()).Returns(() => professors.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Professors).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    // configuration of the new mockDbContext Storers
    private Mock<IAll> ConfigureMockDbContextStorers()
    {
        List<Storer> storers = new List<Storer>()
        {
            new Storer
            {
                StorerId = "1718192021",
                Name = "Anele",
                LastNameP = "Gomez",
                LastNameM = "Mesas",
                Password = Program.EncryptPass("Colomos23"),
                MaintenanceRegisters = new List<MaintenanceRegister>(),
                Requests = new List<Request>(),
                Petitions = new List<Petition>()
            }
        };

        var mockDbSet = new Mock<DbSet<Storer>>();
        var queryableStorers = storers.AsQueryable();
        mockDbSet.As<IQueryable<Storer>>().Setup(x => x.Provider).Returns(queryableStorers.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Storer>>().Setup(x => x.Expression).Returns(queryableStorers.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Storer>>().Setup(x => x.ElementType).Returns(queryableStorers.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Storer>>().Setup(x => x.GetEnumerator()).Returns(() => queryableStorers.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Storers).Returns(mockDbSet.Object);

        return mockDbContext;

    }

    // configuration of the new mockDbContext Coordinators
    private Mock<IAll> ConfigureMockDbContextCoordinators()
    {
        List<Coordinator> coordinators = new List<Coordinator>()
        {
            new Coordinator
            {
                CoordinatorId = "1231231231",
                Name = "Andres",
                LastNameP = "Figueroa",
                LastNameM = "Flores",
                Password = Functions.EncryptPass("Colomos23"),
            }
        };

        var mockDbSet = new Mock<DbSet<Coordinator>>();
        var queryable = coordinators.AsQueryable();
        mockDbSet.As<IQueryable<Coordinator>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Coordinator>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Coordinator>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Coordinator>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Coordinators).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    // configuration of the new mockDbContext Request NO TERMINADO
    private Mock<IAll> ConfigureMockDbContextRequests()
    {
        List<Request> requests = new List<Request>()
        {
            new Request
            {
                RequestId = 1,
                ClassroomId = 22,
                ProfessorId = Functions.EncryptPass("1010101010"),
                StudentId = "20300679",
                StorerId = Functions.EncryptPass("1718192021"),
                SubjectId = "182392839"
            }
        };

        var mockDbSet = new Mock<DbSet<Request>>();
        var queryable = requests.AsQueryable();
        mockDbSet.As<IQueryable<Request>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Request>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Request>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Request>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Requests).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    private Mock<IAll> ConfigureMockDbContextRequestsDetails()
    {
        List<RequestDetail> requestsDetails = new List<RequestDetail>()
        {
            new RequestDetail
            {
                RequestDetailsId = 1,
                RequestId = 1,
                EquipmentId = null,
                StatusId = 1,
                ProfessorNip = 1,
                DispatchTime = new DateTime(2023, 11, 16, 12, 0, 0),
                ReturnTime = new DateTime(2023, 11, 16, 12, 50, 0),
                RequestedDate = new DateTime(2023, 11, 15),
                CurrentDate = new DateTime(2023, 11, 15)
            }
        };

        var mockDbSet = new Mock<DbSet<Request>>();
        var queryable = requestsDetails.AsQueryable();
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Requests).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    private Mock<IAll> ConfigureMockDbContextRequestsDetailsFromToday()
    {
        List<RequestDetail> requestsDetails = new List<RequestDetail>()
        {
            new RequestDetail
            {
                RequestDetailsId = 1,
                RequestId = 1,
                EquipmentId = null,
                StatusId = 1,
                ProfessorNip = 1,
                DispatchTime = new DateTime(2023, 11, 21, 12, 0, 0),
                ReturnTime = new DateTime(2023, 11, 21, 12, 50, 0),
                RequestedDate = new DateTime(2023, 11, 21),
                CurrentDate = new DateTime(2023, 11, 16)
            }
        };

        var mockDbSet = new Mock<DbSet<Request>>();
        var queryable = requestsDetails.AsQueryable();
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.Provider).Returns(queryable.AsQueryable().Provider);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.Expression).Returns(queryable.AsQueryable().Expression);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.ElementType).Returns(queryable.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Requests).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    private Mock<IAll> ConfigureMockDbContextSubjects()
    {
        List<Subject> subjects = new List<Subject>()
        {
            new Subject
            {
                SubjectId = "hkjshdfj",
                Name = "Interfaces",
                AcademyId = 1
            }
        };

        var mockDbSet = new Mock<DbSet<Subject>>();
        mockDbSet.As<IQueryable<Subject>>().Setup(x => x.Provider).Returns(subjects.AsQueryable().Provider);
        mockDbSet.As<IQueryable<Subject>>().Setup(x => x.Expression).Returns(subjects.AsQueryable().Expression);
        mockDbSet.As<IQueryable<Subject>>().Setup(x => x.ElementType).Returns(subjects.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<Subject>>().Setup(x => x.GetEnumerator()).Returns(() => subjects.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Subjects).Returns(mockDbSet.Object);

        return mockDbContext;
    }

    /*Fin configuraciones Mockup*/

    [Fact]
    public void TestAddStorer()
    {
        var mockDbContext = ConfigureMockDbContextStorers();
        string ACT = Functions.AddStorerForUt(mockDbContext.Object);
        Assert.NotNull(ACT);
    }
    [Fact]
    public void TestListStorers()
    {
        var mockDbContext = ConfigureMockDbContextStorers();
        bool ACT = Functions.ListStorers(mockDbContext.Object);
        Assert.Equal(true,ACT);
    }
    [Fact]
    public void TestListStorers_WithNullReferenceException()
    {
        //arrange
        //act and Assert
        Assert.Throws<NullReferenceException>(()=> Functions.ListStorers(null));
    }

    [Fact]
    public void TestListClassroom()
    {
        var mockDbContext = ConfigureMockDbContextClassrooms();
        int act = Functions.ListClassroomsForUt(mockDbContext.Object);
        Assert.Equal(act, 1);
    }

    /*Inicio Unit Tests  Nuevassss*/


    [Fact]
    public void TestVerifyControlNumberThatAlreadyExists()
    {
        // arrange
        string controlnumber = "111111111111111";
        var mockDbContext = ConfigureMockDbContextEquipments();
        bool expected = true;
        //act
        bool actual = Functions.VerifyControlNumberExistence(controlnumber, mockDbContext.Object);
        //assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestListSubjectIfThereIsAny()
    {
        //arrange
        var mockDbContext = ConfigureMockDbContextSubjects();
        bool expected = false;
        //act
        bool actual = Functions.ListSubjects(mockDbContext.Object);
        //assert
        Assert.Equal(expected, actual);
    }

    

    [Fact]
    public void TestUpdateRequestStatusDefaultIsMinusOneExpectZeroAffectedsWithANullParameter()
    {
        var mockDbContext = ConfigureMockDbContextRequestsDetails();
        string RequestId = "1";
        //act 
        //assert
        Assert.Throws<ArgumentNullException>(()=> Functions.UpdateRequestFormatStatus(RequestId,mockDbContext.Object));
    }
    
    [Fact]
    public void TestStudentsLateReturnStatus1Nip1RequestedDateMenorACurrentDate_NullReferenceException()
    {// status needs to be 2, and it is not
        //arrange
        var mockDbContext = ConfigureMockDbContextRequestsDetails();
        bool expected = true;
        //act
        Assert.Throws<NullReferenceException>(()=> Functions.StudentsLateReturn(mockDbContext.Object));
    }
    
    
    [Fact]
    public void TestListAreasWithWrongMock()
    {
        var mockDbContext = ConfigureMockDbContextStatus();
        int expected = 0;
        int actual = Functions.ListAreas(mockDbContext.Object);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestEquipmentRequestChangeStatus_()
    {
        string RequestId = "1";
        var mockDbContext = ConfigureMockDbContextRequestsDetails();     
        Assert.Throws<ArgumentNullException>(()=> Functions.UpdateRequestEquipmentsStatus(RequestId, mockDbContext.Object));
    }

    [Fact]
    public void ListEquipmentsRequest_NullReferenceException()
    {
        // Arrange
        var mockDbContext = ConfigureMockDbContextRequestsDetails();

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => Functions.ListEquipmentsRequests(mockDbContext.Object));
    }

    [Fact]
    public void SearchEquipmentsById_ShouldThrowExceptionOnNullOrEmptySearchTerm()
    {
        // Arrange
        var mockDbContext = ConfigureMockDbContextEquipments();
        var mockConsoleInput = new Mock<IConsoleInput>();
        var mockConsoleOutput = new Mock<IConsoleOutput>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            Functions.SearchEquipmentsById(mockConsoleInput.Object, mockConsoleOutput.Object, mockDbContext.Object, null);
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            Functions.SearchEquipmentsById(mockConsoleInput.Object, mockConsoleOutput.Object, mockDbContext.Object, "");
        });
    }

    [Fact]
    public void MenuSignUp_ShouldReturnValidOption()
    {
        // Arrange
        var mockConsoleInput = new Mock<IConsoleInput>(); 
        mockConsoleInput.SetupSequence(x => x.ReadLine()) //SetupSequence es un metodo proporcionado por Moq y establece un comportamiento en especifico
        //ejemplo si quiero llamarlo varias veces y hacer que devuelva diferentes valores, puedo usar el SetupSecuences y el return()
            .Returns("3");

        var mockConsoleOutput = new Mock<IConsoleOutput>();

        // Act
        var result = Functions.MenuSignUp(mockConsoleInput.Object, mockConsoleOutput.Object);

        // Assert
        Assert.Equal(3, result);  // Verificar que la función devuelve la opción esperada
        mockConsoleOutput.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(8));  // Verificar que WriteLine se llamo 8 veces
    }

    // sam, furri y caro UnitTests
    [Fact]
    public void TestVerifyPlantelArgumentEx()
    {
        // Arrange
        string Plantel = "Tonala";
        // Act y assert

        Assert.Throws<ArgumentException>(() => Program.VerifyPlantel(Plantel));
    }

    [Fact]
    public void TestIsAlphabetic()
    {
        // Arrange
        string Invalid = "Tonala1234";
        bool expected = false;
        // Act 
        bool actual = Program.IsAlphabetic(Invalid);
        Assert.Equal(actual, expected);
    }

    [Fact]
    public void TestDeleteNotExistUser()
    {
        // Arrange
        string username = "000000000";

        // Act
        var ex = Assert.Throws<SqliteException>(() => Program.DeleteRequestFormat(username));

        // Assert
        Assert.IsType<SqliteException>(ex);
    }

    [Fact]
    public void TestEncryptPass_LongPlainText()
    {
        // Arrange
        string longPlainText = new string('a', 1000000000);

        // Act & Assert
        Assert.Throws<OutOfMemoryException>(() => Program.EncryptPass(longPlainText));
    }

    [Fact]
    public void TestDecrypt_InvalidCipherText()
    {
        // Arrange
        string invalidCipherText = "texto_cifrado_invalido";

        // Act & Assert
        Assert.Throws<FormatException>(() => Program.Decrypt(invalidCipherText));
    }

    [Fact]
    public void TestSearchEquipmentsById()
    {
        // Arrange
        string searchTerm = string.Empty;

        var dbContextMock = new Mock<bd_storage>();

        dbContextMock.Setup(db => db.Equipments).Throws<InvalidOperationException>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => Program.SearchEquipmentsById(searchTerm));
    }

    [Fact]
    public void DeleteRequest_WithInvalidId()
    {
        // Arrange
        int invalidRequestId = unchecked(int.MaxValue + 1);

        // Act & Assert
        Assert.Throws<SqliteException>(() => Program.DeleteRequest(invalidRequestId));
    }


    [Fact]
    public void TestInputStringToInt()
    {
        // Arrange
        string input = "1";

        // Act
        int actual = Program.TryParseStringaEntero(input);

        // Assert
        Assert.Equal(1, actual);
    }


    [Fact]
    public void TestReadExactNumberOfChars()
    {
        // Arrange
        var input = "TESTONE\nTESTTWO\nTESTTHREE\nTESTFOUR\n";

        using (var stringReader = new StringReader(input))
        {
            Console.SetIn(stringReader);

            // Act
            string actual = Program.VerifyReadLengthStringExact(9);

            // Assert
            Assert.Equal("TESTTHREE", actual);
        }
    }
    
    [Fact]
    public void TestDateRequest()
    {
        var input = "2023/14/11\n2023/11/21\n2023/12/14\n";

        using (var stringReader = new StringReader (input))
        {
           Console.SetIn(stringReader);
           DateTime actual = Functions.AddDate(DateTime.Now);
           DateTime expected = new DateTime(2023, 11, 21);
           Assert.Equal(actual, expected);
        }
    }


    [Fact]
    public void TestReadMinimumNumberOfChars()
    {
        // Arrange
        var input = "TESTONE\nTESTTWO\nTESTTHREE\nTESTFOUR\n";

        using (var stringReader = new StringReader(input))
        {
            Console.SetIn(stringReader);

            // Act
            string actual = Program.VerifyReadLengthString(8);

            // Assert
            Assert.Equal("TESTTHREE", actual);
        }
    }

    [Fact]
    public void TestReadMaximumNumberOfChars()
    {
        // Arrange
        var input = "TESTONE\nTESTTWO\nTEST3\nTESTFOUR\n";

        using (var stringReader = new StringReader(input))
        {
            Console.SetIn(stringReader);

            // Act
            string actual = Program.VerifyReadMaxLengthString(5);

            // Assert
            Assert.Equal("TEST3", actual);
        }
    }

    [Fact]
    public void TestReadNonEmptyLine()
    {
        // Arrange
        var input = "\n \nTEST\nTEST2\n";

        using (var stringReader = new StringReader(input))
        {
            Console.SetIn(stringReader);

            // Act
            string actual = Program.ReadNonEmptyLine();

            // Assert
            Assert.Equal("TEST", actual);

        }
    }

    [Fact]
    public void TestReadDateTime()
    {
        // Arrange
        var input = "11/15/2023\n2023/15/11\n2023/11/15\n2023/11/21\n";
        DateTime currentDate = new(2023, 11, 13);

        using (var stringReader = new StringReader(input))
        {
            Console.SetIn(stringReader);

            // Act
            DateTime actual = Program.ReturnMaintenanceDate(currentDate);

            // Assert
            Assert.Equal(2023, actual.Year);
            Assert.Equal(11, actual.Month);
            Assert.Equal(15, actual.Day);
        }
    }

    [Fact]
    public void TestHasUpperCase()
    {
        // Arrange
        string text = "Hello World";

        // Act
        bool result = text.Any(char.IsUpper);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestHasNumeric()
    {
        // Arrange
        string text = "abc123";

        // Act
        bool result = text.Any(char.IsDigit);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestVerifyReadLengthStringExact()
    {
        // Arrange
        int expectedCharacters = 5;
        string input = "ABCDE";
        
        // Set up a StringReader to simulate user input
        using (StringReader reader = new StringReader(input + Environment.NewLine))
        {
            Console.SetIn(reader);

            // Act
            string result = Program.VerifyReadLengthStringExact(expectedCharacters);

            // Assert
            Assert.Equal(input, result);
        }
    }

    [Fact]
    public void TestIsAlphabeticValidString()
    {
        // Arrange
        string input1 = "ValidString";
        string input2 = "   ";
        string input3 = "Invalid123";

        // Act
        bool result1 = Program.IsAlphabetic(input1);
        bool result2 = Program.IsAlphabetic(input2);
        bool result3 = Program.IsAlphabetic(input3);

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.False(result3);
    }

    [Fact]
    public void TestChangeStorerPassword()
    {
        //Arrange
        var mockDbContext = ConfigureMockDbContextStorers();
        var input = "Colomos23\nColomos2023\n";

        using (var stringReader = new StringReader(input))
        {
            //Act
            var act = Functions.ChangeStorerPsw(Functions.EncryptPass("1718192021"), mockDbContext.Object);
            //Assert
            Assert.NotNull(act.affected);
        }
    }
}