using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain;
using AuthService.Infrastructure.IntegrationTests.Fixtures;
using AuthService.Infrastructure.Persistance.Repositories;
using static AuthService.Shared.Enums;

namespace AuthService.Infrastructure.IntegrationTests.Repositories
{
    [Collection("Database collection")]
    public class TokenRepositoryTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;

        public TokenRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _tokenRepository = new TokenRepository(_databaseFixture.DbContext);
            _userRepository = new UserRepository(_databaseFixture.DbContext);
        }

        [Fact]
        public async Task PersistToken_BasicCRD()
        {
            // Arrange
            var user = User.Create("johndoe", "johndoe@test.com", "hashedpassword", "John", "Doe").Data!;
            var token = Token.Create(user.Id, TokenType.Access, "1234").Data!;

            // Create token
            await _userRepository.AddAsync(user);
            await _tokenRepository.AddAsync(token);
            await _databaseFixture.DbContext.SaveChangesAsync();

            // Read token
            var retrievedToken = await _tokenRepository.GetByIdAsync(token.Id);
            Assert.NotNull(retrievedToken);

            // Update token
            retrievedToken!.Revoke();
            await _tokenRepository.UpdateAsync(retrievedToken);

            // Verify update
            var updatedToken = await _tokenRepository.GetByIdAsync(token.Id);
            Assert.True(updatedToken!.IsRevoked);

            // Delete token
            await _tokenRepository.RemoveAsync(retrievedToken);
            await _databaseFixture.DbContext.SaveChangesAsync();
            var deletedToken = await _tokenRepository.GetByIdAsync(token.Id);
            Assert.Null(deletedToken);

        }
    }
}