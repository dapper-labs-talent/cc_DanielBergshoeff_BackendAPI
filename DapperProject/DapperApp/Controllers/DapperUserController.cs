using DapperApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DapperApp.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class DapperUserController : Controller
	{
		private readonly IConfiguration _configuration;

		public DapperUser GetById(int id)
		{
			string query = @"
				SELECT
						UserId as ""UserId"",
						UserFirstName as ""UserFirstName"",
						UserLastName as ""UserLastName"",
						UserMail as ""UserMail""
				FROM DapperUser
				WHERE
						UserId = @UserId
			";

			DataTable table = new DataTable();
			string sqlDataSource = _configuration.GetConnectionString("DapperAppCon");
			NpgsqlDataReader myReader;
			using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
			{
				myCon.Open();
				using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
				{
					myCommand.Parameters.AddWithValue("@UserId", id);

					myReader = myCommand.ExecuteReader();
					table.Load(myReader);

					myReader.Close();
					myCon.Close();
				}
			}

			List<DapperUser> users = table.AsEnumerable().Select(row =>
				new DapperUser
				{
					UserId = row.Field<int>("UserId"),
					UserFirstName = row.Field<string>("UserFirstName"),
					UserLastName = row.Field<string>("UserLastName"),
					UserMail = row.Field<string>("UserMail"),
				}).ToList();

			//return null if user not found
			if (users.Count == 0)
				return null;

			return users[0];
		}

		public DapperUserController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost("login")]
		public JsonResult Authenticate(AuthenticateRequest model)
		{
			return new JsonResult(GetUserAndCreateToken(model).Token);
		}

		private DapperUser GetUserAndCreateToken(AuthenticateRequest model)
		{
			//Authenticate user by checking username and password
			string query = @"
				SELECT
						UserId as ""UserId"",
						UserFirstName as ""UserFirstName"",
						UserLastName as ""UserLastName"",
						UserMail as ""UserMail""
				FROM DapperUser
				WHERE
						UserMail = @UserMail AND
						UserPassword = @UserPassword
			";

			DataTable table = new DataTable();
			string sqlDataSource = _configuration.GetConnectionString("DapperAppCon");
			NpgsqlDataReader myReader;
			using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
			{
				myCon.Open();
				using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
				{
					myCommand.Parameters.AddWithValue("@UserMail", model.UserMail);
					myCommand.Parameters.AddWithValue("@UserPassword", model.UserPassword);

					myReader = myCommand.ExecuteReader();
					table.Load(myReader);

					myReader.Close();
					myCon.Close();
				}
			}

			List<DapperUser> users = table.AsEnumerable().Select(row =>
				new DapperUser
				{
					UserId = row.Field<int>("UserId"),
					UserFirstName = row.Field<string>("UserFirstName"),
					UserLastName = row.Field<string>("UserLastName"),
					UserMail = row.Field<string>("UserMail"),
				}).ToList();

			//return null if user not found
			if (users.Count == 0)
				return null;

			var token = generateJwtToken(users[0]);
			users[0].Token = token;

			return users[0];
		}

		[Helpers.Authorize]
		[HttpGet("users")]
		public JsonResult Get()
		{
			string query = @"
				SELECT
						UserFirstName as ""UserFirstName"",
						UserLastName as ""UserLastName"",
						UserMail as ""UserMail""
				FROM DapperUser
			";

			DataTable table = new DataTable();
			string sqlDataSource = _configuration.GetConnectionString("DapperAppCon");
			NpgsqlDataReader myReader;
			using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
			{
				myCon.Open();
				using(NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
				{
					myReader = myCommand.ExecuteReader();
					table.Load(myReader);

					myReader.Close();
					myCon.Close();
				}
			}

			return new JsonResult(table);
		}

		[HttpPost("signup")]
		public JsonResult Post(DapperUser user)
		{
			string query = @"
				INSERT INTO DapperUser(UserFirstName, UserLastName, UserMail, UserPassword) 
				VALUES (@UserFirstName, @UserLastName, @UserMail, @UserPassword);
			";

			DataTable table = new DataTable();
			string sqlDataSource = _configuration.GetConnectionString("DapperAppCon");
			NpgsqlDataReader myReader;
			using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
			{
				myCon.Open();
				using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
				{
					myCommand.Parameters.AddWithValue("@UserFirstName", user.UserFirstName);
					myCommand.Parameters.AddWithValue("@UserLastName", user.UserLastName);
					myCommand.Parameters.AddWithValue("@UserMail", user.UserMail);
					myCommand.Parameters.AddWithValue("@UserPassword", user.UserPassword);
					myReader = myCommand.ExecuteReader();
					table.Load(myReader);

					myReader.Close();
					myCon.Close();
				}
			}

			DapperUser du = GetUserAndCreateToken(new AuthenticateRequest(user.UserMail, user.UserPassword));

			return new JsonResult(du.Token);
		}

		[Helpers.Authorize]
		[HttpPut("users")]
		public JsonResult Put(FullName name)
		{
			DapperUser user = (DapperUser)HttpContext.Items["DapperUser"];
			string query = @"
				UPDATE DapperUser
				SET UserFirstName = @UserFirstName, UserLastName = @UserLastName
				WHERE UserId = @UserId
			";

			DataTable table = new DataTable();
			string sqlDataSource = _configuration.GetConnectionString("DapperAppCon");
			NpgsqlDataReader myReader;
			using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
			{
				myCon.Open();
				using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
				{
					myCommand.Parameters.AddWithValue("@UserFirstName", name.UserFirstName);
					myCommand.Parameters.AddWithValue("@UserLastName", name.UserLastName);
					myCommand.Parameters.AddWithValue("@UserId", user.UserId);
					myReader = myCommand.ExecuteReader();
					table.Load(myReader);

					myReader.Close();
					myCon.Close();
				}
			}

			return new JsonResult("Updated succesfully");
		}

		[Helpers.Authorize]
		[HttpDelete("{id}")]
		public JsonResult Delete(int id)
		{
			string query = @"
				DELETE FROM DapperUser
				WHERE UserId = @UserId
			";

			DataTable table = new DataTable();
			string sqlDataSource = _configuration.GetConnectionString("DapperAppCon");
			NpgsqlDataReader myReader;
			using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
			{
				myCon.Open();
				using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
				{
					myCommand.Parameters.AddWithValue("@UserId", id);
					myReader = myCommand.ExecuteReader();
					table.Load(myReader);

					myReader.Close();
					myCon.Close();
				}
			}

			return new JsonResult("Deleted succesfully");
		}

		public IActionResult Index()
		{
			return View();
		}

		private string generateJwtToken(DapperUser user)
		{
			// generate token that is valid for 7 days
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration.GetConnectionString("Secret"));
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()) }),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
