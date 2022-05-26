using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using SPO.Contracts;

namespace SPO.Services
{
	public class SharePointRequestService : ISharePointRequestService
	{
		private readonly IAppSettingService AppSettingService;
		private readonly ICacheService CacheHelper;

		public SharePointRequestService(IAppSettingService appSettingService, ICacheService cacheService)
		{
			this.AppSettingService = appSettingService;
			this.CacheHelper = cacheService;
		}

		public ContextWebInfo GetXRequestDigest(HttpMethod method, string siteUrl)
		{
			HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(this.AppSettingService.SharePointBaseUrl + $"{siteUrl}/_api/contextinfo");

			endpointRequest.Method = method.Method;
			endpointRequest.Accept = "application/json;odata=verbose";
			endpointRequest.Headers.Add("Authorization", $"Bearer {this.AppSettingService.AccessToken}");

			HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();

			return ResponseToObject<ContextWebInfo>(endpointResponse.GetResponseStream(), "GetContextWebInformation");
		}

		public string GetSPOAccessToken()
		{
			this.AppSettingService.AccessToken = "";

			if (this.CacheHelper.GetValue("access_token") == null)
			{

				WebRequest webRequest;
				var accessTokenUrl = string.Format(this.AppSettingService.AccessTokenUrl, this.AppSettingService.Tenant);
				var postData = BuildStringContent();
				var data = Encoding.ASCII.GetBytes(postData);

				webRequest = WebRequest.Create(accessTokenUrl);
				webRequest.ContentType = "application/x-www-form-urlencoded";
				webRequest.Method = "POST";

				using (var stream = webRequest.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}

				var response = (HttpWebResponse)webRequest.GetResponse();
				var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

				SPOAccessToken token = JsonConvert.DeserializeObject<SPOAccessToken>(responseString);

				TimeSpan t = TimeSpan.FromSeconds(double.Parse(token.expires_in));
				Console.WriteLine($"New token requested");
				Console.WriteLine($"Token will expire in { string.Format("{0:D2} hours {1:D2} minutes {2:D2} seconds {3:D3} ms", t.Hours, t.Minutes, t.Seconds, t.Milliseconds) }");
				Console.WriteLine($"That will be on {DateTime.Parse("1970/01/01 00:00:00").AddSeconds(double.Parse(token.expires_on)).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt")}");

				this.AppSettingService.AccessToken = token.access_token;
				this.CacheHelper.Add("access_token", token.access_token, (int)double.Parse(token.expires_in) / 3600);
			}
			else
			{
				this.AppSettingService.AccessToken = this.CacheHelper.GetValue("access_token") as string;
				Console.WriteLine("Token requested from cache");
			}

			return this.AppSettingService.AccessToken;
		}

		public HttpWebResponse RequestSharePointOnline(HttpMethod method, string siteUrl, string odataQuery, string payload = null)
		{
			try
			{
				HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(this.AppSettingService.SharePointBaseUrl + $"{siteUrl}/_api/web/{odataQuery}");

				endpointRequest.Method = method.Method;
				endpointRequest.Accept = "application/json;odata=verbose";
				endpointRequest.Headers.Add("Authorization", $"Bearer {this.AppSettingService.AccessToken}");

				if (payload != null)
				{
					endpointRequest.ContentType = "application/json;odata=verbose";
					endpointRequest.ContentLength = payload.Length;

					var writer = new StreamWriter(endpointRequest.GetRequestStream());

					writer.Write(payload);
					writer.Flush();
				}

				HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();

				return endpointResponse;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.InnerException?.Message ?? ex.Message);
				return null;
			}
		}

		public HttpWebResponse RequestSharePointOnlineFileCreate(string siteUrl, string odataQuery, byte[] file)
		{
			HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(this.AppSettingService.SharePointBaseUrl + $"{siteUrl}/_api/web/{odataQuery}");

			endpointRequest.Method = HttpMethod.Post.Method;
			endpointRequest.Headers.Add("binaryStringRequestBody", "true");
			endpointRequest.Headers.Add("Authorization", "Bearer " + this.AppSettingService.AccessToken);
			endpointRequest.GetRequestStream().Write(file, 0, file.Length);

			HttpWebResponse endpointresponse = (HttpWebResponse)endpointRequest.GetResponse();

			return endpointresponse;
		}

		public HttpWebResponse RequestSharePointOnlineFileRead(string siteUrl, string odataQuery)
		{
			HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(this.AppSettingService.SharePointBaseUrl + $"{siteUrl}/_api/web/{odataQuery}");

			endpointRequest.Method = HttpMethod.Get.Method;
			endpointRequest.Headers.Add("Authorization", "Bearer " + this.AppSettingService.AccessToken);

			HttpWebResponse endpointresponse = (HttpWebResponse)endpointRequest.GetResponse();

			return endpointresponse;
		}

		public List<T> ResponseToListofObject<T>(Stream response, string path)
		{
			using (Stream stream = response)
			{
				var reader = new StreamReader(stream, Encoding.UTF8);
				var responseString = reader.ReadToEnd();
				var jObject = JsonConvert.DeserializeObject<JObject>(responseString);
				var folderContents = jObject.Value<JObject>("d").Value<JArray>(path).ToObject<List<T>>();

				return folderContents;
			}
		}

		public T ResponseToObject<T>(Stream response, string path)
		{
			using (Stream stream = response)
			{
				var reader = new StreamReader(stream, Encoding.UTF8);
				var responseString = reader.ReadToEnd();
				var jObject = JsonConvert.DeserializeObject<JObject>(responseString);
				var folderContents = (
										path == "" ?
										jObject.Value<JObject>("d").ToObject<T>() :
										jObject.Value<JObject>("d").Value<JObject>(path).ToObject<T>()
									 );

				return folderContents;
			}
		}

		public object CreateFolderPayload(string folderPath)
		{
			var type = new { type = "SP.Folder" };
			var request = new { __metadata = type, ServerRelativeUrl = folderPath };

			return request;
		}

		private string BuildStringContent()
		{
			var bodyStringBuilder = new StringBuilder();

			bodyStringBuilder.Append("grant_type=client_credentials");
			bodyStringBuilder.Append($"&client_id={this.AppSettingService.ClientAppId}@{this.AppSettingService.Tenant}");
			bodyStringBuilder.Append($"&client_secret={this.AppSettingService.ClientAppSecret}");
			bodyStringBuilder.Append($"&resource={this.AppSettingService.SharePointAudiencePrincipal}/{this.AppSettingService.SharePointSiteDomain}@{this.AppSettingService.Tenant}");

			return bodyStringBuilder.ToString();
		}

	}
}
