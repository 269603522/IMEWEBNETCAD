using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IMEWebCAD.Common.Encrypt;
using IMEWebCAD.Common.Text;
using IMEWebCAD.DAL;
using MySql.Data.MySqlClient;

namespace Business
{
    public class CADOperator
    {
        public string addCADFileHandleLog(string fileJavaID, string fTPPath, string fileName, string useCode, string fileURL, string state, string thumbnail, string referer, string ip, string isCopy)
        {
            string strResult = "";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            string sqlStr = "proc_addCADFileHandleLog ";
            string strCreateTime = DateTime.Now.ToString();
            string fileCode = MD5Tool.MD5Encrypt16(DateTime.Now.Ticks + "codeCad" + fTPPath);
            Dictionary<string, string> myDic = new Dictionary<string, string>();
            myDic.Add("inFileJavaID", fileJavaID);
            myDic.Add("FileCode", fileCode);
            myDic.Add("FTPPath", fTPPath);
            myDic.Add("FileName", fileName);
            myDic.Add("UseCode", useCode);
            myDic.Add("FileURL", fileURL);
            myDic.Add("State", state);
            myDic.Add("Thumbnail", thumbnail);
            myDic.Add("Referer", referer);
            myDic.Add("IP", ip);
            myDic.Add("isCopy", isCopy);
            DataSet ds = objDb.executionProc(sqlStr, myDic);
            if (ds!=null&& ds.Tables.Count>0&& ds.Tables[0].Rows[0][0].ToString()=="done")
            {
                strResult = fileCode;
            }
            return strResult;
        }
        public string getCADFileHandleLogByFileCode(string fileCodes)
        {
            string strResult = "";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            if (fileCodes.IndexOf(',') > 0)
            {
                fileCodes = Regex.Replace(fileCodes, ",", "','");
                fileCodes = "'" + fileCodes + "'";
            }
            MySqlParameter[] paramters = new MySqlParameter[] {
                new MySqlParameter("FileCode",fileCodes)
            };
            string sql = "";
            if (fileCodes.IndexOf(",") > 0)
            {
                sql = "select * from t_cadfileshandlelog where FileCode in (" + fileCodes + ")";
            }
            else
            {
                sql = "select * from t_cadfileshandlelog where FileCode =@FileCode";
            }
            DataSet ds = objDb.getDataSet(sql, paramters);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strResult = "";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        strResult += ds.Tables[0].Rows[i]["FileURL"].ToString();
                        strResult += StringTool.strSubCut + ds.Tables[0].Rows[i]["FileJavaID"].ToString();
                        strResult += StringTool.strSubCut + ds.Tables[0].Rows[i]["Thumbnail"].ToString();
                        strResult += StringTool.strCut;
                    }

                }
                if (strResult.IndexOf(StringTool.strCut) > 0)
                {
                    strResult = strResult.Substring(0, strResult.Length - StringTool.strCut.Length);
                }
            }
            return strResult;
        }
        public string getCADFileHandleLogByJavaCode(string fileCodes)
        {
            string strResult = "";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            if (fileCodes.IndexOf(',') > 0)
            {
                fileCodes = Regex.Replace(fileCodes, ",", "','");
                fileCodes = "'" + fileCodes + "'";
            }
            MySqlParameter[] paramters = new MySqlParameter[] {
                new MySqlParameter("FileCode",fileCodes)
            };
            string sql = "";
            if (fileCodes.IndexOf(",") > 0)
            {
                sql = "select * from t_cadfileshandlelog where FileJavaId in (" + fileCodes + ")";
            }
            else
            {
                sql = "select * from t_cadfileshandlelog where FileJavaId =@FileCode";
            }
            DataSet ds = objDb.getDataSet(sql, paramters);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strResult = "";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        strResult += ds.Tables[0].Rows[i]["FileURL"].ToString();
                        strResult += StringTool.strSubCut + ds.Tables[0].Rows[i]["State"].ToString();
                        strResult += StringTool.strSubCut + ds.Tables[0].Rows[i]["Thumbnail"].ToString();
                        strResult += StringTool.strCut;
                    }

                }
                if (strResult.IndexOf(StringTool.strCut) > 0)
                {
                    strResult = strResult.Substring(0, strResult.Length - StringTool.strCut.Length);
                }
            }
            return strResult;
        }
        public string CreateReading(string inHandleCodes, string inCreateType, string inInnerOrOut, string inFileURL, string inThumbnail, string inCreaterCode, string inCreaterReferer, string inCreaterIP, string inUserAgent = "")
        {
            string strResult = "失败请求！";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            string sqlStr = "INSERT INTO t_cadfilesReadingLog (ReadingCode, HandleCode, CreateType, InnerOrOut, FileURL, Thumbnail, CreaterCode, CreaterReferer, CreaterIP, CreateTime,UserAgent) ";
            sqlStr += " VALUES ";
            sqlStr += " (@ReadingCode, @HandleCode, @CreateType, @InnerOrOut, @FileURL, @Thumbnail, @CreaterCode, @CreaterReferer, @CreaterIP, @CreateTime,@UserAgent) ";
            string strCreateTime = DateTime.Now.ToString();
            string readingCode = MD5Tool.MD5Encrypt32(DateTime.Now.Ticks + "codeReadCad" + inHandleCodes);
            MySqlParameter[] paramters = new MySqlParameter[] {
                new MySqlParameter("ReadingCode",readingCode)
                ,new MySqlParameter("HandleCode",inHandleCodes)
                ,new MySqlParameter("CreateType",inCreateType)
                ,new MySqlParameter("InnerOrOut",inInnerOrOut)
                ,new MySqlParameter("FileURL",inFileURL)
                ,new MySqlParameter("Thumbnail",inThumbnail)
                ,new MySqlParameter("CreaterCode",inCreaterCode)
                ,new MySqlParameter("CreaterReferer",inCreaterReferer)
                ,new MySqlParameter("CreaterIP",inCreaterIP)
                 ,new MySqlParameter("CreateTime",strCreateTime)
                  ,new MySqlParameter("UserAgent",inUserAgent)
            };
            int i = objDb.doExecute(sqlStr, paramters);
            if (i > 0)
            {
                strResult = readingCode;
            }
            return strResult;
        }
        public string getReadingByCode(string inReadingCode)
        {
            string strResult = "";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            MySqlParameter[] paramters = new MySqlParameter[] {
                new MySqlParameter("ReadingCode",inReadingCode)
            };
            string sql = "";
            sql = " select `FileURL`,  `CreateTime`, `ReadingCount` from t_cadfilesreadinglog where ReadingCode=@ReadingCode";
            DataSet ds = objDb.getDataSet(sql, paramters);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    strResult = "";
                    strResult += ds.Tables[0].Rows[0]["FileURL"].ToString();
                    strResult += StringTool.strSubCut + ds.Tables[0].Rows[0]["CreateTime"].ToString();
                    strResult += StringTool.strSubCut + ds.Tables[0].Rows[0]["ReadingCount"].ToString();
                }
                if (strResult.IndexOf(StringTool.strCut) > 0)
                {
                    strResult = strResult.Substring(0, strResult.Length - StringTool.strCut.Length);
                }
            }
            return strResult;
        }
        public string doReadingByCode(string inReadingCode, string inReaderIP, string inReaderReferer, string inReaderCode, string inUserAgent = "")
        {
            string strResult = "失败请求！";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            string sqlStr = "update t_cadfilesreadinglog "; 
            sqlStr += " set ReaderIP=@ReaderIP ";
            sqlStr += " , ReadingCount=ifnull(ReadingCount,0)+1 ";
            sqlStr += " , ReaderCode=@ReaderCode ";
            sqlStr += " , LastReadingTime=now() ";
            sqlStr += " , ReaderReferer=@ReaderReferer ";
            sqlStr += " , UserAgent=@UserAgent ";
            
            sqlStr += " where ReadingCode=@ReadingCode ";
            MySqlParameter[] paramters = new MySqlParameter[] {
                new MySqlParameter("ReaderIP",inReaderIP)
                ,new MySqlParameter("ReaderCode",inReaderCode)
                ,new MySqlParameter("ReaderReferer",inReaderReferer)
                ,new MySqlParameter("ReadingCode",inReadingCode)
                ,new MySqlParameter("UserAgent",inUserAgent)
            };
            int i = objDb.doExecute(sqlStr, paramters);
            if (i > 0)
            {
                strResult = "追加阅读记录成功";
            }
            return strResult;
        }
        public string getHandleList(string _pagecurrent, string _pagesize, string _order)
        {
            string strResult = "";
            strResult=DB.getDataList("",_pagecurrent, _pagesize, "*", "t_cadfileshandlelog", _order);
            return strResult;
        }
        public string getReadingList(string _draw,string _pagecurrent, string _pagesize, string _order,string strKey="")
        {
            string strResult = "";
            string where = "t_cadfilesreadinglog where 1=1  and (FileURL like '%"+ strKey + "%' or ReadingCode like '%" + strKey + "%' or CreaterIP like '%" + strKey + "%' or UserAgent like '%" + strKey + "%' or HandleCode like '%" + strKey + "%')  ";
            strResult = DB.getDataList( _draw,_pagecurrent, _pagesize, "*", where, _order, "json_with_count_head");
            return strResult;
        }
        public string deleteReadingByCode(string _ReadingCode)
        {
            string strResult = "";
            MySqlParameter[] paramters = new MySqlParameter[] {
                new MySqlParameter("ReadingCode",_ReadingCode)
            };
            string sql = "";
            sql = "delete from t_cadfilesreadinglog where ReadingCode =@ReadingCode";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            DataSet ds = objDb.getDataSet(sql, paramters);
            if(ds!=null)
            {
                strResult = "删除成功！";
            }
            return strResult;
        }
        public string deleteHandleByCode(string _FileCode)
        {
            string strResult = "";
            MySqlParameter[] paramters = new MySqlParameter[] {
                new MySqlParameter("FileCode",_FileCode)
            };
            string sql = "";
            sql = "delete from t_cadfileshandlelog where FileCode =@FileCode";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            DB objDb = new DB(connetStr);
            DataSet ds = objDb.getDataSet(sql, paramters);
            if (ds != null)
            {
                strResult = "删除成功！";
            }
            return strResult;
        }
    }
}
