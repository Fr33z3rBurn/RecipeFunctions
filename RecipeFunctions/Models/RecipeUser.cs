namespace RecipeFunctions.Models
{
	public class RecipeUser
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
	}
}