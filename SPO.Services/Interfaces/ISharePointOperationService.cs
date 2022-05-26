using SPO.Contracts;
using System.Collections.Generic;

namespace SPO.Services
{
	public interface ISharePointOperationService
	{
		void ReadFile(string relativeFilePath, string fileDestinationPath);
		void CreateFile(string relativePath, string fileSourcePath);
		void CreateFolderStructure(string folderStructurePath);
		void CreateFolder(string relativePath, string newFolderName);
		bool IsFolderExist(string relativePath);
		List<SPOObject> GetFolders(string relativePath);
		List<SPOObject> GetFolderContents(string relativePath);
		void GetAccessToken();
	}
}
