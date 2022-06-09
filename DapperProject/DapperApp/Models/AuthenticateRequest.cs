using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DapperApp.Models
{
	public class AuthenticateRequest
	{
		[Required]
		public string UserMail { get; set; }

		[Required]
		public string UserPassword { get; set; }

		public AuthenticateRequest(string userMail, string userPassword)
		{
			UserMail = userMail;
			UserPassword = userPassword;
		}
	}
}
