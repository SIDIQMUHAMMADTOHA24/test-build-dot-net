using Api.Share.Tools;
using Npgsql;
using Sodium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Repo
{
    public class Connection
    {
        private readonly string _connection_string;
        public Connection()
        {
            this._connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["EnkripaClient"].ConnectionString;
        }
        public NpgsqlConnection Connect()
        {
            var _public_key_certificate = Convert.FromBase64String(Const.public_key_certificate);
            var _private_key_certificate = Convert.FromBase64String(Const.private_key_certificate);
            KeyPair key_pair_certificate = new KeyPair(_public_key_certificate, _private_key_certificate);
            var pass_dekr = SealedPublicKeyBox.Open(Convert.FromBase64String(this._connection_string), key_pair_certificate);
            string con_string = Encoding.UTF8.GetString(pass_dekr);

            NpgsqlConnection connection = new NpgsqlConnection(con_string);
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
