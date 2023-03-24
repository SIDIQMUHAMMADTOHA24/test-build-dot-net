using Api.Share;
using Api.Share.Payment;
using Api.Share.Tools;
using Api.Share.User;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Api.Share.Tools.Result;

namespace Api.Repo
{
    public class UserDb : IDisposable
    {
        private readonly Connection _con;
        private readonly Kripto kripto;
        public UserDb()
        {
            this._con = new Connection();
            this.kripto = new Kripto();
        }

        public void Dispose()
        {
        }

        public PostResult Post(User model)
        {
            PostResult pr = new PostResult();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        if (IsExistUserLoginInfo(model.user_login_info.user_name))
                        {
                            pr.result = false;
                            pr.message = "User name is exist";
                        }
                        else if (IsExistNik(model.user_info.nik))
                        {
                            pr.result = false;
                            pr.message = "NIK/No ID Card is exist";
                        }
                        else
                        {
                            var days_trial = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("days_trial"));
                            var free_balance = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("free_balance"));
                            string query = string.Format("");
                            query += string.Format($" INSERT INTO UserInfo (id,name,email_address,phone_number,payment,license,input_date,update_date,status,id_subscriber,nik) VALUES (@id_user_info, @name_user_info, @email_user_info, @phone_number_user_info, @payment_user_info, @license_user_info, @input_date_user_info::timestamp, @update_date_user_info::timestamp, @status_user_info, @subscriber_user_info, @nik);");

                            cmd.Parameters.AddWithValue("id_user_info", model.user_info.id);
                            cmd.Parameters.AddWithValue("name_user_info", model.user_info.name);
                            cmd.Parameters.AddWithValue("email_user_info", kripto.Enkrip(model.user_info.email_address));
                            cmd.Parameters.AddWithValue("phone_number_user_info", kripto.Enkrip(model.user_info.phone_number));
                            cmd.Parameters.AddWithValue("payment_user_info", (int)model.user_info.payment);
                            cmd.Parameters.AddWithValue("license_user_info", model.user_info.license);
                            cmd.Parameters.AddWithValue("status_user_info", (int)model.user_info.status);
                            cmd.Parameters.AddWithValue("subscriber_user_info", model.user_info.id_subscriber);
                            cmd.Parameters.AddWithValue("input_date_user_info", model.user_info.input_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("update_date_user_info", model.user_info.update_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("nik", kripto.Hashing(model.user_info.nik));


                            query += string.Format(" INSERT INTO UserLoginInfo (id,id_user_info,user_name,password_login,is_active,login_type,input_date,update_date,active_date,change_password_date) VALUES (@id_user_login_info,@id_user_info,@user_name_user_login_info,@password_user_login_info,@is_active_user_login_info,@login_type_user_login_info,@input_date_user_login_info::timestamp,@update_date_user_login_info::timestamp,@active_date_user_login_info::timestamp,@input_date_user_login_info::timestamp);");

                            cmd.Parameters.AddWithValue("id_user_login_info", model.user_login_info.id);
                            cmd.Parameters.AddWithValue("user_name_user_login_info", kripto.Hashing(model.user_login_info.user_name));
                            cmd.Parameters.AddWithValue("password_user_login_info", model.user_login_info.password_login.TrimEnd('\0'));
                            cmd.Parameters.AddWithValue("is_active_user_login_info", Convert.ToBoolean(model.user_login_info.is_active));
                            cmd.Parameters.AddWithValue("login_type_user_login_info", (int)model.user_login_info.login_type);
                            cmd.Parameters.AddWithValue("input_date_user_login_info", model.user_login_info.input_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("update_date_user_login_info", model.user_login_info.update_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("active_date_user_login_info", model.user_login_info.active_date.AddDays(days_trial).ToString(Const.format_string_date));

                            int i_list_user_package = 0;
                            foreach (var lup in model.list_user_package)
                            {
                                Guid id_user_package = Guid.NewGuid();
                                query += string.Format($" INSERT INTO UserPackage (id,id_user_info,package,expired_date) VALUES (@id_package_{i_list_user_package},@id_user_info, @package_{i_list_user_package}, @expired_date_{i_list_user_package}::timestamp);");

                                cmd.Parameters.AddWithValue(string.Format($"id_package_{i_list_user_package}"), id_user_package);
                                cmd.Parameters.AddWithValue(string.Format($"package_{i_list_user_package}"), (int)lup.package);
                                cmd.Parameters.AddWithValue(string.Format($"expired_date_{i_list_user_package}"), DateTime.UtcNow.AddDays(days_trial).ToString(Const.format_string_date));

                                query += string.Format($" INSERT INTO UserAccess (id,id_user_package,sum_access,date_update) VALUES (@id_user_access_{i_list_user_package},@id_package_{i_list_user_package}, @sum_access_{i_list_user_package}, @update_date_{i_list_user_package}::timestamp);");

                                cmd.Parameters.AddWithValue(string.Format($"id_user_access_{i_list_user_package}"), Guid.NewGuid());
                                cmd.Parameters.AddWithValue(string.Format($"sum_access_{i_list_user_package}"), free_balance);
                                cmd.Parameters.AddWithValue(string.Format($"update_date_{i_list_user_package}"), DateTime.UtcNow.ToString(Const.format_string_date));

                                i_list_user_package++;
                            }

                            query += string.Format($" INSERT INTO UserFromEkyc (id,id_user_info,id_ekyc) VALUES (@id_user_from_ekyc,@id_user_info,@id_ekyc);");

                            cmd.Parameters.AddWithValue("id_user_from_ekyc", Guid.NewGuid());
                            cmd.Parameters.AddWithValue("id_ekyc", model.user_from_ekyc.id_ekyc);
                            
                            query += string.Format($"");
                            cmd.CommandText = query;
                            cmd.Connection = a;
                            cmd.ExecuteNonQuery();
                            pr.result = true;
                            pr.message = "Success";
                        }
                        return pr;
                    }
                }
            }
            catch (Exception ex)
            {
                pr.result = false;
                pr.message = ex.Message.ToString();
                return pr;
            }
        }

        public PostResult PostPR(UserPR model)
        {
            PostResult pr = new PostResult();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        if (IsExistUserLoginInfo(model.user_login_info.user_name))
                        {
                            pr.result = false;
                            pr.message = "User name is exist";
                        }
                        //else if (IsExistNik(model.user_info.nik))
                        //{
                        //    pr.result = false;
                        //    pr.message = "NIK/No ID Card is exist";
                        //}
                        else
                        {
                            var days_trial = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("days_trial"));
                            var free_balance = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("free_balance"));
                            string query = string.Format("");
                            query += string.Format($" INSERT INTO UserInfo (id,name,email_address,phone_number,payment,license,input_date,update_date,status,id_subscriber,nik) VALUES (@id_user_info, @name_user_info, @email_user_info, @phone_number_user_info, @payment_user_info, @license_user_info, @input_date_user_info::timestamp, @update_date_user_info::timestamp, @status_user_info, @subscriber_user_info, @nik);");

                            cmd.Parameters.AddWithValue("id_user_info", model.user_info.id);
                            cmd.Parameters.AddWithValue("name_user_info", model.badan_usaha.nama);
                            cmd.Parameters.AddWithValue("email_user_info", kripto.Enkrip(model.user_info.email_address));
                            cmd.Parameters.AddWithValue("phone_number_user_info", kripto.Enkrip(model.user_info.phone_number));
                            cmd.Parameters.AddWithValue("payment_user_info", (int)model.user_info.payment);
                            cmd.Parameters.AddWithValue("license_user_info", model.user_info.license);
                            cmd.Parameters.AddWithValue("status_user_info", (int)model.user_info.status);
                            cmd.Parameters.AddWithValue("subscriber_user_info", model.user_info.id_subscriber);
                            cmd.Parameters.AddWithValue("input_date_user_info", model.user_info.input_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("update_date_user_info", model.user_info.update_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("nik", kripto.Hashing(model.user_login_info.user_name));

                            query += string.Format($" INSERT INTO BadanUsaha (id,id_user_info,nama) VALUES (@id_badan_usaha,@id_user_info, @nama_badan_usaha);");

                            cmd.Parameters.AddWithValue("id_badan_usaha", model.badan_usaha.id);
                            cmd.Parameters.AddWithValue("nama_badan_usaha", model.badan_usaha.nama);

                            query += string.Format(" INSERT INTO UserLoginInfo (id,id_user_info,user_name,password_login,is_active,login_type,input_date,update_date,active_date,change_password_date) VALUES (@id_user_login_info,@id_user_info,@user_name_user_login_info,@password_user_login_info,@is_active_user_login_info,@login_type_user_login_info,@input_date_user_login_info::timestamp,@update_date_user_login_info::timestamp,@active_date_user_login_info::timestamp,@input_date_user_login_info::timestamp);");

                            cmd.Parameters.AddWithValue("id_user_login_info", model.user_login_info.id);
                            cmd.Parameters.AddWithValue("user_name_user_login_info", kripto.Hashing(model.user_login_info.user_name));
                            cmd.Parameters.AddWithValue("password_user_login_info", model.user_login_info.password_login.TrimEnd('\0'));
                            cmd.Parameters.AddWithValue("is_active_user_login_info", Convert.ToBoolean(model.user_login_info.is_active));
                            cmd.Parameters.AddWithValue("login_type_user_login_info", (int)model.user_login_info.login_type);
                            cmd.Parameters.AddWithValue("input_date_user_login_info", model.user_login_info.input_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("update_date_user_login_info", model.user_login_info.update_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("active_date_user_login_info", model.user_login_info.active_date.AddDays(days_trial).ToString(Const.format_string_date));

                            int i_list_user_package = 0;
                            foreach (var lup in model.list_user_package)
                            {
                                Guid id_user_package = Guid.NewGuid();
                                query += string.Format($" INSERT INTO UserPackage (id,id_user_info,package,expired_date) VALUES (@id_package_{i_list_user_package},@id_user_info, @package_{i_list_user_package}, @expired_date_{i_list_user_package}::timestamp);");

                                cmd.Parameters.AddWithValue(string.Format($"id_package_{i_list_user_package}"), id_user_package);
                                cmd.Parameters.AddWithValue(string.Format($"package_{i_list_user_package}"), (int)lup.package);
                                cmd.Parameters.AddWithValue(string.Format($"expired_date_{i_list_user_package}"), DateTime.UtcNow.AddDays(days_trial).ToString(Const.format_string_date));

                                query += string.Format($" INSERT INTO UserAccess (id,id_user_package,sum_access,date_update) VALUES (@id_user_access_{i_list_user_package},@id_package_{i_list_user_package}, @sum_access_{i_list_user_package}, @update_date_{i_list_user_package}::timestamp);");

                                cmd.Parameters.AddWithValue(string.Format($"id_user_access_{i_list_user_package}"), Guid.NewGuid());
                                cmd.Parameters.AddWithValue(string.Format($"sum_access_{i_list_user_package}"), free_balance);
                                cmd.Parameters.AddWithValue(string.Format($"update_date_{i_list_user_package}"), DateTime.UtcNow.ToString(Const.format_string_date));

                                i_list_user_package++;
                            }

                            query += string.Format($" INSERT INTO UserFromEkyc (id,id_user_info,id_ekyc) VALUES (@id_user_from_ekyc,@id_user_info,@id_ekyc);");

                            cmd.Parameters.AddWithValue("id_user_from_ekyc", Guid.NewGuid());
                            cmd.Parameters.AddWithValue("id_ekyc", model.user_from_ekyc.id_ekyc);

                            query += string.Format($"");
                            cmd.CommandText = query;
                            cmd.Connection = a;
                            cmd.ExecuteNonQuery();
                            pr.result = true;
                            pr.message = "Success";
                        }
                        return pr;
                    }
                }
            }
            catch (Exception ex)
            {
                pr.result = false;
                pr.message = ex.Message.ToString();
                return pr;
            }
        }

        public bool PostUserLoginDatas(UserLoginDatas model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Empty;
                        if (IsExistUserLoginDatas(model.user_name))
                        {
                            query = string.Format($"UPDATE UserLoginDatas SET token=@model_token, expiry=@expiry::timestamp, last_login=@last_login::timestamp Where user_name_hash =@user_name_hash");

                            cmd.Parameters.AddWithValue("model_token", model.token);
                            cmd.Parameters.AddWithValue("user_name_hash", kripto.Hashing(model.user_name));
                            cmd.Parameters.AddWithValue("expiry", model.expiry.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("last_login", model.last_login.ToString(Const.format_string_date));
                        }
                        else
                        {
                            query = string.Format($"INSERT INTO UserLoginDatas (id,user_name,token,expiry,last_login,user_name_hash) VALUES (@model_id, @model_user_name_2, @model_token_2, @expiry::timestamp, @last_login::timestamp,@user_name_hash)");

                            cmd.Parameters.AddWithValue("model_id", model.id);
                            cmd.Parameters.AddWithValue("model_user_name_2", kripto.Enkrip(model.user_name));
                            cmd.Parameters.AddWithValue("model_token_2", model.token);
                            cmd.Parameters.AddWithValue("expiry", model.expiry.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("last_login", model.last_login.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("user_name_hash", kripto.Hashing(model.user_name));
                        }
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetUserLogin(string user_name, string token)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT user_name FROM UserLoginDatas Where user_name_hash =@user_name_hash and token = @token");
                        cmd.Parameters.AddWithValue("user_name_hash", kripto.Hashing(user_name));
                        cmd.Parameters.AddWithValue("token", token);
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        string result = kripto.Dekrip((string)cmd.ExecuteScalar());

                        if (result != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }   
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsExistUserLoginDatas(string user_name)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {

                    string query = "SELECT user_name FROM UserLoginDatas Where user_name_hash =@p1";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, a))
                    {
                        cmd.Parameters.AddWithValue("p1", kripto.Hashing(user_name));
                        string result = (string)cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsExistUserLoginInfo(string user_name)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {

                    string query = "SELECT user_name FROM UserLoginInfo Where user_name =@p1";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, a))
                    {
                        cmd.Parameters.AddWithValue("p1", kripto.Hashing(user_name));
                        string result = (string)cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsExistNik(string nik)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {

                    string query = "SELECT nik FROM UserInfo Where nik =@p1";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, a))
                    {
                        cmd.Parameters.AddWithValue("p1", kripto.Hashing(nik));
                        string result = (string)cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GetResultObject<UserLoginInfo> GetUserLoginInfo(string user_name)
        {
            UserLoginInfo models = new UserLoginInfo();
            GetResultObject<UserLoginInfo> result = new GetResultObject<UserLoginInfo>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Empty;
                        query = string.Format($"select a.*, b.id_subscriber from UserLoginInfo a left join UserInfo b on a.id_user_info = b.id where user_name = @user_name");
                        cmd.Parameters.AddWithValue("user_name", kripto.Hashing(user_name));
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();
                        result.result = false;
                        while (_con.reader.Read())
                        {
                            models = new UserLoginInfo() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), id_user_info = Guid.Parse(Convert.ToString(_con.reader["id_user_info"])), user_name = Convert.ToString(_con.reader["user_name"]), password_login = Convert.ToString(_con.reader["password_login"]), is_active = Convert.ToBoolean(_con.reader["is_active"]), login_type = (LoginType)Convert.ToInt32(_con.reader["login_type"]), input_date = Convert.ToDateTime(_con.reader["input_date"]), update_date = Convert.ToDateTime(_con.reader["update_date"]), active_date = Convert.ToDateTime(_con.reader["active_date"]), id_subscriber = Guid.Parse(Convert.ToString(_con.reader["id_subscriber"])) };
                            result.result = true;
                        }
                        _con.reader.Close();
                    }
                }
                result.objek = models;
                return result;
            }
            catch (Exception ex)
            {
                result.result = false;
                result.message = ex.Message.ToString();
                return result;
            }
        }

        public GetResultObject<List<UserInfo>> Get(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserInfo>> result = new GetResultObject<List<UserInfo>>();
            try
            {
                List<UserInfo> models = new List<UserInfo>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT u.*, s.status as status_string, uk.id_ekyc FROM UserInfo u left join MStatusUserInfo s on u.status = s.id left join UserFromEkyc uk on uk.id_user_info = u.id left join UserLoginInfo uli on uli.id_user_info = u.id WHERE (uli.login_type NOT IN ({(int)LoginType.admin}, {(int)LoginType.superadmin}, {(int)LoginType.statis}))");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" AND (name LIKE @s1 OR email_address like @s2 OR phone_number like @s3 OR to_char(u.input_date, '{Const.format_string_date}') like @s4 OR to_char(u.update_date, '{Const.format_string_date}') like @s5 OR s.status like @s6)");
                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                        }

                        if (kolom == 1)
                        {
                            nama_kolom = "u.name";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "u.email_address";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "u.phone_number";
                        }
                        else if (kolom == 4)
                        {
                            nama_kolom = "u.input_date";
                        }
                        else if (kolom == 5)
                        {
                            nama_kolom = "u.update_date";
                        }
                        else if (kolom == 6)
                        {
                            nama_kolom = "s.status";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");
                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();
                        while (_con.reader.Read())
                        {
                            string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));
                            string phone_number = kripto.Dekrip(Convert.ToString(_con.reader["phone_number"]));

                            UserInfo ts = new UserInfo() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), name = Convert.ToString(_con.reader["name"]), email_address = email_address, phone_number = phone_number, input_date = Convert.ToDateTime(_con.reader["input_date"]), update_date = Convert.ToDateTime(_con.reader["update_date"]), status_string = Convert.ToString(_con.reader["status_string"]), status = (StatusUserInfo)Convert.ToInt32(_con.reader["status"]), id_ekyc = Guid.Parse(Convert.ToString(_con.reader["id_ekyc"])) };
                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserInfo>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserInfo>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlah(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT count(u.id) as jumlah FROM UserInfo u left join MStatusUserInfo s on u.status = s.id left join UserFromEkyc uk on uk.id_user_info = u.id left join UserLoginInfo uli on uli.id_user_info = u.id WHERE (uli.login_type NOT IN ({(int)LoginType.admin}, {(int)LoginType.superadmin}, {(int)LoginType.statis}))");

                        if (search != null)
                        {
                            query += string.Format($" AND (name LIKE @s1 OR email_address like @s2 OR phone_number like @s3 OR to_char(u.input_date, '{Const.format_string_date}') like @s4 OR to_char(u.update_date, '{Const.format_string_date}') like @s5 OR s.status like @s6)");
                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                        }
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = Convert.ToInt64(cmd.ExecuteScalar());
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool UpdateStatus(User model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format("");
                        query += string.Format($" UPDATE UserInfo SET status = @status_user_info, update_date =@update_date_user_info::timestamp WHERE id = @id_user_info;");
                        query += string.Format($" UPDATE UserLoginInfo SET is_active = @is_active_user_login_info, update_date =@update_date_user_login_info::timestamp WHERE id_user_info = @id_user_info;");
                        query += string.Format($"");

                        cmd.Parameters.AddWithValue("status_user_info", (int)model.user_info.status);
                        cmd.Parameters.AddWithValue("id_user_info", model.user_info.id);
                        cmd.Parameters.AddWithValue("is_active_user_login_info", Convert.ToBoolean(model.user_login_info.is_active));
                        cmd.Parameters.AddWithValue("update_date_user_info", model.user_info.update_date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        cmd.Parameters.AddWithValue("update_date_user_login_info", model.user_login_info.update_date.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public GetResultObject<User> GetUser(Guid id_user_info)
        {
            User models = new User();
            GetResultObject<User> result = new GetResultObject<User>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Empty;

                        query = string.Format($"select u.*, up.expired_date as expired_date_package, mp.package as package_string, up.package from UserInfo u left join UserLoginInfo ul on u.id = ul.id_user_info left join UserPackage up on u.id = up.id_user_info left join MPackage mp on up.package = mp.id left join MStatusUserInfo ms on ms.id = u.status where u.id = @id_user_info");
                        cmd.Parameters.AddWithValue("id_user_info", id_user_info);
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        List<UserPackage> list_user_package = new List<UserPackage>();
                        UserInfo user_info = new UserInfo();
                        while (_con.reader.Read())
                        {

                            list_user_package.Add(new UserPackage() { expired_date = Convert.ToDateTime(_con.reader["expired_date_package"]), package_string = Convert.ToString(_con.reader["package_string"]), package = (TypePackage)Convert.ToInt32(_con.reader["package"]) });
                            if (user_info.id == Guid.Empty)
                            {
                                string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));
                                string phone_number = kripto.Dekrip(Convert.ToString(_con.reader["phone_number"]));

                                user_info = new UserInfo() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), email_address = email_address, input_date = Convert.ToDateTime(_con.reader["input_date"]), license = Convert.ToString(_con.reader["license"]), name = Convert.ToString(_con.reader["name"]), payment = (TypePayment)Convert.ToInt32(_con.reader["payment"]), phone_number = phone_number, update_date = Convert.ToDateTime(_con.reader["update_date"]), status = (StatusUserInfo)Convert.ToInt32(_con.reader["status"]), id_subscriber = Guid.Parse(Convert.ToString(_con.reader["id_subscriber"])) };
                            }

                        }
                        _con.reader.Close();
                        models.list_user_package = list_user_package;
                        models.user_info = user_info;
                    }
                    result.objek = models;
                    result.result = true;
                    return result;
                }    
            }
            catch (Exception ex)
            {
                result.result = false;
                result.message = ex.Message.ToString();
                return result;
            }
        }

        public bool PostUserLog(UserLog model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"INSERT INTO userlog (id,user_name,activity,time_input,status,apps,status_log,un) VALUES (@id, @user_name, @activity, @time_input::timestamp, @status, @apps, @status_log, @un)");
                        cmd.Parameters.AddWithValue("id", model.id);
                        cmd.Parameters.AddWithValue("user_name", kripto.Enkrip(model.user_name));
                        cmd.Parameters.AddWithValue("activity", model.activity);
                        cmd.Parameters.AddWithValue("status", model.status);
                        cmd.Parameters.AddWithValue("apps", model.apps);
                        cmd.Parameters.AddWithValue("status_log", model.status_log);
                        cmd.Parameters.AddWithValue("time_input", model.time_input.ToString(Const.format_string_date));
                        cmd.Parameters.AddWithValue("un", kripto.Hashing(model.user_name));
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateUserLogStatus(UserLog model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"UPDATE userlog set status_log = '{Const.log_reviewed}' where id IN ({model.status_log})");
                    var cmd = new NpgsqlCommand(query, a);
                    //cmd.Parameters.AddWithValue("status_log", string.Format($"{model.status_log}"));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GetResultObject<List<UserLog>> GetLog(int start, int length, int kolom, string order_by, string search, string from_date, string to_date)
        {
            GetResultObject<List<UserLog>> result = new GetResultObject<List<UserLog>>();
            try
            {
                List<UserLog> models = new List<UserLog>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT * FROM userlog");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" WHERE (un like (@s1) OR activity like (@s2) OR to_char(time_input, '{Const.format_string_date}') like (@s3) OR status like (@s4) OR apps like (@s5) OR status_log like (@s6))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{kripto.Hashing(search)}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                        }

                        if (from_date != null && to_date != null)
                        {
                            if (search != null)
                            {
                                query += string.Format($" AND");
                            }
                            else
                            {
                                query += string.Format($" WHERE");
                            }

                            query += string.Format($" (time_input >= @from_date::timestamp and time_input <= @to_date::timestamp)");

                            cmd.Parameters.AddWithValue("from_date", string.Format($"{from_date} 00:00:00.000"));
                            cmd.Parameters.AddWithValue("to_date", string.Format($"{to_date} 23:59:59.000"));
                        }

                        if (kolom == 1)
                        {
                            nama_kolom = "user_name";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "activity";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "time_input";
                        }
                        else if (kolom == 4)
                        {
                            nama_kolom = "status";
                        }
                        else if (kolom == 5)
                        {
                            nama_kolom = "apps";
                        }
                        else if (kolom == 6)
                        {
                            nama_kolom = "status_log";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");
                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            string user_name = kripto.Dekrip(Convert.ToString(_con.reader["user_name"]));

                            UserLog ts = new UserLog() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), user_name = user_name, activity = Convert.ToString(_con.reader["activity"]), time_input = Convert.ToDateTime(_con.reader["time_input"]), status = Convert.ToString(_con.reader["status"]), apps = Convert.ToString(_con.reader["apps"]), status_log = Convert.ToString(_con.reader["status_log"]) };
                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserLog>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }    
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserLog>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahLog(string search, string from_date, string to_date)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT count(id) as jumlah FROM userlog");
                        if (search != null)
                        {
                            query += string.Format($" WHERE (un like (@s1) OR activity like (@s2) OR to_char(time_input, '{Const.format_string_date}') like (@s3) OR status like (@s4) OR apps like (@s5) OR status_log like (@s6))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{kripto.Hashing(search)}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                        }

                        if (from_date != null && to_date != null)
                        {
                            if (search != null)
                            {
                                query += string.Format($" AND");
                            }
                            else
                            {
                                query += string.Format($" WHERE");
                            }

                            query += string.Format($" (time_input >= @from_date::timestamp and time_input <= @to_date::timestamp)");

                            cmd.Parameters.AddWithValue("from_date", string.Format($"{from_date} 00:00:00.000"));
                            cmd.Parameters.AddWithValue("to_date", string.Format($"{to_date} 23:59:59.000"));
                        }
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = Convert.ToInt64(cmd.ExecuteScalar());
                        return total;
                    }
                       
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool Update(UserInfo model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"UPDATE UserInfo SET name = @name, phone_number = @phone_number, update_date =@update_date::timestamp WHERE id = @id");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("name", model.name);
                    cmd.Parameters.AddWithValue("phone_number", kripto.Enkrip(model.phone_number));
                    cmd.Parameters.AddWithValue("id", model.id);
                    cmd.Parameters.AddWithValue("update_date", model.update_date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdatePassword(UserLoginInfo model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"UPDATE UserLoginInfo SET password_login = @password, update_date =@update_date::timestamp, change_password_date= @update_date::timestamp WHERE id_user_info = @id_user_info");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("password", model.password_login.TrimEnd('\0'));
                    cmd.Parameters.AddWithValue("id_user_info", model.id_user_info);
                    cmd.Parameters.AddWithValue("update_date", model.update_date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePasswordForgot(UserLoginInfo model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"UPDATE UserLoginInfo SET password_login = @password, update_date =@update_date::timestamp, change_password_date= @update_date::timestamp WHERE user_name = @user_name");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("password", model.password_login.TrimEnd('\0'));
                    cmd.Parameters.AddWithValue("user_name", kripto.Hashing(model.user_name));
                    cmd.Parameters.AddWithValue("update_date", model.update_date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool PostUserAccess(UserAccess model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format("");
                        if (IsExistUserAccess(model.id_user_package))
                        {
                            query += string.Format($"Update UserAccess SET sum_access=@sum_access, date_update=@date_update::timestamp Where id_user_package =@id_user_package;");

                            cmd.Parameters.AddWithValue("sum_access", model.sum_access);
                            cmd.Parameters.AddWithValue("id_user_package", model.id_user_package);
                            cmd.Parameters.AddWithValue("date_update", model.date_update.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }
                        else
                        {
                            query += string.Format($"INSERT INTO UserAccess (id,id_user_package,sum_access,date_update) VALUES (@id,@id_user_package,@sum_access,@date_update::timestamp);");

                            cmd.Parameters.AddWithValue("id", model.id);
                            cmd.Parameters.AddWithValue("id_user_package", model.id_user_package);
                            cmd.Parameters.AddWithValue("sum_access", model.sum_access);
                            cmd.Parameters.AddWithValue("date_update", model.date_update.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }

                        query += string.Format($" INSERT INTO UserAccessHt (id,id_user_package,sum_access,date_input) VALUES (@id_user_access_ht,@id_user_package,@sum_access,@date_update::timestamp);");

                        cmd.Parameters.AddWithValue("id_user_access_ht", Guid.NewGuid());
                        //cmd.Parameters.AddWithValue("id_user_package", model.id_user_package);
                        //cmd.Parameters.AddWithValue("sum_access", model.sum_access);

                        query += string.Format($"");
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        cmd.ExecuteNonQuery();
                        return true;
                    }    
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool MinUserAccess(UserAccess model)
        {
            try
            {
                UserAccess user_access = this.GetUserAccessByIdUserPackage(model.id_user_info, model.package);
                Int64 sum_access_now = user_access.sum_access - 1;
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Empty;
                    query = string.Format($"Update UserAccess SET sum_access=@sum_access_now Where id_user_package =@id_user_package");

                    var cmd = new NpgsqlCommand(query, a);

                    cmd.Parameters.AddWithValue("sum_access_now", sum_access_now);
                    cmd.Parameters.AddWithValue("id_user_package", user_access.id_user_package);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public UserAccess GetUserAccessByIdUserPackage(Guid id_user_info, TypePackage type_package)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"SELECT ua.* FROM UserAccess ua left join UserPackage up on ua.id_user_package = up.id Where up.id_user_info = @id_user_info and up.package = @type_package");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id_user_info", id_user_info);
                    cmd.Parameters.AddWithValue("type_package", (int)type_package);
                    _con.reader = cmd.ExecuteReader();

                    UserAccess user_access = new UserAccess();

                    while (_con.reader.Read())
                    {
                        user_access = new UserAccess() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), id_user_package = Guid.Parse(Convert.ToString(_con.reader["id_user_package"])), sum_access = Convert.ToInt64(_con.reader["sum_access"]), date_update = Convert.ToDateTime(_con.reader["date_update"]) };
                    }
                    _con.reader.Close();
                    return user_access;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool IsExistUserAccess(Guid id_user_package)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"SELECT id_user_package FROM UserAccess Where id_user_package =@id_user_package");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id_user_package", id_user_package);
                    Guid result = (Guid)cmd.ExecuteScalar();

                    if (result == id_user_package)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public UserInfoAccess GetUserInfoAccess(Guid id_user_info, TypePackage type_package)
        {
            UserInfoAccess models = new UserInfoAccess();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Empty;

                    query = string.Format($"select ud.id, ud.email_address, ud.name, ua.sum_access from UserPackage up left join UserInfo ud on up.id_user_info = ud.id left join UserAccess ua on up.id = ua.id_user_package where ud.id = @id_user_info and up.package = @package");

                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id_user_info", id_user_info);
                    cmd.Parameters.AddWithValue("package", (int)type_package);
                    _con.reader = cmd.ExecuteReader();

                    UserInfo user_info = new UserInfo();
                    UserAccess user_access = new UserAccess();

                    while (_con.reader.Read())
                    {
                        string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));

                        user_info = new UserInfo() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), email_address = email_address, name = Convert.ToString(_con.reader["name"])};

                        if (_con.reader.IsDBNull(3))
                        {
                            user_access = new UserAccess() { sum_access = 0 };
                        }
                        else
                        {
                            user_access = new UserAccess() { sum_access = Convert.ToInt64(_con.reader["sum_access"]) };
                        }
                    }

                    models.user_info = user_info;
                    models.user_access = user_access;

                    _con.reader.Close();
                    return models;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public GetResultObject<List<UserAccess>> GetUserAccess(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserAccess>> result = new GetResultObject<List<UserAccess>>();
            try
            {
                List<UserAccess> models = new List<UserAccess>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT up.id, ud.email_address, u.sum_access, u.date_update, mp.package FROM UserAccess u right join UserPackage up on u.id_user_package = up.id left join mpackage mp on mp.id = up.package left join UserInfo ud on ud.id = up.id_user_info left join UserLoginInfo c on c.id_user_info = ud.id WHERE c.login_type ={(int)LoginType.client}");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" AND (ud.email_address like (@s1) OR to_char(u.sum_access, '9') like (@s2) OR to_char(u.date_update, '{Const.format_string_date}') like (@s3) OR mp.package like (@s4))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                        }

                        if (kolom == 1)
                        {
                            nama_kolom = "ud.email_address";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "u.sum_access";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "u.date_update";
                        }
                        else if (kolom == 4)
                        {
                            nama_kolom = "mp.package";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");
                        
                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));

                            UserAccess ts = new UserAccess() { user_data_nama = email_address, id_user_package = Guid.Parse(Convert.ToString(_con.reader["id"])) };

                            if (!_con.reader.IsDBNull(2))
                            {
                                ts.sum_access = Convert.ToInt64(_con.reader["sum_access"]);
                            }

                            if (!_con.reader.IsDBNull(3))
                            {
                                ts.date_update = Convert.ToDateTime(_con.reader["date_update"]);
                            }

                            if (!_con.reader.IsDBNull(4))
                            {
                                ts.user_package = Convert.ToString(_con.reader["package"]);
                            }

                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserAccess>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserAccess>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahTotalUserAccess(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT COUNT(ud.id) as jumlah FROM UserAccess u right join UserPackage up on u.id_user_package = up.id left join mpackage mp on mp.id = up.package left join UserInfo ud on ud.id = up.id_user_info left join UserLoginInfo c on c.id_user_info = ud.id WHERE c.login_type ={(int)LoginType.client}");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" AND (ud.email_address like (@s1) OR to_char(u.sum_access, '9') like (@s2) OR to_char(u.date_update, '{Const.format_string_date}') like (@s3) OR mp.package like (@s4))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                        }

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = (long)cmd.ExecuteScalar();
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public GetResultObject<List<UserAccess>> GetUserAccessHt(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserAccess>> result = new GetResultObject<List<UserAccess>>();
            try
            {
                List<UserAccess> models = new List<UserAccess>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT ud.id, ud.email_address, u.sum_access, u.date_input, mp.package FROM UserAccessHt u left join UserPackage up on u.id_user_package = up.id left join mpackage mp on mp.id = up.package left join UserInfo ud on ud.id = up.id_user_info");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" WHERE (ud.email_address like (@s1) OR to_char(u.sum_access, '9') like (@s2) OR to_char(u.date_input, '{Const.format_string_date}') like (@s3) OR mp.package like (@s4))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                        }

                        if (kolom == 1)
                        {
                            nama_kolom = "ud.email_address";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "u.sum_access";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "u.date_input";
                        }
                        else if (kolom == 4)
                        {
                            nama_kolom = "mp.package";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");

                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));

                            UserAccess ts = new UserAccess() { user_data_nama = email_address, id_user_package = Guid.Parse(Convert.ToString(_con.reader["id"])) };

                            if (!_con.reader.IsDBNull(2))
                            {
                                ts.sum_access = Convert.ToInt64(_con.reader["sum_access"]);
                            }

                            if (!_con.reader.IsDBNull(3))
                            {
                                ts.date_update = Convert.ToDateTime(_con.reader["date_input"]);
                            }

                            if (!_con.reader.IsDBNull(4))
                            {
                                ts.user_package = Convert.ToString(_con.reader["package"]);
                            }

                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserAccess>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserAccess>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahTotalUserAccessHt(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT COUNT(ud.id) as jumlah FROM UserAccessHt u left join UserPackage up on u.id_user_package = up.id left join mpackage mp on mp.id = up.package left join UserInfo ud on ud.id = up.id_user_info");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" WHERE (ud.email_address like (@s1) OR to_char(u.sum_access, '9') like (@s2) OR to_char(u.date_input, '{Const.format_string_date}') like (@s3) OR mp.package like (@s4))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                        }

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = (long)cmd.ExecuteScalar();
                        return total;
                    } 
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public GetResultObject<List<UserInfo>> GetAdmin(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserInfo>> result = new GetResultObject<List<UserInfo>>();
            try
            {
                List<UserInfo> models = new List<UserInfo>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT u.*, s.status as status_string, uk.id_ekyc FROM UserInfo u left join MStatusUserInfo s on u.status = s.id left join UserFromEkyc uk on uk.id_user_info = u.id left join UserLoginInfo uli on uli.id_user_info = u.id WHERE (uli.login_type IN ({(int)LoginType.admin}))");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" AND (name like (@s1) OR email_address like (@s2) OR phone_number like (@s3) OR to_char(u.input_date, '{Const.format_string_date}') like (@s4) OR to_char(u.update_date, '{Const.format_string_date}') like (@s5) OR s.status like (@s6))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                        }

                        if (kolom == 1)
                        {
                            nama_kolom = "u.name";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "u.email_address";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "u.phone_number";
                        }
                        else if (kolom == 4)
                        {
                            nama_kolom = "u.input_date";
                        }
                        else if (kolom == 5)
                        {
                            nama_kolom = "u.update_date";
                        }
                        else if (kolom == 6)
                        {
                            nama_kolom = "s.status";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");

                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));
                            string phone_number = kripto.Dekrip(Convert.ToString(_con.reader["phone_number"]));

                            UserInfo ts = new UserInfo() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), name = Convert.ToString(_con.reader["name"]), email_address = email_address, phone_number = phone_number, input_date = Convert.ToDateTime(_con.reader["input_date"]), update_date = Convert.ToDateTime(_con.reader["update_date"]), status_string = Convert.ToString(_con.reader["status_string"]), status = (StatusUserInfo)Convert.ToInt32(_con.reader["status"]) };
                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserInfo>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserInfo>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahAdmin(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT count(u.id) as jumlah FROM UserInfo u left join MStatusUserInfo s on u.status = s.id left join UserFromEkyc uk on uk.id_user_info = u.id left join UserLoginInfo uli on uli.id_user_info = u.id WHERE (uli.login_type IN ({(int)LoginType.admin}))");

                        if (search != null)
                        {
                            query += string.Format($" AND (name like (@s1) OR email_address like (@s2) OR phone_number like (@s3) OR to_char(u.input_date, '{Const.format_string_date}') like (@s4) OR to_char(u.update_date, '{Const.format_string_date}') like (@s5) OR s.status like (@s6))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                        }

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = Convert.ToInt64(cmd.ExecuteScalar());
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public PostResult PostAdmin(User model)
        {
            PostResult pr = new PostResult();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    if (IsExistUserLoginInfo(model.user_login_info.user_name))
                    {
                        pr.result = false;
                        pr.message = "User name is exist";
                    }
                    else
                    {
                        var days_trial = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("days_trial"));
                        string query = string.Format("");
                        query += string.Format($" INSERT INTO UserInfo (id,name,email_address,phone_number,payment,license,input_date,update_date,status,id_subscriber,nik) VALUES (@model.user_info.id, @model.user_info.name, @model.user_info.email_address, @model.user_info.phone_number, @model.user_info.payment, @model.user_info.license, @model.user_info.input_date::timestamp, @model.user_info.update_date::timestamp, @model.user_info.status, @model.user_info.id_subscriber, @nik);");
                        query += string.Format(" INSERT INTO UserLoginInfo (id,id_user_info,user_name,password_login,is_active,login_type,input_date,update_date,active_date,change_password_date) VALUES (@model.user_login_info.id,@model.user_info.id,@model.user_login_info.user_name,@model.user_login_info.password_login,@model.user_login_info.is_active,@model.user_login_info.login_type,@model.user_login_info.input_date::timestamp,@model.user_login_info.update_date::timestamp,@model.user_login_info.active_date::timestamp,@model.user_login_info.input_date::timestamp);");

                        query += string.Format($"");
                        var cmd = new NpgsqlCommand(query, a);

                        cmd.Parameters.AddWithValue("model.user_info.id", model.user_info.id);
                        cmd.Parameters.AddWithValue("model.user_info.name", model.user_info.name);
                        cmd.Parameters.AddWithValue("model.user_info.email_address", kripto.Enkrip(model.user_info.email_address));
                        cmd.Parameters.AddWithValue("model.user_info.phone_number", kripto.Enkrip(model.user_info.phone_number) );
                        cmd.Parameters.AddWithValue("model.user_info.payment", (int)model.user_info.payment);
                        cmd.Parameters.AddWithValue("model.user_info.license", model.user_info.license);
                        cmd.Parameters.AddWithValue("model.user_info.status", (int)model.user_info.status);
                        cmd.Parameters.AddWithValue("model.user_info.id_subscriber", model.user_info.id_subscriber);
                        cmd.Parameters.AddWithValue("model.user_info.input_date", model.user_info.input_date.ToString(Const.format_string_date));
                        cmd.Parameters.AddWithValue("model.user_info.update_date", model.user_info.update_date.ToString(Const.format_string_date));
                        cmd.Parameters.AddWithValue("nik", kripto.Hashing(model.user_info.nik));

                        cmd.Parameters.AddWithValue("model.user_login_info.id", model.user_login_info.id);
                        cmd.Parameters.AddWithValue("model.user_login_info.user_name", kripto.Hashing(model.user_login_info.user_name));
                        cmd.Parameters.AddWithValue("model.user_login_info.password_login", model.user_login_info.password_login.TrimEnd('\0'));
                        cmd.Parameters.AddWithValue("model.user_login_info.is_active", Convert.ToBoolean(model.user_login_info.is_active));
                        cmd.Parameters.AddWithValue("model.user_login_info.login_type", (int)model.user_login_info.login_type);

                        cmd.Parameters.AddWithValue("model.user_login_info.input_date", model.user_login_info.input_date.ToString(Const.format_string_date));
                        cmd.Parameters.AddWithValue("model.user_login_info.update_date", model.user_login_info.update_date.ToString(Const.format_string_date));
                        cmd.Parameters.AddWithValue("model.user_login_info.active_date", model.user_login_info.active_date.AddDays(days_trial).ToString(Const.format_string_date));

                        cmd.ExecuteNonQuery();
                        pr.result = true;
                        pr.message = "Success";
                    }
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

        public bool DeleteUserAdmin(Guid id_user_info)
        {
            try
            {
                if (!IsNonActive(id_user_info))
                {
                    return false;
                }
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format("");
                    query += string.Format($" DELETE FROM UserLoginInfo WHERE id_user_info = @id_user_info;");
                    query += string.Format($" DELETE FROM UserInfo WHERE id = @id_user_info;");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id_user_info", id_user_info);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteUser(Guid id_user_info)
        {
            try
            {
                if (!IsNonActive(id_user_info))
                {
                    return false;
                }
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format("");

                    if (IsBadanUsaha(id_user_info))
                    {
                        query += string.Format($" DELETE FROM BadanUsaha WHERE id_user_info = @id_user_info;");
                    }

                    query += string.Format($" DELETE FROM UserAccess WHERE id_user_package IN (SELECT id FROM UserPackage where id_user_info = @id_user_info);");
                    query += string.Format($" DELETE FROM UserPackage WHERE id_user_info = @id_user_info;");
                    query += string.Format($" DELETE FROM UserFromEkyc WHERE id_user_info = @id_user_info;");
                    query += string.Format($" DELETE FROM UserLoginInfo WHERE id_user_info = @id_user_info;");
                    query += string.Format($" DELETE FROM UserInfo WHERE id = @id_user_info;");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id_user_info", id_user_info);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GetResultObject<List<long>> GetSumStatus()
        {
            GetResultObject<List<long>> datas = new GetResultObject<List<long>>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    List<long> model = new List<long>();
                    string query = string.Format($"SELECT COUNT(u.id) as jumlah FROM UserInfo u left join UserLoginInfo uli on uli.id_user_info = u.id WHERE(uli.login_type NOT IN({ (int)LoginType.admin}, { (int)LoginType.superadmin}, { (int)LoginType.statis})) and u.status = {(int)StatusUserInfo.non_active} UNION ALL SELECT COUNT(u.id) as jumlah FROM UserInfo u left join UserLoginInfo uli on uli.id_user_info = u.id WHERE(uli.login_type NOT IN({ (int)LoginType.admin}, { (int)LoginType.superadmin}, { (int)LoginType.statis})) and u.status = {(int)StatusUserInfo.active} UNION ALL SELECT COUNT(u.id) as jumlah FROM UserInfo u left join UserLoginInfo uli on uli.id_user_info = u.id WHERE(uli.login_type NOT IN({ (int)LoginType.admin}, { (int)LoginType.superadmin}, { (int)LoginType.statis})) and u.status = {(int)StatusUserInfo.pending}");
                    _con.command = new NpgsqlCommand(query, a);
                    _con.reader = _con.command.ExecuteReader();

                    while (_con.reader.Read())
                    {
                        model.Add(Convert.ToInt64(_con.reader["jumlah"]));
                    }
                    _con.reader.Close();
                    datas.objek = model;
                    datas.result = true;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString(); 
                return datas;
            }
        }

        public GetResultObject<List<SumPerWeeks>> GetSumWeeks()
        {
            GetResultObject<List<SumPerWeeks>> datas = new GetResultObject<List<SumPerWeeks>>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    List<SumPerWeeks> model = new List<SumPerWeeks>();
                    string query = string.Format($"SELECT count(*) as jumlah,to_char( time_input, 'Dy') as hari, date_trunc('day', time_input) FROM userlog WHERE date_trunc('day', time_input) > current_date + '7 day ago'::interval group by to_char( time_input, 'Dy'),to_char( time_input, 'D'), date_trunc('day', time_input) ORDER BY date_trunc('day', time_input) ASC");
                    _con.command = new NpgsqlCommand(query, a);
                    _con.reader = _con.command.ExecuteReader();

                    while (_con.reader.Read())
                    {
                        model.Add( new SumPerWeeks() { count = Convert.ToInt64(_con.reader["jumlah"]), day = Convert.ToString(_con.reader["hari"]) });
                    }
                    _con.reader.Close();
                    datas.objek = model;
                    datas.result = true;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public GetResultObject<UserLoginError> GetLoginError(string user_name)
        {
            GetResultObject<UserLoginError> datas = new GetResultObject<UserLoginError>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    UserLoginError model = new UserLoginError();
                    string query = string.Format($"SELECT sum, update_date FROM UserLoginError where user_name = @user_name");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("user_name", kripto.Hashing(user_name));
                    _con.reader = cmd.ExecuteReader();

                    while (_con.reader.Read())
                    {
                        model = new UserLoginError() { sum = Convert.ToInt32(_con.reader["sum"]), update_date = Convert.ToDateTime(_con.reader["update_date"]) };
                    }
                    _con.reader.Close();
                    datas.objek = model;
                    datas.result = true;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        private bool IsExistLoginError(string user_name)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"SELECT count(id) FROM UserLoginError where user_name = @user_name");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("user_name", kripto.Hashing(user_name));
                    long total = Convert.ToInt64(cmd.ExecuteScalar());
                    if(total == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PostLoginError(int sum_login_error, string user_name)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"INSERT INTO UserLoginError (id, user_name, sum, update_date) VALUES(@id, @user_name, 1, @update_date::timestamp)");
                        cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("user_name", kripto.Hashing(user_name));
                        cmd.Parameters.AddWithValue("update_date", DateTime.UtcNow.ToString(Const.format_string_date));
                        if (IsExistLoginError(user_name))
                        {
                            query = string.Format($"UPDATE UserLoginError SET sum = @sum_login_error, update_date = @update_date::timestamp where user_name = @user_name");
                            cmd.Parameters.AddWithValue("sum_login_error", sum_login_error);
                        }
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        cmd.ExecuteNonQuery();
                        return true;
                    }  
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PostUserLogAdmin(UserLogAdmin model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"INSERT INTO userlogadmin (id,user_name,activity,time_input,status,un) VALUES (@id, @user_name, @activity, @time_input::timestamp, @status, @un)");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id", model.id);
                    cmd.Parameters.AddWithValue("user_name", kripto.Enkrip(model.user_name));
                    cmd.Parameters.AddWithValue("activity", model.activity);
                    cmd.Parameters.AddWithValue("status", model.status);
                    cmd.Parameters.AddWithValue("time_input", model.time_input.ToString(Const.format_string_date));
                    cmd.Parameters.AddWithValue("un", kripto.Hashing(model.user_name));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GetResultObject<List<UserLogAdmin>> GetLogAdmin(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserLogAdmin>> result = new GetResultObject<List<UserLogAdmin>>();
            try
            {
                List<UserLogAdmin> models = new List<UserLogAdmin>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT * FROM userlogadmin");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" WHERE (un like (@s1) OR activity like (@s2) OR to_char(time_input, '{Const.format_string_date}') like (@s3) OR status like (@s4))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{kripto.Hashing(search)}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                        }

                        if (kolom == 0)
                        {
                            nama_kolom = "user_name";
                        }
                        else if (kolom == 1)
                        {
                            nama_kolom = "activity";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "time_input";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "status";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");

                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            string user_name = kripto.Dekrip(Convert.ToString(_con.reader["user_name"]));

                            UserLogAdmin ts = new UserLogAdmin() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), user_name = user_name, activity = Convert.ToString(_con.reader["activity"]), time_input = Convert.ToDateTime(_con.reader["time_input"]), status = Convert.ToString(_con.reader["status"]) };
                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserLogAdmin>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserLogAdmin>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahLogAdmin(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT count(id) as jumlah FROM userlogadmin");
                        if (search != null)
                        {
                            query += string.Format($" WHERE (un like (@s1) OR activity like (@s2) OR to_char(time_input, '{Const.format_string_date}') like (@s3) OR status like (@s4))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{kripto.Hashing(search)}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                        }

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = Convert.ToInt64(cmd.ExecuteScalar());
                        return total;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public GetResultObject<List<UserLoginDatas>> GetUserLastLogin(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserLoginDatas>> result = new GetResultObject<List<UserLoginDatas>>();
            try
            {
                List<UserLoginDatas> models = new List<UserLoginDatas>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT user_name, last_login FROM userlogindatas WHERE user_name_hash NOT IN(@client, @super)");

                        cmd.Parameters.AddWithValue("client", kripto.Hashing("client_register@enkripa.id"));
                        cmd.Parameters.AddWithValue("super",  kripto.Hashing("superadmin@enkripa.id"));
                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" AND (user_name like (@s1) OR to_char(last_login, '{Const.format_string_date}') like (@s2))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                        }

                        if (kolom == 0)
                        {
                            nama_kolom = "user_name";
                        }
                        else if (kolom == 1)
                        {
                            nama_kolom = "last_login";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");

                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            UserLoginDatas ts = new UserLoginDatas() { user_name = kripto.Dekrip(Convert.ToString(_con.reader["user_name"])), last_login = Convert.ToDateTime(_con.reader["last_login"]) };
                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserLoginDatas>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserLoginDatas>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahLastLogin(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT count(id) as jumlah FROM userlogindatas WHERE user_name NOT IN('client_register@enkripa.id', 'superadmin@enkripa.id')");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" AND (user_name like (@s1) OR to_char(last_login, '{Const.format_string_date}') like (@s2))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                        }

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = Convert.ToInt64(cmd.ExecuteScalar());
                        return total;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public PostResult PostUserCertRevoke(UserCertRevoke model)
        {
            PostResult pr = new PostResult();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"INSERT INTO UserCertRevoke (id,reason,date_input,date_update,id_user_info,id_ekyc,status) VALUES (@id,@reason, @date_input::timestamp, @date_update::timestamp, @id_user_info, @id_ekyc, @status)");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id", model.id);
                    cmd.Parameters.AddWithValue("reason", model.reason);
                    cmd.Parameters.AddWithValue("id_user_info", model.id_user_info);
                    cmd.Parameters.AddWithValue("id_ekyc", model.id_ekyc);
                    cmd.Parameters.AddWithValue("status", (int)model.status);
                    cmd.Parameters.AddWithValue("date_input", model.date_input.ToString(Const.format_string_date));
                    cmd.Parameters.AddWithValue("date_update", model.date_update.ToString(Const.format_string_date));
                    cmd.ExecuteNonQuery();
                    pr.message = "Success";
                    pr.result = true;
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

        public bool UpdateStatusUserCertRevoke(string email)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"UPDATE usercertrevoke as a SET status = 1, date_update = @date_update::timestamp from userlogininfo as b where a.id_user_info = b.id_user_info and b.user_name = @email and a.status = 0 ");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("email", kripto.Hashing(email));
                    cmd.Parameters.AddWithValue("date_update", DateTime.UtcNow.ToString(Const.format_string_date));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GetResultObject<List<UserCertRevoke>> GetUserCertRevoke(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserCertRevoke>> result = new GetResultObject<List<UserCertRevoke>>();
            try
            {
                List<UserCertRevoke> models = new List<UserCertRevoke>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT a.*, b.name, b.email_address, b.phone_number, c.value as status_value FROM usercertrevoke a left join userinfo b on a.id_user_info = b.id left join mstatuscertrevoke c on c.id = a.status");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" WHERE (b.name like (@s1) OR b.email_address like (@s2) OR b.phone_number like (@s3) OR a.reason like (@s4) OR to_char(a.date_input, '{Const.format_string_date}') like (@s5) OR to_char(a.date_update, '{Const.format_string_date}') like (@s6) OR c.value like (@s7))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s7", string.Format($"%{search}%"));
                        }

                        if (kolom == 1)
                        {
                            nama_kolom = "b.name";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "b.email_address";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "b.phone_number";
                        }
                        else if (kolom == 4)
                        {
                            nama_kolom = "a.reason";
                        }
                        else if (kolom == 5)
                        {
                            nama_kolom = "a.date_input";
                        }
                        else if (kolom == 6)
                        {
                            nama_kolom = "a.date_update";
                        }
                        else if (kolom == 7)
                        {
                            nama_kolom = "c.value";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");

                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));
                            string phone_number = kripto.Dekrip(Convert.ToString(_con.reader["phone_number"]));

                            UserInfo user_info = new UserInfo() { name = Convert.ToString(_con.reader["name"]), email_address = email_address, phone_number = phone_number };
                            UserCertRevoke ts = new UserCertRevoke() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), reason = Convert.ToString(_con.reader["reason"]), date_input = Convert.ToDateTime(_con.reader["date_input"]), date_update = Convert.ToDateTime(_con.reader["date_update"]), status_value = Convert.ToString(_con.reader["status_value"]), status = (StatusUserCertRevoke)Convert.ToInt32(_con.reader["status"]), user_info = user_info, id_ekyc = Guid.Parse(Convert.ToString(_con.reader["id_ekyc"])) };
                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserCertRevoke>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }
                       
                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserCertRevoke>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahCertRevoke(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT count(a.id) as jumlah FROM usercertrevoke a left join userinfo b on a.id_user_info = b.id left join mstatuscertrevoke c on c.id = b.status");

                        if (search != null)
                        {
                            query += string.Format($" WHERE (b.name like (@s1) OR b.email_address like (@s2) OR b.phone_number like (@s3) OR a.reason like (@s4) OR to_char(a.date_input, '{Const.format_string_date}') OR to_char(a.date_update, '{Const.format_string_date}') like (@s5) OR c.value like (@s6))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s4", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                        }

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = Convert.ToInt64(cmd.ExecuteScalar());
                        return total;
                    } 
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private bool IsExistUserValidToken(string user_name)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"SELECT user_name FROM UserValidToken Where user_name =@user_name");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("user_name", kripto.Hashing(user_name));
                    string result = (string)cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ChangeIsActiveUserValidToken(string user_name)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"Update UserValidToken SET is_active = false Where user_name =@user_name;");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("user_name", kripto.Hashing(user_name));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool PostUserValidToken(UserValidToken model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format("");
                        if (IsExistUserValidToken(model.user_name))
                        {
                            query += string.Format($"Update UserValidToken SET valid_token=@valid_token, create_date = @create_date::timestamp, active_date=@active_date::timestamp, is_active = @is_active Where user_name =@user_name;");

                            cmd.Parameters.AddWithValue("valid_token", model.valid_token.TrimEnd('\0'));
                            cmd.Parameters.AddWithValue("is_active", model.is_active);
                            cmd.Parameters.AddWithValue("user_name", kripto.Hashing(model.user_name));
                            cmd.Parameters.AddWithValue("create_date", model.create_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("active_date", model.active_date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }
                        else
                        {
                            query += string.Format($"INSERT INTO UserValidToken (id,user_name,valid_token,create_date,active_date,is_active) VALUES (@id,@user_name, @valid_token, @create_date::timestamp, @active_date::timestamp, @is_active);");

                            cmd.Parameters.AddWithValue("valid_token", model.valid_token.TrimEnd('\0'));
                            cmd.Parameters.AddWithValue("is_active", model.is_active);
                            cmd.Parameters.AddWithValue("user_name", kripto.Hashing(model.user_name));
                            cmd.Parameters.AddWithValue("id", model.id);
                            cmd.Parameters.AddWithValue("create_date", model.create_date.ToString(Const.format_string_date));
                            cmd.Parameters.AddWithValue("active_date", model.active_date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }

                        query += string.Format($"");
                        cmd.CommandText = query;
                        cmd.Connection = a;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                        
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public GetResultObject<UserValidToken> GetUserValidToken(string user_name)
        {
            GetResultObject<UserValidToken> datas = new GetResultObject<UserValidToken>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    UserValidToken model = new UserValidToken();
                    string query = string.Format($"SELECT valid_token, active_date, is_active FROM UserValidToken where user_name = @user_name");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("user_name", kripto.Hashing(user_name));
                    _con.reader = cmd.ExecuteReader();

                    while (_con.reader.Read())
                    {
                        model = new UserValidToken() { valid_token = Convert.ToString(_con.reader["valid_token"]), active_date = Convert.ToDateTime(_con.reader["active_date"]), is_active = Convert.ToBoolean(_con.reader["is_active"]) };
                    }
                    _con.reader.Close();
                    datas.objek = model;
                    datas.result = true;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public bool PostUserPayment(TransactionDetail model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"INSERT INTO userpayment(order_id,id_user_info,gross_amount,input_date,update_date,status) values(@order_id,@id_user_info,@gross_amount,@input_date::timestamp,@update_date::timestamp,@status)");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("order_id", Guid.Parse(model.order_id));
                    cmd.Parameters.AddWithValue("id_user_info", model.id_user_info);
                    cmd.Parameters.AddWithValue("gross_amount", model.gross_amount);
                    cmd.Parameters.AddWithValue("input_date", model.input_date.ToString(Const.format_string_date));
                    cmd.Parameters.AddWithValue("update_date", model.update_date.ToString(Const.format_string_date));
                    cmd.Parameters.AddWithValue("status", model.status);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateStatusUserPayment(TransactionDetail model)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"UPDATE userpayment set status=@status, update_date=@update_date::timestamp where order_id=@order_id");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("order_id", Guid.Parse(model.order_id));
                    cmd.Parameters.AddWithValue("update_date", model.update_date.ToString(Const.format_string_date));
                    cmd.Parameters.AddWithValue("status", model.status);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public GetResultObject<TransactionDetail> GetUserPayment(Guid order_id)
        {
            GetResultObject<TransactionDetail> datas = new GetResultObject<TransactionDetail>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    TransactionDetail model = new TransactionDetail();
                    string query = string.Format($"SELECT * FROM userpayment where order_id = @order_id");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("order_id", order_id);
                    _con.reader = cmd.ExecuteReader();

                    while (_con.reader.Read())
                    {
                        model = new TransactionDetail() { order_id = Convert.ToString(_con.reader["order_id"]), gross_amount = Convert.ToInt64(_con.reader["gross_amount"]), id_user_info = Guid.Parse(Convert.ToString(_con.reader["id_user_info"])), input_date = Convert.ToDateTime(_con.reader["input_date"]), status = Convert.ToString(_con.reader["status"]), update_date = Convert.ToDateTime(_con.reader["update_date"]) };
                    }
                    _con.reader.Close();
                    datas.objek = model;
                    datas.result = true;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public bool PostPrivateKey(string private_key)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"INSERT INTO pk(value) values(@private_key)");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("private_key", private_key);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GetResultObject<string> GetPrivateKey()
        {
            GetResultObject<string> datas = new GetResultObject<string>();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"SELECT * FROM pk");
                    var cmd = new NpgsqlCommand(query, a);
                    _con.reader = cmd.ExecuteReader();
                    string pk = string.Empty;
                    while (_con.reader.Read())
                    {
                        pk = Convert.ToString(_con.reader["value"]);
                    }
                    _con.reader.Close();
                    datas.objek = pk;
                    datas.result = true;
                    return datas;
                }
            }
            catch (Exception ex)
            {
                datas.result = false;
                datas.message = ex.Message.ToString();
                return datas;
            }
        }

        public PostResult PostUserCertRekeying(UserCertRevoke model)
        {
            PostResult pr = new PostResult();
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"INSERT INTO UserCertRekeying(id,date_input,date_update,id_user_info,id_ekyc,status) VALUES (@id, @date_input::timestamp, @date_update::timestamp, @id_user_info, @id_ekyc, @status)");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("id", model.id);
                    cmd.Parameters.AddWithValue("id_user_info", model.id_user_info);
                    cmd.Parameters.AddWithValue("id_ekyc", model.id_ekyc);
                    cmd.Parameters.AddWithValue("status", (int)model.status);
                    cmd.Parameters.AddWithValue("date_input", model.date_input.ToString(Const.format_string_date));
                    cmd.Parameters.AddWithValue("date_update", model.date_update.ToString(Const.format_string_date));
                    cmd.ExecuteNonQuery();
                    pr.message = "Success";
                    pr.result = true;
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

        public bool UpdateStatusUserCertRekeying(string email)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    string query = string.Format($"UPDATE UserCertRekeying as a SET status = 1, date_update = @date_update::timestamp from userlogininfo as b where a.id_user_info = b.id_user_info and b.user_name = @email and a.status = 0 ");
                    var cmd = new NpgsqlCommand(query, a);
                    cmd.Parameters.AddWithValue("email", kripto.Hashing(email));
                    cmd.Parameters.AddWithValue("date_update", DateTime.UtcNow.ToString(Const.format_string_date));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GetResultObject<List<UserCertRevoke>> GetUserCertRekeying(int start, int length, int kolom, string order_by, string search)
        {
            GetResultObject<List<UserCertRevoke>> result = new GetResultObject<List<UserCertRevoke>>();
            try
            {
                List<UserCertRevoke> models = new List<UserCertRevoke>();
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT a.*, b.name, b.email_address, b.phone_number, c.value as status_value FROM UserCertRekeying a left join userinfo b on a.id_user_info = b.id left join mstatuscertrevoke c on c.id = a.status");

                        string nama_kolom = string.Empty;

                        if (search != null)
                        {
                            query += string.Format($" WHERE (b.name like (@s1) OR b.email_address like (@s2) OR b.phone_number like (@s3) OR to_char(a.date_input, '{Const.format_string_date}') like (@s5) OR to_char(a.date_update, '{Const.format_string_date}') like (@s6) OR c.value like (@s7))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s7", string.Format($"%{search}%"));
                        }

                        if (kolom == 1)
                        {
                            nama_kolom = "b.name";
                        }
                        else if (kolom == 2)
                        {
                            nama_kolom = "b.email_address";
                        }
                        else if (kolom == 3)
                        {
                            nama_kolom = "b.phone_number";
                        }
                        else if (kolom == 4)
                        {
                            nama_kolom = "a.date_input";
                        }
                        else if (kolom == 5)
                        {
                            nama_kolom = "a.date_update";
                        }
                        else if (kolom == 6)
                        {
                            nama_kolom = "c.value";
                        }

                        query += string.Format($" order by {nama_kolom} {order_by} offset @start rows fetch next @length rows only");

                        cmd.Parameters.AddWithValue("start", start);
                        cmd.Parameters.AddWithValue("length", length);

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        _con.reader = cmd.ExecuteReader();

                        while (_con.reader.Read())
                        {
                            string email_address = kripto.Dekrip(Convert.ToString(_con.reader["email_address"]));
                            string phone_number = kripto.Dekrip(Convert.ToString(_con.reader["phone_number"]));

                            UserInfo user_info = new UserInfo() { name = Convert.ToString(_con.reader["name"]), email_address = email_address, phone_number = phone_number };
                            UserCertRevoke ts = new UserCertRevoke() { id = Guid.Parse(Convert.ToString(_con.reader["id"])), date_input = Convert.ToDateTime(_con.reader["date_input"]), date_update = Convert.ToDateTime(_con.reader["date_update"]), status_value = Convert.ToString(_con.reader["status_value"]), status = (StatusUserCertRevoke)Convert.ToInt32(_con.reader["status"]), user_info = user_info, id_ekyc = Guid.Parse(Convert.ToString(_con.reader["id_ekyc"])), id_user_info = Guid.Parse(Convert.ToString(_con.reader["id_user_info"])) };
                            models.Add(ts);
                        }

                        result = new GetResultObject<List<UserCertRevoke>>() { objek = models, result = true };
                        _con.reader.Close();
                        return result;
                    }

                }
            }
            catch (Exception ex)
            {
                result = new GetResultObject<List<UserCertRevoke>>() { result = false, message = ex.Message.ToString() };
                return result;
            }
        }

        public long GetJumlahCertRekeying(string search)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand())
                    {
                        string query = string.Format($"SELECT count(a.id) as jumlah FROM usercertrevoke a left join userinfo b on a.id_user_info = b.id left join mstatuscertrevoke c on c.id = b.status");

                        if (search != null)
                        {
                            query += string.Format($" WHERE (b.name like (@s1) OR b.email_address like (@s2) OR b.phone_number like (@s3) OR to_char(a.date_input, '{Const.format_string_date}') like (@s5) OR to_char(a.date_update, '{Const.format_string_date}') like (@s6) OR c.value like (@s7))");

                            cmd.Parameters.AddWithValue("s1", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s2", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s3", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s5", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s6", string.Format($"%{search}%"));
                            cmd.Parameters.AddWithValue("s7", string.Format($"%{search}%"));
                        }

                        cmd.CommandText = query;
                        cmd.Connection = a;
                        long total = Convert.ToInt64(cmd.ExecuteScalar());
                        return total;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool IsBadanUsaha(Guid id_user_info)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {

                    string query = "SELECT nama FROM BadanUsaha Where id_user_info =@p1";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, a))
                    {
                        cmd.Parameters.AddWithValue("p1", id_user_info);
                        string result = (string)cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsNonActive(Guid id_user_info)
        {
            try
            {
                using (NpgsqlConnection a = _con.Connect())
                {

                    string query = "SELECT name FROM UserInfo Where id =@p1 and status = @status";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, a))
                    {
                        cmd.Parameters.AddWithValue("p1", id_user_info);
                        cmd.Parameters.AddWithValue("status", (int)StatusUserInfo.non_active);
                        string result = (string)cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
