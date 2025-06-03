using System.Security.Claims;
using InvenireServer.Infrastructure.Authentication;

namespace InvenireServer.Tests.Unit.Common;

public class JwtBuilderTests
{
    private readonly JwtBuilder _builder;

    public JwtBuilderTests()
    {
        _builder = new JwtBuilder("issuer", 30);
    }

    [Fact]
    public void Build_ReturnsPayloadWithDefaultClaims()
    {
        // Act & Assert.
        var jwt = _builder.Build([]); // Insert no additional claims.

        // Assert that the payload includes all default claims.
        jwt.Payload.Should().ContainSingle(c => c.Type == "iss" && c.Value == _builder.Issuer);
        jwt.Payload.Should().ContainSingle(c => c.Type == "aud" && c.Value == _builder.Issuer);
        jwt.Payload.Should().ContainSingle(c => c.Type == "exp" && c.Value != null);
    }

    [Fact]
    public void Build_ReturnsPayloadWithProvidedClaims()
    {
        // Act & Assert.
        var claims = new List<Claim>
        {
            new("role", "test"),
            new("purpose", "testing")
        };
        var jwt = _builder.Build(claims);

        // Assert that the payload includes provided claims.
        jwt.Payload.Should().ContainSingle(c => c.Type == claims[0].Type && c.Value == claims[0].Value);
        jwt.Payload.Should().ContainSingle(c => c.Type == claims[1].Type && c.Value == claims[1].Value);
    }

    [Fact]
    public void Parse_ReturnsCorrectJwtRepresentation()
    {
        // Prepare.
        var claims = new List<Claim>
        {
            new("role", "test"),
            new("purpose", "testing")
        };
        var stringJwt = new JwtWriter("test_key").Write(_builder.Build(claims));

        // Act & Assert.
        var parsedJwt = JwtBuilder.Parse(stringJwt);

        // Assert that the payload includes provided claims.
        parsedJwt.Payload.Should().ContainSingle(c => c.Type == claims[0].Type && c.Value == claims[0].Value);
        parsedJwt.Payload.Should().ContainSingle(c => c.Type == claims[1].Type && c.Value == claims[1].Value);
    }

    [Fact]
    public void Parse_ThrowsArgumentExceptionWhenWrongFormat()
    {
        // Act & Assert.
        var action = () => JwtBuilder.Parse(""); // Create a action that will trigger an exception.

        action.Should().Throw<ArgumentException>();
    }
}
