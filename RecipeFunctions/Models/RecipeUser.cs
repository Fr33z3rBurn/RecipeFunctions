namespace RecipeFunctions.Models
{
	public class RecipeUser
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public bool IsEmailVerified { get; set; }
		public bool IsAdmin { get; set; }
		public byte[] PasswordHash { get; set; }
		public byte[] PasswordSalt { get; set; }
	}
}