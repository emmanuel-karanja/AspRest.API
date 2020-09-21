namespace AspRest.API.Utils
{
    public class AppConfigSettings
    {
        public string JwtSecret {get;set;}
        public string JwtClaimsIssuer{get;set;}
        public string JwtClaimsAudience{get;set;}
    }
}