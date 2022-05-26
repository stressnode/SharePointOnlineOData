using System;

namespace SPO.Contracts
{
	public class SPOObject
	{
		public bool Exists { get; set; }
		public int ItemCount { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public DateTime TimeCreated { get; set; }
		public Guid UniqueId { get; set; }
		public string ServerRelativeUrl { get; set; }
	}
}
