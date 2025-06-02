using System.Security.Claims;
using InvenireServer.Application.Core.Authentication;
using InvenireServer.Domain.Core.Entities.Common;

namespace InvenireServer.Tests.Unit.Common;

public class JwtWriterTests
{
    private readonly JwtWriter _writer;

    public JwtWriterTests()
    {
        _writer = new JwtWriter("test");
    }

    public void Write_ReturnsACorrectStringJwtRepresentation()
    {
        // Prepare.
        var header = new List<Claim>
        {
            new("alg", "HS256"),
            new("typ", "JWT")
        };
        var payload = new List<Claim>
        {
            new("role", "test"),
            new("purpose", "testing")
        };
        var jwt = new Jwt([], payload);

        // Act & Assert.
        var stringJwt = _writer.Write(jwt);

        // Assert that the string is not null or empty.
        stringJwt.Should().NotBeNullOrEmpty();
        // Assert that the string contains 2 dots (3 segments).
        stringJwt.Count(c => c == '.').Should().Be(2);
    }
}
