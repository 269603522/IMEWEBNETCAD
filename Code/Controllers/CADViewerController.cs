using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CADImport;
using CADImport.DWG;
using Code.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Xml;
using WebCAD;
using IMEWebCAD.Common.Encrypt;
using System.Configuration;
using IMEWebCAD.Common.Time;
using IMEWebCAD.DAL;
using Business;
using IMEWebCAD.Common.Text;
using System.Reflection;
using CADImport.RasterImage;
using System.Text;
using IMEWebCAD.Common.WebNet;
using Common.Encrypt;

namespace IMEWebCAD.Controllers
{
    public class CADViewerController : Controller
    {
        CADOperator cadOpt = new CADOperator();
        //https://192.168.255.103:2019/CADViewer/viewCAD/?code=ncqlg37ljqn98p7n2lvcyimq8dz7nfdoeenohse8tbpkw3npsxmxau7ikcdm41gz51d3jd1zsaisvclvprcgswdc1qmcg5jtjp44&HLHandle=160,162
        public ActionResult viewCAD(string code, string HLHandle = "", string token = "")
        {
            bool isPass = true;
            string strUrlReferrer = "";
            string strResult = "失败请求！";
            string strErrorInfo = "";
            if (token == null || token == "")
            {
                return RedirectToAction("Index", "Error", new { msg = "无效访问" });
            }
            else
            {
                if (Request.UrlReferrer != null)
                {
                    strUrlReferrer = Request.UrlReferrer.Host.ToString().ToLower();
                }
                #region 合法访问文件
                if (code != null)
                {
                    string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                    string TempCode = EncryptAndDecryptHelper.Locker.Decrypt(code, tempFileCode + "|View");
                    if (TempCode != null && TempCode != "")
                    {
                        string[] items = TempCode.Split('|');
                        if (items.Length > 0)
                        {

                            string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                            string strHandleInfo = cadOpt.getCADFileHandleLogByFileCode(items[0]);
                            if (strHandleInfo == null || strHandleInfo == "")
                            {
                                strErrorInfo = "查,无申请读取记录[77]";
                                isPass = false;
                                return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                            }
                            else
                            {
                                string[] strHandleInfoItems = StringTool.Split(strHandleInfo, StringTool.strSubCut);

                                if (strHandleInfoItems.Length > 1)
                                {
                                    #region 读取CAD文件主体
                                    string strFilePath = strHandleInfoItems[0];
                                    string strJavaID = strHandleInfoItems[1];
                                    #region 到java端检查令牌
                                    StringBuilder javaTokenCheckData = new StringBuilder();
                                    javaTokenCheckData.AppendFormat("\"adId\":\"" + strJavaID + "\",");
                                    javaTokenCheckData.AppendFormat("\"tokenType\":\"" + "1" + "\",");
                                    javaTokenCheckData.AppendFormat("\"token\":\"" + token + "\"");
                                    string data = "{" + javaTokenCheckData.ToString() + "}";
                                    string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                                    string dataJson = IMEAESTool.Encrypt(data, IMEJavaKey);
                                    string enterpriseNo = ConfigurationManager.AppSettings["enterpriseNo"].ToString();
                                    string javaTokenCheckURL = ConfigurationManager.AppSettings["javaTokenCheckURL"].ToString();
                                    string postData = "{\"enterpriseNo\":\"" + enterpriseNo + "\",\"data\":\"" + dataJson + "\"}";
                                    string checkResult = NetHelper.PostJsonWebRequest(javaTokenCheckURL, postData);
                                    dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(checkResult);
                                    string codeOfCheck = domDetailObj.code;
                                    string message = domDetailObj.message;
                                    if (codeOfCheck != "200")
                                    {
                                        return RedirectToAction("Index", "Error", new { msg = "令牌检查失败：" + message });
                                    }
                                    #endregion
                                    int intSeconts = TimeTool.DateDiffBySeconds(DateTime.Now, DateTime.Parse("2019-5-30 10:22"));
                                    if ((intSeconts > int.Parse(tempFileTimeoutNum) * 60) && false)
                                    {
                                        strResult += "<br/>超时了，时间差为" + intSeconts / 60 + "分";
                                        strErrorInfo += "起始时间: " + items[1];
                                        strErrorInfo += ",超时了，时间差为: " + intSeconts / 60 + "分";
                                        isPass = false;
                                    }
                                    else
                                    {
                                        string filePath = strFilePath;
                                        if (filePath != null && filePath.Length > 0 && System.IO.File.Exists(filePath))
                                        {
                                            strResult = "";
                                            var fileName = Path.GetFileName(filePath);
                                            string pathCADFileId = DrawingManager.Add(null, filePath).Id;
                                            strResult += "<br/>CAD文件插件ID:" + pathCADFileId;
                                            ViewBag.DrawingFile = filePath;
                                            ViewBag.DrawingID = DrawingManager.Add(null, ViewBag.DrawingFile).Id;
                                            if (HLHandle != "")
                                            {
                                                DrawingState ds = DrawingManager.Get(ViewBag.DrawingID);
                                                if (ds != null)
                                                {
                                                    string[] ids = HLHandle.Split(',');
                                                    for (int i = 0; i < ids.Length; i++)
                                                    {
                                                        CADImage image = ds.Drawing.GetInstance() as CADImage;
                                                        CADEntity entity = image.Converter.Entities.Find(ent => ent.Handle == ulong.Parse(ids[i]));
                                                        if (entity != null)
                                                        {
                                                            entity.Color = Color.Blue;
                                                            entity.LineWeight = 2;
                                                            image.Converter.Loads(entity);
                                                        }
                                                    }
                                                }
                                            }


                                        }
                                    }
                                    #endregion
                                    #region 记录阅读记录
                                    string referer = "";
                                    if (Request.UrlReferrer != null)
                                    {
                                        referer = Request.UrlReferrer.Host + "|" + Request.UrlReferrer.AbsoluteUri;
                                    }
                                    string strIP = Request.ServerVariables.Get("Remote_Addr");
                                    cadOpt.CreateReading(items[0], "ViewCAD", "", strFilePath, "", "", referer, strIP, Request.UserAgent);
                                    #endregion
                                }
                                else
                                {
                                    strErrorInfo = "无效读取[463]";
                                    isPass = false;
                                    return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                                }
                            }

                        }
                        else
                        {
                            strErrorInfo = "无效查看[459]";
                            isPass = false;
                            return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                        }
                    }
                    else
                    {
                        strErrorInfo = "无效查看[457]";
                        isPass = false;
                        return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                    }
                }
                #endregion
                if (isPass)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                }
            }
        }

        public ActionResult Download(string code, string token = "")
        {
            bool isPass = true;
            string strUrlReferrer = "";
            string strResult = "失败请求！";
            string strErrorInfo = "";
            if (token == null || token == "")
            {
                return RedirectToAction("Index", "Error", new { msg = "无效访问" });
            }
            else
            {
                if (Request.UrlReferrer != null)
                {
                    strUrlReferrer = Request.UrlReferrer.Host.ToString().ToLower();
                }
                #region 合法访问文件
                if (code != null)
                {
                    string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                    string TempCode = EncryptAndDecryptHelper.Locker.Decrypt(code, tempFileCode + "|DownLoad");
                    if (TempCode != null && TempCode != "")
                    {
                        string[] items = TempCode.Split('|');
                        if (items.Length > 0)
                        {
                            string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                            string strHandleInfo = cadOpt.getCADFileHandleLogByFileCode(items[0]);
                            if (strHandleInfo == null || strHandleInfo == "")
                            {
                                strErrorInfo = "查无申请读取记录[222]";
                                isPass = false;
                                return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                            }
                            else
                            {
                                string[] strHandleInfoItems = StringTool.Split(strHandleInfo, StringTool.strSubCut);
                                if (strHandleInfoItems.Length > 1)
                                {
                                    #region 下载CAD文件主体
                                    string strFilePath = strHandleInfoItems[0];
                                    string strJavaID = strHandleInfoItems[1];
                                    #region 到java端检查令牌
                                    StringBuilder javaTokenCheckData = new StringBuilder();
                                    javaTokenCheckData.AppendFormat("\"adId\":\"" + strJavaID + "\",");
                                    javaTokenCheckData.AppendFormat("\"tokenType\":\"" + "2" + "\",");
                                    javaTokenCheckData.AppendFormat("\"token\":\"" + token + "\"");
                                    string data = "{" + javaTokenCheckData.ToString() + "}";
                                    string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                                    string dataJson = IMEAESTool.Encrypt(data, IMEJavaKey);
                                    string enterpriseNo = ConfigurationManager.AppSettings["enterpriseNo"].ToString();
                                    string javaTokenCheckURL = ConfigurationManager.AppSettings["javaTokenCheckURL"].ToString();
                                    //string postData = "enterpriseNo=" + enterpriseNo + "&data=" + dataJson;
                                    //string checkResult = NetHelper.PostWebRequest(javaTokenCheckURL, postData);
                                    string postData = "{\"enterpriseNo\":\"" + enterpriseNo + "\",\"data\":\"" + dataJson + "\"}";
                                    string checkResult = NetHelper.PostJsonWebRequest(javaTokenCheckURL, postData);
                                    dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(checkResult);
                                    string codeOfCheck = domDetailObj.code;
                                    string message = domDetailObj.message;
                                    if (codeOfCheck != "200")
                                    {
                                        return RedirectToAction("Index", "Error", new { msg = "令牌检查失败：" + message });
                                    }
                                    #endregion
                                    int intSeconts = TimeTool.DateDiffBySeconds(DateTime.Now, DateTime.Parse("2019-5-30 11:00"));
                                    if ((intSeconts > int.Parse(tempFileTimeoutNum) * 60) && false)
                                    {
                                        strResult += "<br/>超时了，时间差为" + intSeconts / 60 + "分";
                                        strErrorInfo += "起始时间: " + items[1];
                                        strErrorInfo += ",超时了，时间差为: " + intSeconts / 60 + "分";
                                        isPass = false;
                                    }
                                    else
                                    {
                                        string filePath = strFilePath;
                                        if (filePath != null && filePath.Length > 0 && System.IO.File.Exists(filePath))
                                        {
                                            strResult = "";
                                            var fileName = Path.GetFileName(filePath);
                                            FileStream fs = new FileStream(filePath, FileMode.Open);
                                            byte[] bytes = new byte[(int)fs.Length];
                                            fs.Read(bytes, 0, bytes.Length);
                                            fs.Close();
                                            Response.ContentType = "application/octet-stream";
                                            //通知浏览器下载文件而不是打开
                                            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                                            Response.BinaryWrite(bytes);
                                            Response.Flush();
                                            Response.End();
                                        }
                                    }
                                    #endregion
                                    #region 记录阅读记录
                                    string referer = "";
                                    if (Request.UrlReferrer != null)
                                    {
                                        referer = Request.UrlReferrer.Host + "|" + Request.UrlReferrer.AbsoluteUri;
                                    }
                                    string strIP = Request.ServerVariables.Get("Remote_Addr");
                                    cadOpt.CreateReading(items[0], "Download", "", strFilePath, "", "", referer, strIP, Request.UserAgent);
                                    //cadOpt.doReadingByCode(items[0],  strIP, referer, "Viewer",Request.UserAgent);
                                    #endregion
                                }
                                else
                                {
                                    strErrorInfo = "无效读取[463]";
                                    isPass = false;
                                    return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                                }
                            }

                        }
                        else
                        {
                            strErrorInfo = "无效查看[459]";
                            isPass = false;
                            return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                        }
                    }
                    else
                    {
                        strErrorInfo = "无效查看[457]";
                        isPass = false;
                        return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                    }
                }
                #endregion
                if (isPass)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                }
            }
        }
        public ActionResult getThumbnail(string code, string token = "")
        {
            string strUrlReferrer = "";
            string strResult = "失败请求！";
            if (token == null || token == "")
            {
                return CreateResultImg("~/Content/images/Invalid Access.png");
            }
            else
            {
                if (Request.UrlReferrer != null)
                {
                    strUrlReferrer = Request.UrlReferrer.Host.ToString().ToLower();
                }
                #region 合法访问文件
                if (code != null)
                {
                    string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                    string TempCode = EncryptAndDecryptHelper.Locker.Decrypt(code, tempFileCode + "|Thumbnail");
                    if (TempCode != null && TempCode != "")
                    {
                        string[] items = TempCode.Split('|');
                        if (items.Length > 0)
                        {

                            string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                            string strHandleInfo = cadOpt.getCADFileHandleLogByFileCode(items[0]);
                            if (strHandleInfo == null || strHandleInfo == "")
                            {
                                return CreateResultImg("~/Content/images/noRecord.png");
                            }
                            else
                            {
                                string[] strHandleInfoItems = StringTool.Split(strHandleInfo, StringTool.strSubCut);
                                if (strHandleInfoItems.Length > 1)
                                {
                                    #region 读取CAD文件主体
                                    string strFilePath = strHandleInfoItems[2];
                                    string strJavaID = strHandleInfoItems[1];
                                    #region 到java端检查令牌
                                    StringBuilder javaTokenCheckData = new StringBuilder();
                                    javaTokenCheckData.AppendFormat("\"adId\":\"" + strJavaID + "\",");
                                    javaTokenCheckData.AppendFormat("\"tokenType\":\"" + "3" + "\",");
                                    javaTokenCheckData.AppendFormat("\"token\":\"" + token + "\"");
                                    string data = "{" + javaTokenCheckData.ToString() + "}";
                                    string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                                    string dataJson = IMEAESTool.Encrypt(data, IMEJavaKey);
                                    string enterpriseNo = ConfigurationManager.AppSettings["enterpriseNo"].ToString();
                                    string javaTokenCheckURL = ConfigurationManager.AppSettings["javaTokenCheckURL"].ToString();
                                    //string postData = "enterpriseNo=" + enterpriseNo + "&data=" + dataJson;
                                    //string checkResult = NetHelper.PostWebRequest(javaTokenCheckURL, postData);
                                    string postData = "{\"enterpriseNo\":\"" + enterpriseNo + "\",\"data\":\"" + dataJson + "\"}";
                                    string checkResult = NetHelper.PostJsonWebRequest(javaTokenCheckURL, postData);
                                    dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(checkResult);
                                    string codeOfCheck = domDetailObj.code;
                                    string message = domDetailObj.message;
                                    if (codeOfCheck != "200")
                                    {
                                        return CreateResultImg("~/Content/images/Invalid Access.png");
                                    }
                                    #endregion
                                    int intSeconts = TimeTool.DateDiffBySeconds(DateTime.Now, DateTime.Parse("2019-5-30 11:22"));
                                    if ((intSeconts > int.Parse(tempFileTimeoutNum) * 60) && false)
                                    {
                                        strResult += "<br/>超时了，时间差为" + intSeconts / 60 + "分";
                                        return CreateResultImg("~/Content/images/timeout.png");
                                    }
                                    else
                                    {
                                        string filePath = strFilePath;
                                        if (filePath != null && filePath.Length > 0 && System.IO.File.Exists(filePath))
                                        {
                                            return CreateResultImg(filePath, false);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    return CreateResultImg("~/Content/images/Invalid Access.png");
                                }
                            }

                        }
                        else
                        {
                            return CreateResultImg("~/Content/images/Invalid Access.png");
                        }
                    }
                    else
                    {
                        return CreateResultImg("~/Content/images/Invalid Access.png");
                    }
                }
                #endregion
            }
            return CreateResultImg("~/Content/images/Invalid Access.png");
        }

        public ActionResult viewPDF(string code, string token = "")
        {
            #region 合法访问文件
            if (code != null && code != "" && token != null && token != "")
            {
                ViewBag.code = code;
                ViewBag.token = token;
                string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                string TempCode = EncryptAndDecryptHelper.Locker.Decrypt(code, tempFileCode + "|View");
                if (TempCode != null && TempCode != "")
                {
                    string[] items = TempCode.Split('|');
                    if (items.Length > 0)
                    {

                        string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                        string strHandleInfo = cadOpt.getCADFileHandleLogByFileCode(items[0]);
                        if (strHandleInfo == null || strHandleInfo == "")
                        {
                            return RedirectToAction("Index", "Error", new { msg = "查无申请读取记录[444]" });
                        }
                        else
                        {
                            string[] strHandleInfoItems = StringTool.Split(strHandleInfo, StringTool.strSubCut);
                            if (strHandleInfoItems.Length > 1)
                            {
                                #region 读取CAD文件主体
                                string strFilePath = strHandleInfoItems[0];
                                string strJavaID = strHandleInfoItems[1];
                                #region 到java端检查令牌
                                StringBuilder javaTokenCheckData = new StringBuilder();
                                javaTokenCheckData.AppendFormat("\"adId\":\"" + strJavaID + "\",");
                                javaTokenCheckData.AppendFormat("\"tokenType\":\"" + "1" + "\",");
                                javaTokenCheckData.AppendFormat("\"token\":\"" + token + "\"");
                                string data = "{" + javaTokenCheckData.ToString() + "}";
                                string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                                string dataJson = IMEAESTool.Encrypt(data, IMEJavaKey);
                                string enterpriseNo = ConfigurationManager.AppSettings["enterpriseNo"].ToString();
                                string javaTokenCheckURL = ConfigurationManager.AppSettings["javaTokenCheckURL"].ToString();
                                string postData = "{\"enterpriseNo\":\"" + enterpriseNo + "\",\"data\":\"" + dataJson + "\"}";
                                string checkResult = NetHelper.PostJsonWebRequest(javaTokenCheckURL, postData);
                                dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(checkResult);
                                string codeOfCheck = domDetailObj.code;
                                string message = domDetailObj.message;
                                if (codeOfCheck != "200")
                                {
                                    return RedirectToAction("Index", "Error", new { msg = "令牌检查失败：" + message });
                                }
                                #endregion
                                int intSeconts = TimeTool.DateDiffBySeconds(DateTime.Now, DateTime.Parse("2019-5-30 15:11"));
                                if ((intSeconts > int.Parse(tempFileTimeoutNum) * 60) && false)
                                {
                                    string strResult = "<br/>超时了，时间差为" + intSeconts / 60 + "分";
                                    strResult += "起始时间: " + items[1];
                                    strResult += ",超时了，时间差为: " + intSeconts / 60 + "分";
                                    return RedirectToAction("Index", "Error", new { msg = strResult });
                                }
                                else
                                {
                                    string tempFile = "/temp/" + items[0] + ".pdf";

                                    if (!Directory.Exists(Server.MapPath("/temp/")))
                                    {
                                        Directory.CreateDirectory(Server.MapPath("/temp/"));
                                    }
                                    DirectoryInfo directory = new DirectoryInfo(Server.MapPath("/temp/"));
                                    foreach (FileInfo fi in directory.GetFiles())
                                    {
                                        if (fi.CreationTime.ToString("yyyyMMdd") != DateTime.Now.ToString("yyyyMMdd"))
                                        {
                                            fi.Delete();
                                        }
                                    }
                                    #region 记录阅读记录
                                    string referer = "";
                                    if (Request.UrlReferrer != null)
                                    {
                                        referer = Request.UrlReferrer.Host + "|" + Request.UrlReferrer.AbsoluteUri;
                                    }
                                    string strIP = Request.ServerVariables.Get("Remote_Addr");
                                    cadOpt.CreateReading(items[0], "ViewPDF", "", strFilePath, "", "", referer, strIP, Request.UserAgent);
                                    #endregion

                                }
                                #endregion
                            }
                            else
                            {
                                return RedirectToAction("Index", "Error", new { msg = "Error" });
                            }
                        }

                    }
                    else
                    {
                        return RedirectToAction("Index", "Error", new { msg = "Error" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { msg = "Error" });
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Error", new { msg = "无效查看PDF[438]" });
            }
            #endregion
        }
        #region viewPDF 专用 令牌验证 及 日志功能
        public string getTempPDF(string code, string token = "")
        {
            string strUrlReferrer = "";
            string strResult = "失败请求！";
            string strErrorInfo = "";
            if (Request.UrlReferrer != null)
            {
                strUrlReferrer = Request.UrlReferrer.Host.ToString().ToLower();
            }
            #region 合法访问文件
            if (code != null && token != null && token != "")
            {
                string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                string TempCode = EncryptAndDecryptHelper.Locker.Decrypt(code, tempFileCode + "|View");
                if (TempCode != null && TempCode != "")
                {
                    string[] items = TempCode.Split('|');
                    if (items.Length > 0)
                    {

                        string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                        string strHandleInfo = cadOpt.getCADFileHandleLogByFileCode(items[0]);
                        if (strHandleInfo == null || strHandleInfo == "")
                        {
                            strErrorInfo = "查无申请读取记录[468]";
                            return "Error:" + strErrorInfo;
                        }
                        else
                        {
                            string[] strHandleInfoItems = StringTool.Split(strHandleInfo, StringTool.strSubCut);
                            if (strHandleInfoItems.Length > 1)
                            {
                                #region 读取CAD文件主体
                                string strFilePath = strHandleInfoItems[0];
                                string strJavaID = strHandleInfoItems[1];
                                #region 到java端检查令牌
                                StringBuilder javaTokenCheckData = new StringBuilder();
                                javaTokenCheckData.AppendFormat("\"adId\":\"" + strJavaID + "\",");
                                javaTokenCheckData.AppendFormat("\"tokenType\":\"" + "1" + "\",");
                                javaTokenCheckData.AppendFormat("\"token\":\"" + token + "\"");
                                string data = "{" + javaTokenCheckData.ToString() + "}";
                                string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                                string dataJson = IMEAESTool.Encrypt(data, IMEJavaKey);
                                string enterpriseNo = ConfigurationManager.AppSettings["enterpriseNo"].ToString();
                                string javaTokenCheckURL = ConfigurationManager.AppSettings["javaTokenCheckURL"].ToString();
                                //string postData = "enterpriseNo=" + enterpriseNo + "&data=" + dataJson;
                                //string checkResult = NetHelper.PostWebRequest(javaTokenCheckURL, postData);
                                string postData = "{\"enterpriseNo\":\"" + enterpriseNo + "\",\"data\":\"" + dataJson + "\"}";
                                string checkResult = NetHelper.PostJsonWebRequest(javaTokenCheckURL, postData);
                                dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(checkResult);
                                string codeOfCheck = domDetailObj.code;
                                string message = domDetailObj.message;
                                if (codeOfCheck != "200")
                                {
                                    return "令牌检查失败：" + message;
                                }
                                #endregion
                                int intSeconts = TimeTool.DateDiffBySeconds(DateTime.Now, DateTime.Parse("2019-5-30 15:11"));
                                if ((intSeconts > int.Parse(tempFileTimeoutNum) * 60) && false)
                                {
                                    strResult = "<br/>超时了，时间差为" + intSeconts / 60 + "分";
                                    strErrorInfo += "起始时间: " + items[1];
                                    strErrorInfo += ",超时了，时间差为: " + intSeconts / 60 + "分";
                                    return "Error:" + strResult;
                                }
                                else
                                {
                                    string tempFile = "/temp/" + items[0] + ".pdf";

                                    if (!Directory.Exists(Server.MapPath("/temp/")))
                                    {
                                        Directory.CreateDirectory(Server.MapPath("/temp/"));
                                    }
                                    DirectoryInfo directory = new DirectoryInfo(Server.MapPath("/temp/"));
                                    foreach (FileInfo fi in directory.GetFiles())
                                    {
                                        if (fi.CreationTime.ToString("yyyyMMdd") != DateTime.Now.ToString("yyyyMMdd"))
                                        {
                                            fi.Delete();
                                        }
                                    }
                                    #region 记录阅读记录
                                    //string referer = "";
                                    //if (Request.UrlReferrer != null)
                                    //{
                                    //    referer = Request.UrlReferrer.Host + "|" + Request.UrlReferrer.AbsoluteUri;
                                    //}
                                    //string strIP = Request.ServerVariables.Get("Remote_Addr");
                                    //cadOpt.CreateReading(items[0], "ViewPDF", "", strFilePath, "", "", referer, strIP, Request.UserAgent);
                                    #endregion
                                    if (System.IO.File.Exists(tempFile))
                                    {
                                        return "/Content/PDF/web/viewer.html?file=" + NetHelper.CopyCurrentHost() + "/" + tempFile;
                                    }
                                    else
                                    {
                                        string filePath = strFilePath;
                                        if (filePath != null && filePath.Length > 0 && System.IO.File.Exists(filePath))
                                        {
                                            strResult = "";
                                            FileInfo fi = new FileInfo(filePath);
                                            fi.CopyTo(Server.MapPath(tempFile), true);
                                            string strURL = NetHelper.CopyCurrentHost() + "/Content/PDF/web/viewer.html?file=" + NetHelper.CopyCurrentHost() + "/" + tempFile;
                                            return strURL;
                                        }
                                    }

                                }
                                #endregion
                            }
                            else
                            {
                                return "Error";
                            }
                        }

                    }
                    else
                    {
                        return "Error";
                    }
                }
                else
                {
                    return "Error";
                }
            }
            #endregion
            return "Error";
        }
        #endregion


        public ActionResult viewIMG(string code, string token = "")
        {
            if (code == null || code == "")
            {
                return RedirectToAction("Index", "Error", new { msg = "无效访问" });
            }
            if (token == null || token == "")
            {
                return RedirectToAction("Index", "Error", new { msg = "无效访问" });
            }
            ViewBag.DrawingFile = "/CADViewer/GetCADImg/?code=" + code + "&token=" + token;
            #region 合法访问文件
            if (code != null)
            {
                string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                string TempCode = EncryptAndDecryptHelper.Locker.Decrypt(code, tempFileCode + "|View");
                if (TempCode != null && TempCode != "")
                {
                    string[] items = TempCode.Split('|');
                    if (items.Length > 0)
                    {

                        string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                        string strHandleInfo = cadOpt.getCADFileHandleLogByFileCode(items[0]);
                        if (strHandleInfo == null || strHandleInfo == "")
                        {
                           return RedirectToAction("Index", "Error", new { msg = "查无申请读取记录[694]" });
                        }
                        else
                        {
                            string[] strHandleInfoItems = StringTool.Split(strHandleInfo, StringTool.strSubCut);
                            if (strHandleInfoItems.Length > 1)
                            {
                                #region 读取CAD文件主体
                                string strFilePath = strHandleInfoItems[0];
                                string strJavaID = strHandleInfoItems[1];
                                #endregion
                                #region 记录阅读记录
                                string referer = "";
                                if (Request.UrlReferrer != null)
                                {
                                    referer = Request.UrlReferrer.Host + "|" + Request.UrlReferrer.AbsoluteUri;
                                }
                                string strIP = Request.ServerVariables.Get("Remote_Addr");
                                cadOpt.CreateReading(items[0], "ViewIMG", "", strFilePath, "", "", referer, strIP, Request.UserAgent);
                                #endregion

                            }
                            else
                            {
                                return RedirectToAction("Index", "Error", new { msg = "无效读取[718]" });
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error", new { msg = "无效读取[724]" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Error", new { msg = "无效读取[729]" });
                }
            }
            #endregion
            return View();
        }
        #region 辅助查看图片 viewIMG 令牌验证 日志记录
        public ActionResult GetCADImg(string code, string token = "")
        {
            bool isPass = true;
            string strUrlReferrer = "";
            string strResult = "失败请求！";
            string strErrorInfo = "";
            if (Request.UrlReferrer != null)
            {
                strUrlReferrer = Request.UrlReferrer.Host.ToString().ToLower();
            }
            if (code == null || code == "")
            {
                return CreateResultImg("~/Content/images/Invalid Access.png");
            }
            if (token == null || token == "")
            {
                return CreateResultImg("~/Content/images/Invalid Access.png");
            }
            #region 合法访问文件
            if (code != null)
            {
                string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                string TempCode = EncryptAndDecryptHelper.Locker.Decrypt(code, tempFileCode + "|View");
                if (TempCode != null && TempCode != "")
                {
                    string[] items = TempCode.Split('|');
                    if (items.Length > 0)
                    {

                        string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                        string strHandleInfo = cadOpt.getCADFileHandleLogByFileCode(items[0]);
                        if (strHandleInfo == null || strHandleInfo == "")
                        {
                            strErrorInfo = "查无申请读取记录[397]";
                            isPass = false;
                            return CreateResultImg("~/Content/images/noRecord.png");
                        }
                        else
                        {
                            string[] strHandleInfoItems = StringTool.Split(strHandleInfo, StringTool.strSubCut);
                            if (strHandleInfoItems.Length > 1)
                            {
                                #region 读取CAD文件主体
                                string strFilePath = strHandleInfoItems[0];
                                string strJavaID = strHandleInfoItems[1];
                                #endregion
                                #region 记录阅读记录
                                //string referer = "";
                                //if (Request.UrlReferrer != null)
                                //{
                                //    referer = Request.UrlReferrer.Host + "|" + Request.UrlReferrer.AbsoluteUri;
                                //}
                                //string strIP = Request.ServerVariables.Get("Remote_Addr");
                                //cadOpt.CreateReading(items[0], "ViewIMG", "", strFilePath, "", "", referer, strIP, Request.UserAgent);
                                #region 到java端检查令牌
                                StringBuilder javaTokenCheckData = new StringBuilder();
                                javaTokenCheckData.AppendFormat("\"adId\":\"" + strJavaID + "\",");
                                javaTokenCheckData.AppendFormat("\"tokenType\":\"" + "1" + "\",");
                                javaTokenCheckData.AppendFormat("\"token\":\"" + token + "\"");
                                string data = "{" + javaTokenCheckData.ToString() + "}";
                                string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                                string dataJson = IMEAESTool.Encrypt(data, IMEJavaKey);
                                string enterpriseNo = ConfigurationManager.AppSettings["enterpriseNo"].ToString();
                                string javaTokenCheckURL = ConfigurationManager.AppSettings["javaTokenCheckURL"].ToString();
                                //string postData = "enterpriseNo=" + enterpriseNo + "&data=" + dataJson;
                                //string checkResult = NetHelper.PostWebRequest(javaTokenCheckURL, postData);
                                string postData = "{\"enterpriseNo\":\"" + enterpriseNo + "\",\"data\":\"" + dataJson + "\"}";
                                string checkResult = NetHelper.PostJsonWebRequest(javaTokenCheckURL, postData);
                                dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(checkResult);
                                string codeOfCheck = domDetailObj.code;
                                string message = domDetailObj.message;
                                if (codeOfCheck != "200")
                                {
                                    return CreateResultImg("~/Content/images/Invalid Access.png");
                                }
                                #endregion
                                int intSeconts = TimeTool.DateDiffBySeconds(DateTime.Now, DateTime.Parse("2019-5-30 13:31"));
                                if ((intSeconts > int.Parse(tempFileTimeoutNum) * 60) && false)
                                {
                                    strResult += "<br/>超时了，时间差为" + intSeconts / 60 + "分";
                                    strErrorInfo += "起始时间: " + items[1];
                                    strErrorInfo += ",超时了，时间差为: " + intSeconts / 60 + "分";
                                    isPass = false;
                                    return CreateResultImg("~/Content/images/timeout.png");
                                }
                                else
                                {
                                    string filePath = strFilePath;
                                    if (filePath != null && filePath.Length > 0 && System.IO.File.Exists(filePath))
                                    {
                                        return CreateResultImg(filePath, false);
                                    }
                                }
                                #endregion

                            }
                            else
                            {
                                strErrorInfo = "无效读取[463]";
                                isPass = false;
                                return CreateResultImg("~/Content/images/Invalid Access.png");
                            }
                        }
                    }
                    else
                    {
                        strErrorInfo = "无效查看[459]";
                        isPass = false;
                        return CreateResultImg("~/Content/images/Invalid Access.png");
                    }
                }
                else
                {
                    strErrorInfo = "无效查看[457]";
                    isPass = false;
                    return RedirectToAction("Index", "Error", new { msg = strErrorInfo });
                }
            }
            #endregion
            if (isPass)
            {
                return CreateResultImg("~/Content/images/error.png");
            }
            else
            {
                return CreateResultImg("~/Content/images/error.png");
            }
        }
        public FileContentResult CreateResultImg(string path, bool needMapPath = true)
        {
            string strPath = path;
            if (needMapPath == true)
            {
                strPath = Server.MapPath(path);
            }
            Image img = Image.FromFile(strPath);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return File(ms.ToArray(), "image/jpeg");
        }
        #endregion
    }
}