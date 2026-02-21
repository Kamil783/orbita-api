using Orbita.Contracts.Auth;

namespace Orbita.UnitTests;

public class JwtOptionsTests
{
    [Fact]
    public void JwtOptions_HasExpectedDefaultLifetime()
    {
        var options = new JwtOptions();

        Assert.Equal(60, options.AccessTokenLifetimeMinutes);
    }
}
