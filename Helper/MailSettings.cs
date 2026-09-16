using Microsoft.Extensions.Options;

namespace UGB.MVC.Helper
{
    public class MailSettings : IConfigureOptions<SettingsBase>
    {
        private readonly IConfiguration conf;

        public MailSettings(IConfiguration configuration)
        {
            conf = configuration;
        }

        public void Configure(SettingsBase options)
        {
            conf.GetSection("MailSettings").Bind(options);
        }
    }

    public class SettingsBase
    {
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string SMTP { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool UsePickupDirectory { get; set; } = true;
        public string PickupDirectory { get; set; } = "App_Data/MailDrop";
    }
}
