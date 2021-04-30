using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeFunctions.Models
{
	public class Recipe
	{
		public string Id { get; set; } = Guid.NewGuid().ToString("n");
		public string CreatorUserId { get; set; }
		public bool IsPoolRecipe { get; set; }
		public string RecipeName { get; set; }
		public string RecipeNationality { get; set; }
		public List<Ingredient> Ingredients { get; set; }
		public List<Step> Steps { get; set; }
		public int PrepTimeMinutes { get; set; }
		public int CookTimeMinutes { get; set; }
		public int ReadyInMinutes { get; set; }
		public string Creator { get; set; }
		public string Notes { get; set; }

		//TODO Pictures
	}
}
