using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperApp.Models
{
	public class DapperUser
	{
		public int UserId { get; set; }

		public string UserFirstName { get; set; }
		public string UserLastName { get; set; }
		public string UserMail { get; set; }
		public string UserPassword { get; set; }

		public string Token { get; set; }
	}
}
