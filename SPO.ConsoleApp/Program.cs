using System;
using Microsoft.Extensions.DependencyInjection;
using SPO.Services;

namespace SPO.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			#region ** Dependency Injection **

			var serviceProvider = new ServiceCollection()
			.AddSingleton<IAppSettingService, AppSettingService>()
			.AddScoped<ICacheService, CacheService>()
			.AddScoped<ISharePointRequestService, SharePointRequestService>()
			.AddScoped<ISharePointOperationService, SharePointOperationService>()
			.BuildServiceProvider();

			var operations = serviceProvider.GetService<ISharePointOperationService>();

			#endregion

			operations.GetAccessToken();
			var items = operations.GetFolderContents("path/to/getFiles");

			foreach(var item in items)
			{
				Console.WriteLine($"File: {item.Name} \nTitle: {item.Title} \nLocated: {item.ServerRelativeUrl}");
				Console.WriteLine($"---------------------------------------------------------------------------");
				Console.WriteLine();
			}

			items = operations.GetFolders("path/to/getFolders");

			foreach (var item in items)
			{
				Console.WriteLine($"Object: {item.Name} \t\t | Item count: {item.ItemCount} \t\t | Located: {item.ServerRelativeUrl}");
			}

			Console.ReadLine();
		}
	}
}
