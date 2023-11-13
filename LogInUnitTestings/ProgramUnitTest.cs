namespace LogInUnitTestings;
using AutoGens;
using Microsoft.Data.Sqlite;
using Moq;

public class LogInUnitTest
{
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
    /*
    [Fact]
    public void TestInputStringToInt()
    {
        // Arrange
        var input = "John\nDoe\nSmith\n12345\npass123\n2\nMath\nScience\nDivision A";

        // Simula la entrada del usuario
        using (var stringReader = new StringReader(input))
        {
            Console.SetIn(stringReader);

            // Act
            var profesor = todo.input_info();

            // Encripta el número de nómina proporcionado en la entrada del usuario
            var encryptedNomina = todo.AESEncryption.Encrypt("12345");

            // Assert
            Assert.Equal("John", profesor.NombreProfesor.PrimerNombre);
            Assert.Equal(encryptedNomina, profesor.Nomina);
            Assert.Contains("Math", profesor.Materias);
            Assert.Contains("Science", profesor.Materias);
            Assert.Equal("Division A", profesor.Division);
        }
    }
    */
}