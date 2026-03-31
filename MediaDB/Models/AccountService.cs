using System;
using System.Data.SqlClient;
using System.Configuration;
using MediaDB.Models;

namespace MediaDB.Models
{
    //20260329 mod start - supabase연동 검토 관련
    // public class AccountService : DbServiceBase
    // {
    //     public bool ValidateUser(string username, string password)
    //     {
    //         using (var conn = OpenConnection())
    //         using (var cmd = CreateCommand(conn, "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password"))
    //         {
    //             AddParameter(cmd, "@username", username);
    //             AddParameter(cmd, "@password", password);
    //             var result = cmd.ExecuteScalar();
    //             int count = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
    //             return count == 1;
    //         }
    //     }

    //     public (bool Success, string Message) RegisterUser(string username, string password, string confirmPassword)
    //     {
    //         if (string.IsNullOrWhiteSpace(username))
    //             return (false, "Username이 미입력 상태입니다.");
    //         if (string.IsNullOrWhiteSpace(password))
    //             return (false, "Password가 미입력 상태입니다.");
    //         if (string.IsNullOrWhiteSpace(confirmPassword))
    //             return (false, "Password 확인이 미입력 상태입니다.");
    //         if (password != confirmPassword)
    //             return (false, "Password와 Password 확인의 입력값이 일치하지 않습니다.");

    //         using (var conn = OpenConnection())
    //         using (var cmd = CreateCommand(conn, "INSERT INTO users (username, password) VALUES (@username, @password)"))
    //         {
    //             AddParameter(cmd, "@username", username);
    //             AddParameter(cmd, "@password", password); // 실제로는 암호화를 적용해야 함
    //             cmd.ExecuteNonQuery();
    //         }

    //         return (true, "회원가입을 완료하였습니다.");
    //     }
    // }

        public class AccountService
    {
        private readonly string connectionString;

        public AccountService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool ValidateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM [users] WHERE user_id=@user_id AND user_password=@user_password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", username);
                cmd.Parameters.AddWithValue("@user_password", password);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count == 1;
            }
        }

        public (bool Success, string Message) RegisterUser(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
                return (false, "Username이 미입력 상태입니다.");
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password가 미입력 상태입니다.");
            if (string.IsNullOrWhiteSpace(confirmPassword))
                return (false, "Password 확인이 미입력 상태입니다.");
            if (password != confirmPassword)
                return (false, "Password와 Password 확인의 입력값이 일치하지 않습니다.");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [users] (user_id, user_password) VALUES (@user_id, @user_password)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user_id", username);
                cmd.Parameters.AddWithValue("@user_password", password); // 실제로는 암호화를 적용해야 함
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return (true, "회원가입을 완료하였습니다.");
        }
    }
    //20260329 mod end
}
