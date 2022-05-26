using SPO.Contracts;
using System.Collections.Generic;

namespace SPO.Services
{
	public interface ISharePointOperationService
	{
		void ReadFile(string relativeFilePath, string fileDestinationPath);
		void CreateFile(string relativeFileDestinaitonPath, string fileSourcePath);
		void DeleteFile(string relativeFilePath);
		void CreateFolderStructure(string folderStructurePath);
		void CreateFolder(string relativePath, string newFolderName);
		void DeleteFolder(string relativePath);
		bool IsFolderExist(string relativePath);
		List<SPOObject> GetFolders(string relativePath);
		List<SPOObject> GetFolderContents(string relativePath);
		void GetAccessToken();
	}
}
