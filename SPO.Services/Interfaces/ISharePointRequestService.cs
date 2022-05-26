using System.IO;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using SPO.Contracts;

namespace SPO.Services
{
	public interface ISharePointRequestService
	{
		ContextWebInfo GetXRequestDigest(HttpMethod method, string siteUrl);
		HttpWebResponse RequestSharePointOnlineFileRead(string siteUrl, string url);
		HttpWebResponse RequestSharePointOnlineFileCreate(string siteUrl, string url, byte[] file);
		HttpWebResponse RequestSharePointOnline(HttpMethod method, string siteUrl, string url, string payload = null);
		string GetSPOAccessToken();
		List<T> ResponseToListofObject<T>(Stream response, string path);
		T ResponseToObject<T>(Stream response, string path);
		object CreateFolderPayload(string folderPath);
	}
}
