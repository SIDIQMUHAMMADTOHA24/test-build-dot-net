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
    public class SignCron : IDisposable
    {
        private readonly ConnectionCron _con;
        public SignCron()
        {
            this._con = new ConnectionCron();
        }

        public void Dispose()
        {
        }

        public PostResult DeleteDoc()
        {
            PostResult pr = new PostResult();
            try
            {
                using (NpgsqlConnection a = _con.ConnectSign())
                {
                    string query = string.Format($"DELETE FROM DocsFlowData where idflow in (SELECT c.idflow from DocsInfo a left join docsflow b on a.id = b.iddocuments left join docsflowdata c on c.idflow = b.id where (DATE_PART('year', now()::date) - DATE_PART('year', a.uploadeddate::date)) * 12 + (DATE_PART('month', now()::date) - DATE_PART('month', a.uploadeddate::date)) >= 12); DELETE FROM DocsFlow where iddocuments in (SELECT id from DocsInfo where(DATE_PART('year', now()::date) - DATE_PART('year', uploadeddate::date)) *12 + (DATE_PART('month', now()::date) - DATE_PART('month', uploadeddate::date)) >= 12); DELETE FROM DocsFile where iddocuments in (SELECT id from DocsInfo where(DATE_PART('year', now()::date) - DATE_PART('year', uploadeddate::date)) *12 + (DATE_PART('month', now()::date) - DATE_PART('month', uploadeddate::date)) >= 12); DELETE FROM DocsKey where iddocuments in (SELECT id from DocsInfo where(DATE_PART('year', now()::date) -DATE_PART('year', uploadeddate::date)) *12 + (DATE_PART('month', now()::date) - DATE_PART('month', uploadeddate::date)) >= 12); DELETE FROM DocsLog where iddocument in (SELECT id from DocsInfo where(DATE_PART('year', now()::date) -DATE_PART('year', uploadeddate::date)) *12 + (DATE_PART('month', now()::date) - DATE_PART('month', uploadeddate::date)) >= 12); DELETE FROM Transaction where idfile in (SELECT id from DocsInfo where (DATE_PART('year', now()::date) - DATE_PART('year', uploadeddate::date)) * 12 + (DATE_PART('month', now()::date) - DATE_PART('month', uploadeddate::date)) >= 12); DELETE from DocsInfo where(DATE_PART('year', now()::date) -DATE_PART('year', uploadeddate::date)) *12 + (DATE_PART('month', now()::date) - DATE_PART('month', uploadeddate::date)) >= 12");
                    _con.command = new NpgsqlCommand(query, a);
                    _con.command.ExecuteNonQuery();

                    pr.result = true;
                    pr.message = "success delete document";
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
    }
}
