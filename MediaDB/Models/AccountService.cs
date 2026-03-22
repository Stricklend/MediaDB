using System;
using System.Data.SqlClient;
using System.Configuration;
using MediaDB.Models;

namespace MediaDB.Models
{
    public class AccountService
    {
        private readonly string connectionString;

        public AccountService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

//CREATE TABLE UserMaster(
//UserId NVARCHAR(50) NOT NULL PRIMARY KEY,       -- 회원 아이디(기본키)
//Password VARBINARY(64) NVARCHAR(128) NOT NULL,  -- 암호화된 비밀번호(예: HASHBYTES로 저장)
//Nickname NVARCHAR(50) NOT NULL,                 -- 닉네임
//CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),  -- 등록일시(생성 시 자동 입력)
//UpdatedAt DATETIME NULL                         -- 갱신일시(수정 시 갱신)
//);

        public bool ValidateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM [User] WHERE Username=@Username AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);
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
                string query = "INSERT INTO [User] (Username, Password) VALUES (@Username, @Password)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password); // 실제로는 암호화를 적용해야 함
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return (true, "회원가입을 완료하였습니다.");
        }
    }
}
