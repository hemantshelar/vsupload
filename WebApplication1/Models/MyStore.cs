using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
	public class MyStore : IUserStore<MyUser>,IUserPasswordStore<MyUser>
	{
		public DbConnection GetConnection()
		{
			string connectionString = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=MyDb;Integrated Security=True";
			SqlConnection conn = new SqlConnection(connectionString);
			conn.Open();
			return conn;
		}
		public async Task<IdentityResult> CreateAsync(MyUser user, CancellationToken cancellationToken)
		{
			using (var conn = GetConnection())
			{
				string insertQuery = $"insert into MyUsers(id,username,NormalizedUserName,passwordhash) values ('{user.Id}','{user.UserName}','{user.NormalizedUserName}','{user.PasswordHash}')";
				await conn.ExecuteAsync(insertQuery);
			}
			return IdentityResult.Success;
		}

		public Task<IdentityResult> DeleteAsync(MyUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			Console.WriteLine("disposed called");
		}

		public async Task<MyUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			MyUser myUser = null;
			using (var conn = GetConnection())
			{
				var r = await conn.QueryFirstOrDefaultAsync<MyUser>($"SELECT * from MyUsers where Id = '{userId}'");
			}
			return myUser;
		}

		public async Task<MyUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			MyUser myUser = null;
			using (var conn = GetConnection())
			{
				var r = await conn.QueryFirstOrDefaultAsync<MyUser>($"SELECT * from MyUsers where NormalizedUserName = '{normalizedUserName}'");
			}
			return myUser;
		}

		public Task<string> GetNormalizedUserNameAsync(MyUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetUserIdAsync(MyUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Id);
		}

		public Task<string> GetUserNameAsync(MyUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.UserName);
		}

		public Task SetNormalizedUserNameAsync(MyUser user, string normalizedName, CancellationToken cancellationToken)
		{
			user.NormalizedUserName = normalizedName;
			return Task.CompletedTask;

		}


		public Task SetUserNameAsync(MyUser user, string userName, CancellationToken cancellationToken)
		{
			user.UserName = userName;
			return Task.CompletedTask;
		}

		public async Task<IdentityResult> UpdateAsync(MyUser user, CancellationToken cancellationToken)
		{
			using (var conn = GetConnection())
			{
				string insertQuery = $"UPDATE MyUsers set id = {user.Id}, UserName = {user.UserName}, " +
					$"NormalizedUserName = {user.NormalizedUserName},PasswordHash= {user.PasswordHash})";
				await conn.ExecuteAsync(insertQuery);
			}
			return IdentityResult.Success;
		}

		public Task SetPasswordHashAsync(MyUser user, string passwordHash, CancellationToken cancellationToken)
		{
			user.PasswordHash = passwordHash;
			return Task.CompletedTask;
		}

		public Task<string> GetPasswordHashAsync(MyUser user, CancellationToken cancellationToken)
		{
			var result = user.PasswordHash;
			return Task.FromResult(result);
		}

		public Task<bool> HasPasswordAsync(MyUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash != null);
		}
	}
}
