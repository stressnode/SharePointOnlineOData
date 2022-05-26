using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPO.Services
{
	public interface IAppSettingService
	{
		string SharePointSiteDomain { get; }
		string SharePointBaseUrl { get; }
		string SharePointSiteCollection { get; }
		string Tenant { get; }
		string ClientAppId { get; }
		string ClientAppSecret { get; }
		string SharePointAudiencePrincipal { get; }
		string AccessTokenUrl { get; }
		string AccessToken { get; set; }
	}
}
