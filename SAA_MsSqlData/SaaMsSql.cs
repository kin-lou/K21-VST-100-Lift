using SAA.Common.Database.MsSql;
using SAA.Common.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MsSqlData
{
    public class SaaMsSql
    {
        private IDatabase Readdb = new MsSql();
        private IDatabase Writedb = new MsSql();
        private SqlRes Readres = new SqlRes();
        private SqlRes Writeres = new SqlRes();
        private string SaaDataBase = string.Empty;
        private string SaaIp = string.Empty;
        private string SaaUserId = string.Empty;
        private string SaaPassword = string.Empty;
        /// <summary>
        /// 自動開啟連線,執行SQL命令,關閉連線
        /// </summary>
        private readonly object _Sqlwrite = new object();

        /// <summary>
        /// 自動開啟連線,執行SQL查詢命令,關閉連線
        /// </summary>
        private readonly object _Sqlread = new object();

        public SaaMsSql(string database, string ip, string userid, string password)
        {
            SaaDataBase = database;
            SaaIp = ip;
            SaaUserId = userid;
            SaaPassword = password;
        }

        /// <summary>
        /// 連線MsSql資料庫
        /// </summary>
        /// <returns></returns>
        public bool SasReadDatatBaseConnect()
        {
            try
            {
                Readres = Readdb.OpenDB(SaaIp, SaaDataBase, SaaUserId, SaaPassword);
                return Readres.IsSuccessed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 斷開MsSql資料庫
        /// </summary>
        /// <returns></returns>
        public bool SasReadDatatBaseClose()
        {
            try
            {
                Readres = Readdb.CloseDB();
                return Readres.IsSuccessed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                return false; ;
            }
        }

        /// <summary>
        /// 連線MsSql資料庫
        /// </summary>
        /// <returns></returns>
        public bool SasWriteDatatBaseConnect()
        {
            try
            {
                Writeres = Writedb.OpenDB(SaaIp, SaaDataBase, SaaUserId, SaaPassword);
                return Writeres.IsSuccessed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 斷開MsSql資料庫
        /// </summary>
        /// <returns></returns>
        public bool SasWriteDatatBaseClose()
        {
            try
            {
                Writeres = Writedb.CloseDB();
                return Writeres.IsSuccessed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                return false; ;
            }
        }

        /// <summary>
        /// 寫入SQL Server方法
        /// </summary>
        /// <param name="SqlCommand">SQL語法</param>
        public bool WriteSqlByAutoOpen(string SqlCommand)
        {
            try
            {
                lock (_Sqlwrite)
                {
                    if (SasWriteDatatBaseConnect())
                    {
                        Writeres = Writedb.ExecDB(SqlCommand);
                        SasWriteDatatBaseClose();
                        return Writeres.IsSuccessed;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 讀取SQL Server方法
        /// </summary>
        /// <param name="SqlCommand">SQL語法</param>
        /// <returns></returns>
        public DataTable? QuerySqlByAutoOpen(string SqlCommand)
        {
            try
            {
                lock (_Sqlread)
                {
                    if (SasReadDatatBaseConnect())
                    {
                        var datatable = new DataTable();
                        Readres = Readdb.ReadDB(SqlCommand, out datatable);
                        SasReadDatatBaseClose();
                        return datatable;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}-{ex.StackTrace}");
                return null;
            }
        }
    }
}
