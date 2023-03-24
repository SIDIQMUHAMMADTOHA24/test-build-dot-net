using Api.Share.Tools;
using Api.Share.User;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Api.Share.Tools.Result;

namespace App.WinServiceCron.Db
{
    public class UserCron : IDisposable
    {
        private readonly ConnectionCron _con;
        private readonly Kripto kripto;
        public UserCron()
        {
            this._con = new ConnectionCron();
            this.kripto = new Kripto();
        }

        public void Dispose()
        {
        }

        public GetResultObject<List<UserInfo>> GetUserForChangePassword()
        {
            GetResultObject<List<UserInfo>> user_login_info = new GetResultObject<List<UserInfo>>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"select b.email_address from UserLoginInfo a left join UserInfo b on a.id_user_info = b.id where (DATE_PART('year', now()::date) - DATE_PART('year', a.change_password_date::date)) * 12 + (DATE_PART('month', now()::date) - DATE_PART('month', a.change_password_date::date)) >= 3 and a.is_active = true and a.login_type NOT IN ({(int)LoginType.admin}, {(int)LoginType.superadmin}, {(int)LoginType.statis})");
                    _con.command = new NpgsqlCommand(query, a);
                    _con.reader = _con.command.ExecuteReader();

                    List<UserInfo> models = new List<UserInfo>();

                    while (_con.reader.Read())
                    {
                        models.Add(new UserInfo() { email_address = Convert.ToString(_con.reader["email_address"]) });
                    }
                    _con.reader.Close();
                    user_login_info.objek = models;
                    user_login_info.result = true;
                    user_login_info.message = "success get";
                    return user_login_info;
                }
            }
            catch (Exception ex)
            {
                user_login_info.result = false;
                user_login_info.message = ex.Message.ToString();
                return user_login_info;
            }
        }

        public PostResult UpdateUserLog()
        {
            PostResult pr = new PostResult();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"update UserLog set status_log = 'Reviewed' where now()::date - time_input::date >= 2 and status_log != 'Reviewed'");
                    _con.command = new NpgsqlCommand(query, a);
                    _con.command.ExecuteNonQuery();

                    pr.result = true;
                    pr.message = "Success update user log Ok";
                    return pr;
                }
            }
            catch (Exception ex)
            {
                pr.result = false;
                pr.message = ex.Message.ToString();
                return pr;
            }
        }

        public GetResultObject<List<UserInfo>> GetLogTimeEnroolCertificate(double time)
        {
            GetResultObject<List<UserInfo>> user_login_info = new GetResultObject<List<UserInfo>>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"select a.user_name from UserLog a where (DATE_PART('year', now()::date) - DATE_PART('year', a.time_input::date)) * 12 + (DATE_PART('month', now()::date) - DATE_PART('month', a.time_input::date)) = @time + 12 and a.activity = 'Enrool Certificate'");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("time", time);
                    _con.reader = cmd.ExecuteReader();

                    List<UserInfo> models = new List<UserInfo>();

                    while (_con.reader.Read())
                    {
                        models.Add(new UserInfo() { email_address = kripto.Dekrip(Convert.ToString(_con.reader["user_name"])) });
                    }
                    _con.reader.Close();
                    user_login_info.objek = models;
                    user_login_info.result = true;
                    user_login_info.message = "success get";
                    return user_login_info;
                }
            }
            catch (Exception ex)
            {
                user_login_info.result = false;
                user_login_info.message = ex.Message.ToString();
                return user_login_info;
            }
        }
    }
}
