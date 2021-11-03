using System.ComponentModel.DataAnnotations;

namespace RecipeFunctions.Models
{
	public class AuthenticateModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }
	}
}