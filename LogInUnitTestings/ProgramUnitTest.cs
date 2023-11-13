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
}

