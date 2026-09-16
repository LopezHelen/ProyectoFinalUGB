using Microsoft.Extensions.Options;

namespace UGB.MVC.Aplicaciones.Seguras.Helper
{
    public class JwtSettings : IConfigureOptions<JwtSettingsBase>
    {
        private readonly IConfiguration conf;
        public JwtSettings(IConfiguration _conf)
        {
            conf = _conf;
        }

        public void Configure(JwtSettingsBase options)
        {
            conf.GetSection("Jwt").Bind(options);
        }
    }

    public class JwtSettingsBase
    {
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpireMinutes { get; set; } = 240;
    }
}
