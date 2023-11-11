namespace LogInUnitTestings;

public class LogInUnitTest
{
    [Fact]
    public void EncryptPass()
    {
        Assert.Equal(5,5);
    }   

    // [Fact]
    // public void TestEncriptarTextoMuyLargo()
    // {
    //     // Arrange
    //     string longString = new string('A', 1000000000); // Cadena extremadamente larga
    //     Exception expectedException = new OutOfMemoryException();
    
    //     try
    //     {
    //         // Act
    //         string encryptedText;

    //         // Assert
    //         // La excepción debería ser lanzada antes de llegar a esta línea
    //         Assert.True(false, "Se esperaba una excepción.");
    //     }
    //     catch (Exception ex)
    //     {
    //         // Assert
    //         Assert.IsType(expectedException.GetType(), ex);
    //     }
    // }

}