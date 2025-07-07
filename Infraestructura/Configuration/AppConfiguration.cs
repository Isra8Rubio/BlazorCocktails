using Microsoft.Extensions.Configuration;

namespace Infraestructura.Configuration
{
    public class AppConfiguration
    {
        private readonly IConfiguration configuration;
        public JwtConfig Jwt { get; set; } = new JwtConfig();

        public AppConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Load()
        {
            configuration.GetSection("Jwt").Bind(Jwt);
        }
    }

        public class JwtConfig
        {
            public string Key { get; set; } = null!;
            public string Issuer { get; set; } = null!;
            public string Audience { get; set; } = null!;
            public int ExpiresInMinutes { get; set; }
        }
}
