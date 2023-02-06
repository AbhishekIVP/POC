using com.ivp.rad.common;
using Snowflake.Data.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowflakeAdapter
{
    struct SFConnectionMetadata
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string AccountName { get; private set; }
        public string DatabaseName { get; private set; }
        public string WarehouseName { get; private set; }
        public string RoleName { get; private set; }
        public string PrivateKey { get; private set; }
        public string PrivateKeyFile { get; private set; }
        public string PrivateKeyPassword { get; private set; }

        internal SFConnectionMetadata(string userName, string password, string accountName, string databaseName, string warehouseName, string roleName, string privateKey, string privateKeyFile, string privateKeyPassword)
        {
            StringBuilder errorMessages = new StringBuilder();

            if (string.IsNullOrEmpty(userName))
                errorMessages.Append("User Name cannot be blank.").Append(Environment.NewLine);
            if (string.IsNullOrEmpty(accountName))
                errorMessages.Append("Account Name cannot be blank.").Append(Environment.NewLine);
            if (string.IsNullOrEmpty(databaseName))
                errorMessages.Append("Database Name cannot be blank.").Append(Environment.NewLine);
            if (string.IsNullOrEmpty(warehouseName))
                errorMessages.Append("Warehouse Name cannot be blank.").Append(Environment.NewLine);

            if (errorMessages.Length > 0)
                throw new TypeInitializationException((typeof(SFConnectionMetadata)).FullName, new ArgumentException(errorMessages.ToString()));

            UserName = userName;
            Password = password;
            AccountName = accountName;
            DatabaseName = databaseName;
            WarehouseName = warehouseName;
            RoleName = roleName;
            PrivateKey = privateKey;
            PrivateKeyFile = privateKeyFile;
            PrivateKeyPassword = privateKeyPassword;
        }
    }

    internal static class SFDriver
    {
        internal static readonly SFConnectionMetadata ConnectionMetadata;
        internal static readonly string ConnectionString;
        internal static readonly string ConnectionStringWithSchema;

        static SFDriver()
        {
            string userName = ConfigurationManager.AppSettings["SnowflakeUserName"];
            string password = ConfigurationManager.AppSettings["SnowflakePassword"];
            string accountName = ConfigurationManager.AppSettings["SnowflakeAccountName"];
            string databaseName = ConfigurationManager.AppSettings["SnowflakeDatabaseName"];
            string warehouseName = ConfigurationManager.AppSettings["SnowflakeWarehouseName"];
            string roleName = ConfigurationManager.AppSettings["SnowflakeRoleName"];
            string privateKey = ConfigurationManager.AppSettings["SnowflakePrivateKey"];
            string privateKeyFile = ConfigurationManager.AppSettings["SnowflakePrivateKeyFile"];
            string privateKeyPassword = ConfigurationManager.AppSettings["SnowflakePrivateKeyPassword"];

            ConnectionMetadata = new SFConnectionMetadata(userName, password, accountName, databaseName, warehouseName, roleName, privateKey, privateKeyFile, privateKeyPassword);
            string passwordConnection = string.Empty;
            if (!string.IsNullOrWhiteSpace(ConnectionMetadata.Password))
                passwordConnection = $"password={ConnectionMetadata.Password};";

            string roleNameConnection = string.Empty;
            if (!string.IsNullOrWhiteSpace(ConnectionMetadata.RoleName))
                roleNameConnection = $"ROLE={ConnectionMetadata.RoleName};";

            string privateKeyConnection = string.Empty;
            if (!string.IsNullOrWhiteSpace(ConnectionMetadata.PrivateKey))
                privateKeyConnection = $"PRIVATE_KEY={ConnectionMetadata.PrivateKey};";

            string privateKeyFileConnection = string.Empty;
            if (!string.IsNullOrWhiteSpace(ConnectionMetadata.PrivateKeyFile))
                privateKeyFileConnection = $"PRIVATE_KEY_FILE={ConnectionMetadata.PrivateKeyFile};";

            string privateKeyPasswordConnection = string.Empty;
            if (!string.IsNullOrWhiteSpace(ConnectionMetadata.PrivateKeyPassword))
                privateKeyPasswordConnection = $"PRIVATE_KEY_PWD={ConnectionMetadata.PrivateKeyPassword};";

            string authenticatorConnection = string.Empty;
            if (!string.IsNullOrWhiteSpace(privateKeyConnection) || !string.IsNullOrWhiteSpace(privateKeyFileConnection))
                authenticatorConnection = "AUTHENTICATOR=snowflake_jwt;";

            ConnectionString = $"account={ConnectionMetadata.AccountName};user={ConnectionMetadata.UserName};{passwordConnection}DB={ConnectionMetadata.DatabaseName};WAREHOUSE={ConnectionMetadata.WarehouseName};{roleNameConnection}{authenticatorConnection}{privateKeyConnection}{privateKeyFileConnection}{privateKeyPasswordConnection}";
            ConnectionStringWithSchema = $"account={ConnectionMetadata.AccountName};user={ConnectionMetadata.UserName};{passwordConnection}DB={ConnectionMetadata.DatabaseName};WAREHOUSE={ConnectionMetadata.WarehouseName};SCHEMA=\"{SFRepository.SCHEMA_NAME}\";{roleNameConnection}{authenticatorConnection}{privateKeyConnection}{privateKeyFileConnection}{privateKeyPasswordConnection}";
        }
    }

    internal class SFConnection : IDisposable
    {
        IDbConnection conn;
        bool hasOpenTransaction;
        IDbTransaction transaction;
        static IRLogger logger = RLogFactory.CreateLogger("SFConnection");

        public SFConnection(bool requireTransaction)
        {
            logger.Error("SFConnection -> CTOR -> START");
            try
            {
                conn = new SnowflakeDbConnection();
                conn.ConnectionString = SFDriver.ConnectionStringWithSchema;

                conn.Open();

                if (requireTransaction)
                {
                    transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    hasOpenTransaction = true;
                }
            }
            finally
            {
                logger.Error("SFConnection -> CTOR -> END");
            }
        }

        public SFConnection()
        {
            logger.Error("SFConnection -> CTOR -> START");
            try
            {
                conn = new SnowflakeDbConnection();
                conn.ConnectionString = SFDriver.ConnectionString;
                conn.Open();
            }
            finally
            {
                logger.Error("SFConnection -> CTOR -> END");
            }
        }

        public void Dispose()
        {
            logger.Error("SFConnection -> Dispose -> START");
            try
            {
                conn.Close();
                conn.Dispose();
            }
            finally
            {
                logger.Error("SFConnection -> Dispose -> END");
            }
        }

        public void Commit()
        {
            logger.Error("SFConnection -> Commit -> START");
            try
            {
                if (hasOpenTransaction)
                    transaction.Commit();
            }
            finally
            {
                logger.Error("SFConnection -> Commit -> END");
            }
        }

        public void Rollback()
        {
            logger.Error("SFConnection -> Commit -> START");
            try
            {
                if (hasOpenTransaction)
                    transaction.Rollback();
            }
            finally
            {
                logger.Error("SFConnection -> Commit -> END");
            }
        }

        public void ExecuteNonQuery(string[] scripts)
        {
            try
            {
                logger.Debug("SFConnection -> ExecuteNonQuery -> START");
                foreach (var script in scripts)
                {
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = script;
                        cmd.CommandTimeout = 0;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                logger.Debug("SFConnection -> ExecuteNonQuery -> END");
            }
        }
    }
}
