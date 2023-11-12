namespace LogInUnitTestings;
using AutoGens;
public class LogInUnitTest
{
    [Fact]
    public void TestDeleteNotExistUser()
    {
        // Arrange
        string username = "000000000";

        // Act
        var ex = Assert.Throws<Microsoft.Data.Sqlite.SqliteException>(() => Program.DeleteRequestFormat(username));

        // Assert
        Assert.IsType<Microsoft.Data.Sqlite.SqliteException>(ex);
    }

    [Fact]
    public void TestEncryptPass_LongPlainText()
    {
        // Arrange
        string longPlainText = new string('a', 1000000000);

        // Act & Assert
        Assert.Throws<OutOfMemoryException>(() => Program.EncryptPass(longPlainText));
    }

    public class CryptoTests
{
    [Fact]
    public void TestDecrypt_InvalidCipherText()
    {
        // Arrange
        string invalidCipherText = "texto_cifrado_invalido";

        // Act & Assert
        Assert.Throws<FormatException>(() => Program.Decrypt(invalidCipherText));
    }
}


}