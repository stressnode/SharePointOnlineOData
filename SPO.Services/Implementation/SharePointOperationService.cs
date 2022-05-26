using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using SPO.Contracts;

namespace SPO.Services
{
	public class SharePointOperationService : ISharePointOperationService
	{
		private readonly ISharePointRequestService SharePointRequestService;
		private readonly IAppSettingService AppSettingService;

		public SharePointOperationService(ISharePointRequestService sharePointRequestService, IAppSettingService appSettingService)
		{
			this.SharePointRequestService = sharePointRequestService;
			this.AppSettingService = appSettingService;
		}

		public void CreateFile(string relativePath, string fileSourcePath)
		{
			var file = new FileInfo(fileSourcePath);
			byte[] fileBytes = File.ReadAllBytes(fileSourcePath);

			this.SharePointRequestService.RequestSharePointOnlineFileCreate
				(
					this.AppSettingService.SharePointSiteCollection,
					$"GetFolderByServerRelativeUrl('{this.AppSettingService.SharePointSiteCollection}/{relativePath}')/Files/add(url='{file.Name}',overwrite=true)"
					, fileBytes
				);
		}

		public void ReadFile(string relativeFilePath, string fileDestinationPath)
		{
			var response = this.SharePointRequestService.RequestSharePointOnlineFileRead
								(
									this.AppSettingService.SharePointSiteCollection,
									$"GetFileByServerRelativeUrl('{this.AppSettingService.SharePointSiteCollection}/{relativeFilePath}')/$value"
								);

			if (response.ContentLength > 0)
			{
				using (var outputFileStream = new FileStream(fileDestinationPath, FileMode.Create))
				{
					response.GetResponseStream().CopyTo(outputFileStream);
				}
			}
		}

		public void CreateFolder(string relativePath, string newFolderName)
		{
			var payload = JsonConvert.SerializeObject(this.SharePointRequestService.CreateFolderPayload(newFolderName));
			this.SharePointRequestService.RequestSharePointOnline
				(
					HttpMethod.Post,
					this.AppSettingService.SharePointSiteCollection,
					$"GetFolderByServerRelativeUrl('{this.AppSettingService.SharePointSiteCollection}/{relativePath}')/folders"
					, payload
				);
		}

		public bool IsFolderExist(string relativePath)
		{
			var response = this.SharePointRequestService.RequestSharePointOnline
							(
								HttpMethod.Get,
								this.AppSettingService.SharePointSiteCollection,
								$"GetFolderByServerRelativeUrl('{this.AppSettingService.SharePointSiteCollection}/{relativePath}')"
							);

			if (response == null) return false;

			var content = this.SharePointRequestService.ResponseToObject<SPOObject>(response.GetResponseStream(), "");

			return content.Exists;
		}

		public void CreateFolderStructure(string folderStructurePath)
		{
			var folderStructurePathSegments = folderStructurePath.Split("/");
			var segmentsList = new List<string>();

			foreach (var pathSegment in folderStructurePathSegments)
			{
				var parent = string.Join("/", segmentsList.ToArray());

				this.CreateFolder(parent, pathSegment);

				segmentsList.Add(pathSegment);
			}
		}

		public List<SPOObject> GetFolderContents(string relativePath)
		{
			var response = this.SharePointRequestService.RequestSharePointOnline
					(
						HttpMethod.Get,
						this.AppSettingService.SharePointSiteCollection,
						$"GetFolderByServerRelativeUrl('{this.AppSettingService.SharePointSiteCollection}/{relativePath}')/files?$select=Name,Exists,Title,TimeCreated,ServerRelativeUrl,ItemCount,UniqueId"
					);

			return this.SharePointRequestService.ResponseToListofObject<SPOObject>(response.GetResponseStream(), "results");

		}

		public List<SPOObject> GetFolders(string relativePath)
		{
			var response = this.SharePointRequestService.RequestSharePointOnline
					(
						HttpMethod.Get,
						this.AppSettingService.SharePointSiteCollection,
						$"GetFolderByServerRelativeUrl('{this.AppSettingService.SharePointSiteCollection}/{relativePath}')/folders?$select=Name,Exists,Title,TimeCreated,ServerRelativeUrl,ItemCount,UniqueId"
					);

			return this.SharePointRequestService.ResponseToListofObject<SPOObject>(response.GetResponseStream(), "results");
		}

		public void GetAccessToken()
		{
			this.SharePointRequestService.GetSPOAccessToken();
		}
	}
}
