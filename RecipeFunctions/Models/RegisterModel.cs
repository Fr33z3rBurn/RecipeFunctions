using System.ComponentModel.DataAnnotations;

namespace RecipeFunctions.Models
{
	public class RegisterModel
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		//email

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }
	}
}