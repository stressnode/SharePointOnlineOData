using System.Configuration;

namespace SPO.Services
{
	public class AppSettingService : IAppSettingService
	{
		public string SharePointSiteDomain => ConfigurationManager.AppSettings["SharePointSiteDomain"];
		public string SharePointBaseUrl => ConfigurationManager.AppSettings["SharePointBaseUrl"];
		public string SharePointSiteCollection => ConfigurationManager.AppSettings["SharePointSiteCollection"];
		public string Tenant => ConfigurationManager.AppSettings["Tenant"];
		public string ClientAppId => ConfigurationManager.AppSettings["ClientAppId"];
		public string ClientAppSecret => ConfigurationManager.AppSettings["ClientAppSecret"];
		public string SharePointAudiencePrincipal => ConfigurationManager.AppSettings["SharePointAudiencePrincipal"];
		public string AccessTokenUrl => ConfigurationManager.AppSettings["AccessTokenUrl"];
		public string AccessToken { get; set; }

	}
}
