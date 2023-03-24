using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WinServiceCron.Db
{
    public class ConnectionCron
    {
        private readonly string _connection_string;
        private readonly string _connection_string_sign;
        public ConnectionCron()
        {
            this._connection_string = "server=localhost;user id=postgres;password=@enkripa123!@#;database=EnkripaClient;Port=5432";
            this._connection_string_sign = "server=localhost;user id=postgres;password=@enkripa123!@#;database=EnkripaSign;Port=5432";
        }
        public NpgsqlConnection Connect()
        {
            NpgsqlConnection connection = new NpgsqlConnection(this._connection_string);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        public NpgsqlConnection ConnectSign()
        {
            NpgsqlConnection connection = new NpgsqlConnection(this._connection_string_sign);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        public NpgsqlCommand command;
        public NpgsqlDataReader reader;
    }
}
