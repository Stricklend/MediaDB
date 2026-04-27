using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Web.Helpers;
using Npgsql;
using NpgsqlTypes;

namespace MediaDB.Models
{
    public class AccountService
    {
        private const int UserIdMaxLength = 50;
        private const int PasswordMaxLength = 255;
        private const int UsernameMaxLength = 50;
        private const int EmailMaxLength = 100;

        private readonly string connectionString;

        public AccountService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public User AuthenticateUser(string user_id, string user_password)
        {
            var normalizedUserId = NormalizeText(user_id);

            if (string.IsNullOrWhiteSpace(normalizedUserId) || string.IsNullOrEmpty(user_password))
            {
                return null;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                const string query = @"
SELECT id, username, password, nickname, email, created_at, updated_at
FROM users
WHERE username = @username
ORDER BY id DESC";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    AddVarCharParameter(cmd, "@username", UserIdMaxLength, normalizedUserId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = MapUser(reader);

                            if (!string.IsNullOrEmpty(user.UserPassword) && Crypto.VerifyHashedPassword(user.UserPassword, user_password))
                            {
                                return user;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public (bool Success, string Message) RegisterUser(string user_id, string user_password, string confirm_user_password, string username, string email)
        {
            var normalizedUserId = NormalizeText(user_id);
            var normalizedUsername = NormalizeText(username);
            var normalizedEmail = NormalizeText(email);

            if (string.IsNullOrWhiteSpace(normalizedUserId))
                return (false, "ID is required.");
            if (string.IsNullOrWhiteSpace(user_password))
                return (false, "Password is required.");
            if (string.IsNullOrWhiteSpace(confirm_user_password))
                return (false, "Please confirm your password.");
            if (string.IsNullOrWhiteSpace(normalizedUsername))
                return (false, "Username is required.");
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return (false, "Email is required.");
            if (user_password != confirm_user_password)
                return (false, "Password and confirmation do not match.");
            if (normalizedUserId.Length > UserIdMaxLength)
                return (false, "ID must be 50 characters or less.");
            if (user_password.Length > PasswordMaxLength)
                return (false, "Password must be 255 characters or less.");
            if (normalizedUsername.Length > UsernameMaxLength)
                return (false, "Username must be 50 characters or less.");
            if (normalizedEmail.Length > EmailMaxLength)
                return (false, "Email must be 100 characters or less.");
            if (!new EmailAddressAttribute().IsValid(normalizedEmail))
                return (false, "Please enter a valid email address.");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        if (UserIdExists(conn, transaction, normalizedUserId))
                        {
                            transaction.Rollback();
                            return (false, "This ID is already in use.");
                        }

                        if (EmailExists(conn, transaction, normalizedEmail))
                        {
                            transaction.Rollback();
                            return (false, "This email address is already in use.");
                        }

                        const string query = @"
INSERT INTO users (username, password, nickname, email)
VALUES (@username, @password, @nickname, @email)";

                        using (var cmd = new NpgsqlCommand(query, conn, transaction))
                        {
                            AddVarCharParameter(cmd, "@username", UserIdMaxLength, normalizedUserId);
                            AddVarCharParameter(cmd, "@password", PasswordMaxLength, Crypto.HashPassword(user_password));
                            AddVarCharParameter(cmd, "@nickname", UsernameMaxLength, normalizedUsername);
                            AddVarCharParameter(cmd, "@email", EmailMaxLength, normalizedEmail);

                            var affectedRows = cmd.ExecuteNonQuery();

                            if (affectedRows < 1)
                            {
                                transaction.Rollback();
                                return (false, "Registration could not be completed.");
                            }
                        }

                        transaction.Commit();
                        return (true, "Registration completed successfully.");
                    }
                    catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        transaction.Rollback();

                        if (string.Equals(ex.ConstraintName, "users_email_key", StringComparison.OrdinalIgnoreCase))
                        {
                            return (false, "This email address is already in use.");
                        }

                        return (false, "A database constraint was violated while creating the account.");
                    }
                    catch (NpgsqlException)
                    {
                        transaction.Rollback();
                        return (false, "A database error occurred while creating the account.");
                    }
                }
            }
        }

        private static void AddVarCharParameter(NpgsqlCommand command, string parameterName, int length, string value)
        {
            var parameter = command.Parameters.Add(parameterName, NpgsqlDbType.Varchar, length);
            parameter.Value = (object)value ?? DBNull.Value;
        }

        private static string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool UserIdExists(NpgsqlConnection connection, NpgsqlTransaction transaction, string userId)
        {
            const string query = "SELECT COUNT(1) FROM users WHERE username = @username";

            using (var cmd = new NpgsqlCommand(query, connection, transaction))
            {
                AddVarCharParameter(cmd, "@username", UserIdMaxLength, userId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static bool EmailExists(NpgsqlConnection connection, NpgsqlTransaction transaction, string email)
        {
            const string query = "SELECT COUNT(1) FROM users WHERE email = @email";

            using (var cmd = new NpgsqlCommand(query, connection, transaction))
            {
                AddVarCharParameter(cmd, "@email", EmailMaxLength, email);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static User MapUser(IDataRecord record)
        {
            return new User
            {
                Id = record.GetInt32(record.GetOrdinal("id")),
                UserId = GetNullableString(record, "username"),
                UserPassword = GetNullableString(record, "password"),
                Username = GetNullableString(record, "nickname"),
                Email = GetNullableString(record, "email"),
                CreatedAt = GetNullableDateTime(record, "created_at"),
                UpdatedAt = GetNullableDateTime(record, "updated_at")
            };
        }

        private static string GetNullableString(IDataRecord record, string columnName)
        {
            var ordinal = record.GetOrdinal(columnName);
            return record.IsDBNull(ordinal) ? null : record.GetString(ordinal);
        }

        private static DateTime? GetNullableDateTime(IDataRecord record, string columnName)
        {
            var ordinal = record.GetOrdinal(columnName);
            return record.IsDBNull(ordinal) ? (DateTime?)null : record.GetDateTime(ordinal);
        }
    }
}
