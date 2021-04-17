using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeFunctions.Models
{
	public class Collection
	{
		public string Id { get; set; } = Guid.NewGuid().ToString("n");

		public string CollectionName { get; set; }
	}
}
