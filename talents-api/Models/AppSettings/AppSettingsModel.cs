using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.AppSettings
{
    public class AppSettingsModel
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public Config Config { get; set; }
        public SendGrid SendGrid { get; set; }
        public JwtConfig JwtConfig { get; set; }
    }

    public class ConnectionStrings
    {
        public string MainDatabase { get; set; }
    }
    public class JwtConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }

    public class Config
    {
        public string FilesPath { get; set; }
        public string EncryptionSalt { get; set; }
        public string UiProjectUrl { get; set; }
        public double TokenValidityHours { get; set; }
    }

    public class SendGrid
    {
        public string ApiKey { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
    }
}
