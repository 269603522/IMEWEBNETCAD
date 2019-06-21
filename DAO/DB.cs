using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using IMEWebCAD.Common.Text;

namespace IMEWebCAD.DAL
{
    public class DB 
    {
        private string _connStr;

        public DB(string conn)
        {
            _connStr = conn;
        }
        public DB()
        {
            _connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        }
        public DataSet getSelect(string selectSql)
        {
            DataSet ds = null;
            ds = MySqlHelper.ExecuteDataset(_connStr, selectSql);
            return ds;
        }
        public int doExecute(string executeSql, MySqlParameter[] mySqlParameter)
        {
            int i = -1;
            i = MySqlHelper.ExecuteNonQuery(_connStr, executeSql, mySqlParameter);
            return i;
        }
        public DataSet getDataSet(string executeSql, MySqlParameter[] mySqlParameter)
        {
            DataSet ds = MySqlHelper.ExecuteDataset(_connStr, executeSql, mySqlParameter);
            return ds;
        }
        public DataSet executionProc(string procedureName, IDictionary<string, string> Parameters)
        {
            MySqlConnection myCon = new MySqlConnection(_connStr);
            MySqlConnection sqlConnection = myCon;

            MySqlCommand mysqlcom = new MySqlCommand(procedureName, sqlConnection);
            mysqlcom.CommandType = CommandType.StoredProcedure;//设置调用的类型为存储过程 
            DataSet ds = new DataSet();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            if (Parameters == null)
            {
                return null;
            }
            foreach (string k in Parameters.Keys)
            {
                mysqlcom.Parameters.Add(k, MySqlDbType.VarChar, 20).Value = Parameters[k];
            }

            sqlConnection.Open();//打开数据库连接
            adapter.SelectCommand = mysqlcom;
            adapter.Fill(ds, procedureName);
            mysqlcom.Dispose();
            sqlConnection.Close();
            sqlConnection.Dispose();
            return ds;

        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="_pagecurrent"></param>
        /// <param name="_pagesize"></param>
        /// <param name="_ifelse"></param>
        /// <param name="_where"></param>
        /// <param name="_order"></param>
        /// <param name="resultType">count_json|json_raw|json_with_count_head</param>
        /// <returns></returns>
        public static string getDataList(string draw ,string _pagecurrent, string _pagesize, string _ifelse, string _where, string _order,string resultType="count_json")
        {
            string strResult = "";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            string sqlStr = "proc_pPage ";
            Dictionary<string, string> myDic = new Dictionary<string, string>();
            myDic.Add("_pagecurrent", _pagecurrent);
            myDic.Add("_pagesize", _pagesize);
            myDic.Add("_ifelse", _ifelse);
            myDic.Add("_where", _where);
            myDic.Add("_order", _order);
            DataSet ds = objDb.executionProc(sqlStr, myDic);
            if (ds != null && ds.Tables.Count > 1)
            {
                if(resultType=="count_json")
                {
                    strResult = ds.Tables[1].Rows[0][0] + "|_CUT_|" + StringTool.DataTableToJsonWithStringBuilder(ds.Tables[0]);
                }
                else if (resultType == "json_raw")
                {
                    strResult = StringTool.DataTableToJsonWithStringBuilder(ds.Tables[0]);
                }
                else if(resultType == "json_with_count_head")
                {
                    string strData =  StringTool.DataTableToJsonWithStringBuilder(ds.Tables[0]);
                    if(strData=="")
                    {
                        strData = "[]";
                    }
                    strResult = "  { ";
                    strResult += "  \"draw\": " + draw + ", ";
                    strResult += "  \"recordsTotal\": "+ ds.Tables[1].Rows[0][0] + ", ";
                    strResult += "   \"recordsFiltered\": "+ ds.Tables[1].Rows[0][0] + ", ";
                    strResult += "   \"data\": " + strData;
                    strResult += "  } ";
                }
               
            }
            return strResult;
        }
    }
}
