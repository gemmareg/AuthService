using AuthService.Application.Services;

namespace AuthService.Application.UnitTest.Services
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _service = new PasswordService();

        [Fact]
        public void Hash_ShouldReturnNonEmptyString()
        {
            // Arrange
            var password = "MySecurePassword123!";
            password = "";

            // Act
            var hash = _service.Hash(password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.Contains(".", hash); // formato esperado: iterations.salt.hash }
        }

        [Fact]
        public void Hash_ShouldGenerateDifferentHashesForSamePassword()
        {
            // Arrange
            var password = "SamePassword";

            // Act
            var hash1 = _service.Hash(password);
            var hash2 = _service.Hash(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // el salt debe hacerlos distintos }
        }

        [Fact]
        public void Verify_ShouldReturnTrueForCorrectPassword()
        {
            // Arrange
            var password = "CorrectPassword!";
            var hash = _service.Hash(password);

            // Act
            var result = _service.Verify(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Verify_ShouldReturnFalseForIncorrectPassword()
        {
            // Arrange
            var password = "CorrectPassword!";
            var wrongPassword = "WrongPassword!";
            var hash = _service.Hash(password);

            // Act
            var result = _service.Verify(wrongPassword, hash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Verify_ShouldReturnFalseForInvalidHashFormat()
        {
            // Arrange
            var password = "Password123";
            var invalidHash = "this-is-not-a-valid-hash";

            // Act
            var result = _service.Verify(password, invalidHash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Verify_ShouldReturnFalse_WhenHashHasInvalidBase64()
        { 
            // Arrange
            var password = "Password123"; 
            var invalidHash = "100000.invalid-base64.invalid-base64"; 
            
            // Act & Assert
            var exception = Assert.Throws<FormatException>(
                () => _service.Verify(password, invalidHash)
            );
            
            Assert.NotNull(exception);
        }
        
    }
}