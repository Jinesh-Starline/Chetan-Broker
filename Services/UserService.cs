using Chetan_Broker.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chetan_Broker.Services
{
    public class UserService
    {
        public bool Validate(string username, string password)
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            var user = conn.QueryFirstOrDefault<string>(
                "SELECT PasswordHash FROM User WHERE Username = @Username",
                new { Username = username });

            if (user == null)
                return false;

            return SecurityHelper.VerifyPassword(password, user);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            var current = conn.QueryFirstOrDefault<string>(
                "SELECT PasswordHash FROM User WHERE Username = @Username",
                new { Username = username });

            if (current == null || !SecurityHelper.VerifyPassword(oldPassword,current))
                return false;

            var passwordHash = SecurityHelper.HashPassword(newPassword);

            conn.Execute(
                "UPDATE User SET PasswordHash = @Password WHERE Username = @Username",
                new { Password = passwordHash, Username = username });

            return true;
        }

        public void SetAutoLogin(string username, bool isAuto)
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            conn.Execute(
                "UPDATE User SET AutoLogin = @AutoLogin WHERE Username = @Username",
                new
                {
                    AutoLogin = isAuto ? 1 : 0,
                    Username = username
                });
        }

        public string GetAutoLoginUser()
        {
            using var conn = DbHelper.GetConnection();
            conn.Open();

            return conn.QueryFirstOrDefault<string>(
                "SELECT Username FROM User WHERE AutoLogin = 1 LIMIT 1");
        }
    }
}
