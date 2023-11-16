namespace UnitTests;
using Microsoft.Data.Sqlite;
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
        var queryable = request.AsQueryable();
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
                EquipmentId = "OSC103",
                StatusId = 1,
                ProfessorNip = 0,
                DispatchTime = DateTime.Now,
                ReturnTime = DateTime.Now,
                RequestedDate = DateTime.Now,
                CurrentDate = DateTime.Now
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
        List<Subject> subjeccts = new List<Subject>()
        {
            new Subject 
            {
                SubjectId = "hkjshdfj",
                nameof = ""
                RequestDetailsId = 1,
                RequestId = 1,
                EquipmentId = "OSC103",
                StatusId = 1,
                ProfessorNip = 0,
                DispatchTime = DateTime.Now,
                ReturnTime = DateTime.Now,
                RequestedDate = DateTime.Now,
                CurrentDate = DateTime.Now
            }
        };

        var mockDbSet = new Mock<DbSet<Request>>();
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.Provider).Returns(requestsDetails.AsQueryable().Provider);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.Expression).Returns(requestsDetails.AsQueryable().Expression);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.ElementType).Returns(requestsDetails.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<RequestDetail>>().Setup(x => x.GetEnumerator()).Returns(() => requestsDetails.GetEnumerator());

        var mockDbContext = new Mock<IAll>();
        mockDbContext.Setup(x => x.Requests).Returns(mockDbSet.Object);

        return mockDbContext;
    }

                                    /*Fin configuraciones Mockup*/

    [Fact]
    public void TestMOCK_01()
    {
       var mockDbContext = ConfigureMockDbContextStorers();
       string ACT=Functions.AddStorerForUt(mockDbContext.Object);
       Assert.NotNull(ACT);
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
        var mockDbContext = 
        
    }

    [Fact]
    public void ListEquipmentsRequest_NotContainNullValues()
    {
        var mockDbContext = ConfigureMockDbContextRequestsDetails();
        var result = Functions.ListEquipmentsRequests(mockDbContext.Object);

        Assert.NotNull(result);
    }








    // sam, furri y caro UnitTests
    [Fact]
    public void TestVerifyPlantelArgumentEx(){
        // Arrange
        string Plantel = "Tonala";
        // Act y assert
        
        Assert.Throws<ArgumentException>(() => Program.VerifyPlantel(Plantel));
    }

    [Fact]
    public void TestIsAlphabetic(){
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
    public void NoPermissionToWatch()
    {
        //Arrange
        var mockDbContext = ConfigureMockDbContextRequestsDetails();
        //Act
        bool ACT=Functions.WatchPermissions(Functions.EncryptPass("1010101010"), mockDbContext.Object);
       //Assert
       Assert.NotNull(ACT);
       
    }

    [Fact]
    public void TestChangeStorerPassword()
    {
        //Arrange
        var mockDbContext = ConfigureMockDbContextStorers();
        var input = "Colomos23\n2023/15/11\n2023/11/15\n2023/11/21\n";

        using (var stringReader = new StringReader(input))
        {
            //Act
            bool act = Functions.ChangeStorerPsw(Functions.EncryptPass("1718192021"), mockDbContext.Object);
            //Assert
            Assert.NotNull(act);
        }        
    }

    [Fact]
    public void Test1()
    {

    }

    [Fact]
    public void Test1()
    {

    }

    [Fact]
    public void Test1()
    {

    }
}