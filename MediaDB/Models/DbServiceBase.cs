// using System;
// using System.Configuration;
// using System.Data.Common;

// namespace MediaDB.Models
// {
//     public abstract class DbServiceBase
//     {
//         private readonly string connectionString;
//         private readonly DbProviderFactory providerFactory;

//         protected DbServiceBase()
//         {
//             var settings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
//             if (settings == null)
//             {
//                 throw new ConfigurationErrorsException("DefaultConnection is not configured.");
//             }

//             if (string.IsNullOrWhiteSpace(settings.ProviderName))
//             {
//                 throw new ConfigurationErrorsException("DefaultConnection providerName is missing.");
//             }

//             connectionString = settings.ConnectionString;
//             providerFactory = DbProviderFactories.GetFactory(settings.ProviderName);
//         }

//         protected DbConnection OpenConnection()
//         {
//             var connection = providerFactory.CreateConnection();
//             if (connection == null)
//             {
//                 throw new InvalidOperationException("Failed to create a database connection.");
//             }

//             connection.ConnectionString = connectionString;
//             connection.Open();
//             return connection;
//         }

//         protected DbCommand CreateCommand(DbConnection connection, string commandText)
//         {
//             var command = connection.CreateCommand();
//             command.CommandText = commandText;
//             return command;
//         }

//         protected void AddParameter(DbCommand command, string name, object value)
//         {
//             var parameter = command.CreateParameter();
//             parameter.ParameterName = name;
//             parameter.Value = value ?? DBNull.Value;
//             command.Parameters.Add(parameter);
//         }
//     }
// }
