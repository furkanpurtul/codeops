using System.ComponentModel.DataAnnotations;

using Npgsql;

namespace CodeOps.Infrastructure.EntityFrameworkCore.Npgsql
{
    public class NpgsqlOptions
    {
        public static readonly string SectionKey = "Npgsql";

        [Required]
        public string Host { get; set; } = string.Empty;

        [Required]
        public int Port { get; set; } = 5432;

        [Required]
        public string Database { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConnectionString
        {
            get
            {
                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = Host,
                    Port = Port,
                    Database = Database,
                    Username = Username,
                    Password = Password
                };

                return builder.ConnectionString;
            }
        }
    }
}
