using Api.Repo;
using Api.Share.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static Api.Share.Tools.Result;

namespace UnitTest
{
    [TestClass]
    public class UserDbTest
    {
        private readonly UserDb _udb = new UserDb();
        private readonly string user_name = "azazaza3";
        private readonly string token = "123";
        [TestMethod]
        public void Post()
        {
            UserInfo b = new UserInfo() {id = Guid.NewGuid(), id_ekyc = Guid.NewGuid(), email_address = "r@gmail.com", id_subscriber = Guid.NewGuid(), input_date = DateTime.UtcNow, license = "test", name = "test", payment = TypePayment.transfer, phone_number = "09", status = StatusUserInfo.pending, update_date = DateTime.UtcNow };
            UserLoginInfo a = new UserLoginInfo() {id = Guid.NewGuid(), active_date = DateTime.UtcNow, change_password_date = DateTime.UtcNow, id_subscriber = b.id_subscriber, id_user_info = b.id, input_date = DateTime.UtcNow, is_active = true, login_type = LoginType.User, password_login = "a", update_date = DateTime.UtcNow, user_name = user_name };

            UserFromEkyc c = new UserFromEkyc() {id = Guid.NewGuid(), id_ekyc = Guid.NewGuid(), id_user_info = b.id };
            UserPackage d = new UserPackage() { expired_date = DateTime.UtcNow, id = Guid.NewGuid(), id_user_info = b.id, package = TypePackage.enkripa_ekyc };
            UserPackage e = new UserPackage() { expired_date = DateTime.UtcNow, id = Guid.NewGuid(), id_user_info = b.id, package = TypePackage.enkripa_sign };
            List<UserPackage> list_user_package = new List<UserPackage>();
            list_user_package.Add(d);
            list_user_package.Add(e);

            User model = new User() { user_info = b, user_login_info = a, user_from_ekyc = c, list_user_package = list_user_package };
            PostResult result = _udb.Post(model);

            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void PostUserLoginDatas()
        {
            UserLoginDatas model = new UserLoginDatas() {expiry = DateTime.UtcNow, id = Guid.NewGuid(), last_login = DateTime.UtcNow, token = token, user_name = user_name };
            bool result = _udb.PostUserLoginDatas(model);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetUserLogin()
        {
            bool result = _udb.GetUserLogin(user_name, token);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetUserLoginInfo()
        {
            var result = _udb.GetUserLoginInfo(user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void Get()
        {
            var result = _udb.Get(0, 10, 1, "asc", user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlah()
        {
            var result = _udb.GetJumlah(user_name);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void UpdateStatus()
        {
            UserInfo b = new UserInfo() { id = Guid.NewGuid(), id_ekyc = Guid.NewGuid(), email_address = "r@gmail.com", id_subscriber = Guid.NewGuid(), input_date = DateTime.UtcNow, license = "test", name = "test", payment = TypePayment.transfer, phone_number = "09", status = StatusUserInfo.pending, update_date = DateTime.UtcNow };
            UserLoginInfo a = new UserLoginInfo() { id = Guid.NewGuid(), active_date = DateTime.UtcNow, change_password_date = DateTime.UtcNow, id_subscriber = b.id_subscriber, id_user_info = b.id, input_date = DateTime.UtcNow, is_active = true, login_type = LoginType.User, password_login = "a", update_date = DateTime.UtcNow, user_name = user_name };

            UserFromEkyc c = new UserFromEkyc() { id = Guid.NewGuid(), id_ekyc = Guid.NewGuid(), id_user_info = b.id };
            UserPackage d = new UserPackage() { expired_date = DateTime.UtcNow, id = Guid.NewGuid(), id_user_info = b.id, package = TypePackage.enkripa_ekyc };
            UserPackage e = new UserPackage() { expired_date = DateTime.UtcNow, id = Guid.NewGuid(), id_user_info = b.id, package = TypePackage.enkripa_sign };
            List<UserPackage> list_user_package = new List<UserPackage>();
            list_user_package.Add(d);
            list_user_package.Add(e);

            User model = new User() { user_info = b, user_login_info = a, user_from_ekyc = c, list_user_package = list_user_package };
            var result = _udb.UpdateStatus(model);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetUser()
        {
            var result = _udb.GetUser(Guid.NewGuid());
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void PostUserLog()
        {
            UserLog log = new UserLog() {id = Guid.NewGuid(), activity = "Test", apps = "EnkripaSign", status = "A", status_log = "B", time_input = DateTime.UtcNow, user_name = user_name };
            var result = _udb.PostUserLog(log);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void UpdateUserLogStatus()
        {
            UserLog log = new UserLog() { id = Guid.NewGuid(), activity = "Test", apps = "EnkripaSign", status = "A", status_log = Guid.NewGuid().ToString(), time_input = DateTime.UtcNow, user_name = user_name };
            var result = _udb.UpdateUserLogStatus(log);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetLog()
        {
            var result = _udb.GetLog(0, 10, 1, "asc", user_name, null, null);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlahLog()
        {
            var result = _udb.GetJumlahLog(user_name, null, null);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void Update()
        {
            UserInfo user_info = new UserInfo() {email_address = user_name, id = Guid.NewGuid(), id_ekyc = Guid.NewGuid(), phone_number = "a", update_date = DateTime.UtcNow, name = "aza" };
            var result = _udb.Update(user_info);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void UpdatePassword()
        {
            UserLoginInfo a = new UserLoginInfo() { id = Guid.NewGuid(), active_date = DateTime.UtcNow, change_password_date = DateTime.UtcNow, input_date = DateTime.UtcNow, is_active = true, login_type = LoginType.User, password_login = "a", update_date = DateTime.UtcNow, user_name = user_name };
            var result = _udb.UpdatePassword(a);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void UpdatePasswordForgot()
        {
            UserLoginInfo a = new UserLoginInfo() { id = Guid.NewGuid(), active_date = DateTime.UtcNow, change_password_date = DateTime.UtcNow, input_date = DateTime.UtcNow, is_active = true, login_type = LoginType.User, password_login = "a", update_date = DateTime.UtcNow, user_name = user_name };
            var result = _udb.UpdatePasswordForgot(a);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void PostUserAccess()
        {
            UserAccess user_access = new UserAccess() {id = Guid.NewGuid(), date_update = DateTime.UtcNow, id_user_info = Guid.NewGuid(), id_user_package = Guid.Parse("0d5de563-bc4d-45c9-beb9-9d4ab1de5965"), package = TypePackage.enkripa_sign, sum_access = 1, user_data_nama = user_name, user_package = "EnkripaSign"};
            var result = _udb.PostUserAccess(user_access);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void MinUserAccess()
        {
            UserAccess user_access = new UserAccess() { id = Guid.NewGuid(), date_update = DateTime.UtcNow, id_user_info = Guid.NewGuid(), id_user_package = Guid.NewGuid(), package = TypePackage.enkripa_sign, sum_access = 1, user_data_nama = user_name, user_package = "EnkripaSign" };
            var result = _udb.MinUserAccess(user_access);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetUserAccessByIdUserPackage()
        {
            var result = _udb.GetUserAccessByIdUserPackage(Guid.NewGuid(), TypePackage.enkripa_sign);
            Assert.AreNotEqual(result, null);
        }

        [TestMethod]
        public void GetUserInfoAccess()
        {
            var result = _udb.GetUserInfoAccess(Guid.NewGuid(), TypePackage.enkripa_sign);
            Assert.AreNotEqual(result, null);
        }

        [TestMethod]
        public void GetUserAccess()
        {
            var result = _udb.GetUserAccess(0, 10, 1, "asc", user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlahTotalUserAccess()
        {
            var result = _udb.GetJumlahTotalUserAccess(user_name);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void GetUserAccessHt()
        {
            var result = _udb.GetUserAccessHt(0, 10, 1, "asc", user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlahTotalUserAccessHt()
        {
            var result = _udb.GetJumlahTotalUserAccessHt(user_name);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void GetAdmin()
        {
            var result = _udb.GetAdmin(0, 10, 1, "asc", user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlahAdmin()
        {
            var result = _udb.GetJumlahAdmin(user_name);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void PostAdmin()
        {
            UserInfo b = new UserInfo() { id = Guid.NewGuid(), id_ekyc = Guid.NewGuid(), email_address = "client_register@enkripa.id", id_subscriber = Guid.NewGuid(), input_date = DateTime.UtcNow, license = "admin", name = "Client Register", payment = TypePayment.transfer, phone_number = "021", status = StatusUserInfo.active, update_date = DateTime.UtcNow, nik = "111" };
            UserLoginInfo a = new UserLoginInfo() { id = Guid.NewGuid(), active_date = DateTime.UtcNow.AddYears(10), change_password_date = DateTime.UtcNow, id_subscriber = b.id_subscriber, id_user_info = b.id, input_date = DateTime.UtcNow, is_active = true, login_type = LoginType.statis, password_login = "$7$C6..../....WnvdyCfI.HdJdbLNQfQoH3r1wSe7lUwCcoGpKe6lD2B$jqRoS5aKs8DFKh6.ogHOSp7fR4DTqOej4DTxpUypaE0", update_date = DateTime.UtcNow, user_name = "client_register@enkripa.id" };

            UserFromEkyc c = new UserFromEkyc() { id = Guid.NewGuid(), id_ekyc = Guid.NewGuid(), id_user_info = b.id };
            UserPackage d = new UserPackage() { expired_date = DateTime.UtcNow, id = Guid.NewGuid(), id_user_info = b.id, package = TypePackage.enkripa_ekyc };
            UserPackage e = new UserPackage() { expired_date = DateTime.UtcNow, id = Guid.NewGuid(), id_user_info = b.id, package = TypePackage.enkripa_sign };
            List<UserPackage> list_user_package = new List<UserPackage>();
            list_user_package.Add(d);
            list_user_package.Add(e);

            User model = new User() { user_info = b, user_login_info = a, user_from_ekyc = c, list_user_package = list_user_package };

            var result = _udb.PostAdmin(model);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void DeleteUserAdmin()
        {
            var result = _udb.DeleteUserAdmin(Guid.NewGuid());
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetSumStatus()
        {
            var result = _udb.GetSumStatus();
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetSumWeeks()
        {
            var result = _udb.GetSumWeeks();
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetLoginError()
        {
            var result = _udb.GetLoginError(user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void PostLoginError()
        {
            var result = _udb.PostLoginError(1, user_name);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void PostUserLogAdmin()
        {
            UserLogAdmin log = new UserLogAdmin() {id = Guid.NewGuid(), user_name = user_name, activity = "", status = "", time_input = DateTime.UtcNow };
            var result = _udb.PostUserLogAdmin(log);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetLogAdmin()
        {
            var result = _udb.GetLogAdmin(0, 10, 1, "asc", user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlahLogAdmin()
        {
            var result = _udb.GetJumlahLogAdmin(user_name);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void GetUserLastLogin()
        {
            var result = _udb.GetUserLastLogin(0, 10, 1, "asc", user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlahLastLogin()
        {
            var result = _udb.GetJumlahLastLogin(user_name);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void PostUserCertRevoke()
        {
            UserCertRevoke user = new UserCertRevoke() {id = Guid.NewGuid(), date_input = DateTime.UtcNow, date_update = DateTime.UtcNow, id_ekyc = Guid.NewGuid(), id_user_info = Guid.Parse("b5f60979-ad5b-4f9a-80f6-9a8a8b009b86"), reason = "", status = StatusUserCertRevoke.req_revoke, status_value = ""};
            var result = _udb.PostUserCertRevoke(user);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetUserCertRevoke()
        {
            var result = _udb.GetUserCertRevoke(0, 10, 1, "asc", user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void GetJumlahCertRevoke()
        {
            var result = _udb.GetJumlahCertRevoke(user_name);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void ChangeIsActiveUserValidToken()
        {
            var result = _udb.ChangeIsActiveUserValidToken(user_name);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void PostUserValidToken()
        {
            UserValidToken a = new UserValidToken() {active_date = DateTime.UtcNow, create_date = DateTime.UtcNow, id = Guid.NewGuid(), is_active = true, type_validate = TypeValidate.client, user_name = user_name, valid_token = token  };
            var result = _udb.PostUserValidToken(a);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetUserValidToken()
        {
            var result = _udb.GetUserValidToken(user_name);
            Assert.AreEqual(result.result, true);
        }

        [TestMethod]
        public void CheckIsBadanUsaha()
        {
            var result = _udb.IsBadanUsaha(Guid.Parse("46fc2940-864d-443b-91ba-a24efee002ed"));
            Assert.AreEqual(result, true);
        }
    }
}
