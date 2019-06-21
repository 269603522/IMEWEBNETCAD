using Business;
using IMEWebCAD.Common.Encrypt;
using IMEWebCAD.Common.Pic;
using IMEWebCAD.Common.Text;
using IMEWebCAD.Common.WebNet;
using Spire.Pdf;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.Mvc;
using WebCAD;
using System.Messaging;
using System.Text.RegularExpressions;
using System.Net;
using Common.Encrypt;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace IMEWebCAD.Controllers
{
    public class HandleController : Controller
    {
        CADOperator cadOpt = new CADOperator();
        public string Index()
        {
            string strIpInfo = "";
            strIpInfo = NetHelper.GetClientWebInfo();
            ViewData["UserInfo"] = strIpInfo;

            string webURLOfLocalSitePort = ConfigurationManager.AppSettings["webURLOfLocalSitePort"].ToString();
            string webURLOfLocalSite = Request.ServerVariables.Get("Local_Addr").ToString();
            if (webURLOfLocalSite == "::1")
            {
                webURLOfLocalSite = "localhost";
            }
            string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
            string webURLOfPublicSite = ConfigurationManager.AppSettings["webURLOfPublicSite"].ToString();
            string webURLOfPublicSitePort = ConfigurationManager.AppSettings["webURLOfPublicSitePort"].ToString();

            string ftpStorage = ConfigurationManager.AppSettings["ftpStorage"].ToString();
            string webFileFinalStorage = ConfigurationManager.AppSettings["webFileFinalStorage"].ToString();



            strIpInfo += "<Br/>公网地址：" + webURLOfPublicSite;
            strIpInfo += "<Br/>内网地址：" + webURLOfLocalSite;
            strIpInfo += "<Br/>内网端口：" + webURLOfLocalSitePort;
            strIpInfo += "<Br/>外网端口：" + webURLOfPublicSitePort;
            strIpInfo += "<Br/>超时分值：" + tempFileTimeoutNum;
            strIpInfo += "<Br/>FTP目录：" + ftpStorage;
            strIpInfo += "<Br/>最终目录：" + webFileFinalStorage;
            string strReferrerUrl = "https://" + webURLOfLocalSite + ":" + webURLOfLocalSitePort + "/Referrer/";
            strIpInfo += "<Br/>来源测试:<a href='" + strReferrerUrl + "'>" + strReferrerUrl + "</a>";

            string strReferrerUrlPostFile = "https://" + webURLOfLocalSite + ":" + webURLOfLocalSitePort + "/PostFile.html";
            strIpInfo += "<Br/>测试传文件接口:<a href='" + strReferrerUrlPostFile + "'>" + strReferrerUrlPostFile + "</a>";
            //Request.Headers.Get["Authorization"];
            //Request.Headers.GetKey("Authorization");
            string strReferrerUrlPath = "https://" + webURLOfLocalSite + ":" + webURLOfLocalSitePort + "/Path.html";
            strIpInfo += "<Br/>测试传路径接口:<a href='" + strReferrerUrlPath + "'>" + strReferrerUrlPath + "</a>";
            return strIpInfo;
        }
        /// <summary>
        /// CAD文件拷贝接口
        /// </summary>
        /// <param name="fileJavaID">已经预处理的java端文件号</param>
        /// <param name="fileTargetJavaID">未预处理的java端文件号</param>
        /// <returns>
        /// {
        /// "adId": 图纸云的图纸id,
        /// "status": 转换结果 1: 成功, -1: 转换失败, -2: 文件不存在,
        /// "displayUri": 大图查看的uri,
        /// "ioUri": 源图流获取的uri,
        /// "thumIoUri": 缩略图的uri
        /// }
        /// </returns>
        // https://192.168.255.103:2019/Handle/copyFile?fileJavaID=D8137EF98A34AA0B78D2C18EE168523&fileTargetJavaID=test&strSeal=&strSealLocation=
        [HttpPost]
        public string copyFile(string data = "")
        {
            string strResult = "";
            if (data != null && data != "")
            {
                string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                string dataJson = IMEAESTool.Decrypt(data, IMEJavaKey);
                dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(dataJson);
                string fileJavaID = domDetailObj.fileJavaID;
                string fileTargetJavaID = domDetailObj.fileTargetJavaID;
                if (fileJavaID != null && fileJavaID != "" && fileTargetJavaID != null && fileTargetJavaID != "" && fileJavaID != fileTargetJavaID)
                {
                    string strHandleLog = cadOpt.getCADFileHandleLogByJavaCode(fileJavaID);
                    if (strHandleLog != null && strHandleLog != "" && strHandleLog.IndexOf(StringTool.strSubCut) > 0)
                    {
                        //-------------------------------------------------------------------------
                        string referer = "";
                        if (Request.UrlReferrer != null)
                        {
                            referer = Request.UrlReferrer.Host + "|" + Request.UrlReferrer.AbsoluteUri;
                        }
                        string strIP = Request.ServerVariables.Get("Remote_Addr");
                        string webURLOfLocalSitePort = ConfigurationManager.AppSettings["webURLOfLocalSitePort"].ToString();
                        string webURLOfLocalSite = Request.ServerVariables.Get("Local_Addr").ToString();
                        if (webURLOfLocalSite == "::1")
                        {
                            webURLOfLocalSite = "localhost";
                        }
                        string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
                        string webFileFinalStorage = ConfigurationManager.AppSettings["webFileFinalStorage"].ToString();
                        string strTimeDir = DateTime.Now.ToString("yyyyMMdd");
                        string localDir = webFileFinalStorage + "/" + strTimeDir + "/";
                        string localCopyDir = localDir + "/Copy/";

                        if (!Directory.Exists(localCopyDir))
                        {
                            Directory.CreateDirectory(localCopyDir);
                        }
                        string[] handleInfo = StringTool.Split(strHandleLog, StringTool.strSubCut);
                        if (handleInfo[0] != null && handleInfo[0].Length > 0)
                        {
                            if (System.IO.File.Exists(handleInfo[0]))
                            {
                                string filePath = handleInfo[0].Replace('\\', '/');
                                string fileName = filePath.Substring(filePath.LastIndexOf("/") + 1, (filePath.Length - handleInfo[0].LastIndexOf("/") - 1));
                                string NewFile = localCopyDir + fileName;
                                System.IO.File.Copy(handleInfo[0], NewFile, true);
                                #region add copy handle log
                                string ReturnID = cadOpt.addCADFileHandleLog(fileTargetJavaID, fileJavaID, fileName, "IMECopy", NewFile, "success", handleInfo[2], referer, strIP, "true");
                                if (ReturnID != null && ReturnID.Length > 0)
                                {
                                    string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                                    EncryptAndDecryptHelper encryptAndDecryptHelper = new EncryptAndDecryptHelper();
                                    string TempCode_View = EncryptAndDecryptHelper.Locker.Encrypt(ReturnID, tempFileCode + "|View");
                                    string TempCode_DownLoad = EncryptAndDecryptHelper.Locker.Encrypt(ReturnID, tempFileCode + "|DownLoad");
                                    string TempCode_Thumbnail = EncryptAndDecryptHelper.Locker.Encrypt(ReturnID, tempFileCode + "|Thumbnail");
                                    string tempViewCADUrl = "/CADViewer/viewCAD/?code=" + TempCode_View;
                                    string fileType = filePath.Substring(filePath.LastIndexOf(".") + 1, (filePath.Length - filePath.LastIndexOf(".") - 1)).ToLower();
                                    if (fileType.ToLower() == "png" || fileType.ToLower() == "bmp" || fileType.ToLower() == "jpg" || fileType.ToLower() == "jpeg")
                                    {
                                        tempViewCADUrl = "/CADViewer/viewIMG/?code=" + TempCode_View;
                                    }
                                    else if (fileType.ToLower() == "pdf")
                                    {
                                        tempViewCADUrl = "/CADViewer/viewPDF/?code=" + TempCode_View;
                                    }
                                    string tempDownloadCADUrl = "/CADViewer/Download/?code=" + TempCode_DownLoad;
                                    string tempThumbnailUrl = "/CADViewer/getThumbnail/?code=" + TempCode_Thumbnail;
                                    strResult = "";
                                    strResult = "{";
                                    strResult += "\"adId\": \"" + ReturnID + "\",";
                                    strResult += "\"status\": \"1\",";
                                    strResult += "\"displayUri\": \"" + tempViewCADUrl + "\",";
                                    strResult += "\"ioUri\": \"" + tempDownloadCADUrl + "\",";
                                    strResult += "\"thumIoUri\": \"" + tempThumbnailUrl + "\"";
                                    strResult += "}";
                                }
                                else
                                {
                                    strResult = "";
                                    strResult = "{";
                                    strResult += "\"adId\": \"" + fileJavaID + "\",";
                                    strResult += "\"status\": \"-1\",";
                                    strResult += "\"displayUri\": \"\",";
                                    strResult += "\"ioUri\": \"\",";
                                    strResult += "\"thumIoUri\": \"\"";
                                    strResult += "}";
                                }
                                #endregion
                            }
                            else
                            {
                                strResult = "";
                                strResult = "{";
                                strResult += "\"adId\": \"" + fileJavaID + "\",";
                                strResult += "\"status\": \"-1\",";
                                strResult += "\"displayUri\": \"\",";
                                strResult += "\"ioUri\": \"\",";
                                strResult += "\"thumIoUri\": \"\"";
                                strResult += "}";
                            }

                        }
                        else
                        {
                            strResult = "";
                            strResult = "{";
                            strResult += "\"adId\": \"" + fileJavaID + "\",";
                            strResult += "\"status\": \"-1\",";
                            strResult += "\"displayUri\": \"\",";
                            strResult += "\"ioUri\": \"\",";
                            strResult += "\"thumIoUri\": \"\"";
                            strResult += "}";
                        }
                    }
                    else
                    {
                        strResult = "";
                        strResult = "{";
                        strResult += "\"adId\": \"" + fileJavaID + "\",";
                        strResult += "\"status\": \"-1\",";
                        strResult += "\"displayUri\": \"\",";
                        strResult += "\"ioUri\": \"\",";
                        strResult += "\"thumIoUri\": \"\"";
                        strResult += "}";
                    }
                }
                else
                {
                    strResult = "";
                    strResult = "{";
                    strResult += "\"adId\": \"" + fileJavaID + "\",";
                    strResult += "\"status\": \"-1\",";
                    strResult += "\"displayUri\": \"\",";
                    strResult += "\"ioUri\": \"\",";
                    strResult += "\"thumIoUri\": \"\"";
                    strResult += "}";
                }
            }
            else
            {
                strResult = "";
                strResult = "{";
                strResult += "\"adId\": \"\",";
                strResult += "\"status\": \"-1\",";
                strResult += "\"displayUri\": \"\",";
                strResult += "\"ioUri\": \"\",";
                strResult += "\"thumIoUri\": \"\"";
                strResult += "}";
            }
            return strResult;
        }
        // https://192.168.255.103:2019/Handle/addHandleFileRequest?fileJavaID=11112222&fileFTPPath=test2222&isDeleteAfterHandle=true
        [HttpPost]
        public string addHandleFileRequest(string data)
        {
            string strResult = "";
            if(data!=null&& data!="")
            {
                string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                string dataJson = IMEAESTool.Decrypt( data, IMEJavaKey);
                dynamic domDetailObj = Newtonsoft.Json.JsonConvert.DeserializeObject(dataJson); 
                string fileJavaID = domDetailObj.fileJavaID;
                string fileFTPPath = domDetailObj.filePath;
                string isDeleteAfterHandle = domDetailObj.isDeleteAfterHandle;
                string strIP = "";
                try
                {
                    //StringBuilder info = new StringBuilder();// 我们关注的三个    
                    //info.AppendFormat("HTTP_VIA = {0}\r\n", Request.ServerVariables["HTTP_VIA"]);
                    //info.AppendFormat("HTTP_X_FORWARDED_FOR = {0} \r\n", Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                    //info.AppendFormat("REMOTE_ADDR = {0} \r\n", Request.ServerVariables["REMOTE_ADDR"]);
                    //info.AppendLine("*********** \r\n");// 其他有参考价值的  
                    //foreach (string key in Request.ServerVariables.AllKeys)
                    //{
                    //    info.AppendFormat("{0} = {1} \r\n", key, Request.ServerVariables[key]);
                    //}
                    strIP = Request.ServerVariables["REMOTE_ADDR"];
                }
                catch { }
                try
                {
                    StringBuilder strData = new StringBuilder();
                    strData.AppendFormat("\"action\":\"handleFileRequest\",");
                    strData.AppendFormat("\"fileJavaID\":\"" + fileJavaID + "\",");
                    strData.AppendFormat("\"fileFTPPath\":\"" + fileFTPPath + "\",");
                    strData.AppendFormat("\"isDeleteAfterHandle\":\"" + isDeleteAfterHandle + "\",");
                    strData.AppendFormat("\"handleResult\":\"" + "" + "\",");
                    strData.AppendFormat("\"ip\":\"" + strIP + "\",");
                    strData.AppendFormat("\"time\":\"" + DateTime.Now.ToString() + "\",");
                    strData.AppendFormat("\"handleCount\":\"" + "0" + "\"");
                    
                    //连接到本地的队列
                    MessageQueue myQueue = new MessageQueue(".\\private$\\imeCadFile");
                    Message myMessage = new Message();
                    myMessage.Label = "addHandleFileRequest";
                    myMessage.Body = "{" + strData.ToString() + "}";
                    myMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    // Send a message to the queue.
                    if (myQueue.Transactional == true)
                    {
                        // Create a transaction.
                        MessageQueueTransaction myTransaction = new
                            MessageQueueTransaction();

                        // Begin the transaction.
                        myTransaction.Begin();

                        // Send the message.
                        myQueue.Send(myMessage, myTransaction);

                        // Commit the transaction.
                        myTransaction.Commit();
                    }
                    else
                    {
                        myQueue.Send(myMessage);
                    }
                    strResult = "{\"Result\":\"Success\"}";
                }
                catch (Exception e)
                {
                    string webURLOfLocalSite = Request.ServerVariables.Get("Local_Addr").ToString();
                    if (webURLOfLocalSite == "::1")
                    {
                        strResult = "{\"Result\":\"Exception[" + e.Message + "]\"}";
                    }
                    else
                    {
                        strResult = "{\"Result\":\"Fault\"}";
                    }
                }
            }
            return strResult;
        }
        // https://192.168.255.103:2019/Handle/getThumbnail?filePath=E:/IMENetCadFiles/20190612/CAD/imeTestCADFile_20190604__0_636959278483858542.dwg&type=dwg
        public string getThumbnail(string filePath, string type="")
        {
            string pathPic = "";
            try
            {
                if(type.ToLower().Trim()=="dxf"|| type.ToLower().Trim() == "dwg")
                {
                    string webFileFinalStorage = ConfigurationManager.AppSettings["webFileFinalStorage"].ToString();
                    string strTimeDir = DateTime.Now.ToString("yyyyMMdd");
                    string localDir = webFileFinalStorage + "/" + strTimeDir + "/Thumbnail/";
                    if(!Directory.Exists(localDir))
                    {
                        Directory.CreateDirectory(localDir);
                    }
                    string pathCADFileId = DrawingManager.Add(null, filePath).Id;
                    DrawingState ds = DrawingManager.Get(pathCADFileId);
                    Bitmap bmp = new Bitmap((int)300, (int)300);
                    double[,] matrix = new double[4, 4];
                    System.Drawing.Rectangle rectangle = new Rectangle(0, 0, 300, 300);
                    ds.Drawing.Draw(ref bmp, rectangle, Color.Transparent, Color.Transparent);
                    string strThumbnailName = "Thumbnail" + DateTime.Now.Ticks + "_300_300.bmp";
                    pathPic = Path.Combine(localDir, strThumbnailName);
                    bmp.Save(pathPic, ImageFormat.Bmp);
                }
            }
            catch (Exception e) {
                return "{\"result\":\"exception\",\"data\":\""+e.Message+"["+ filePath + "]\"}";
            }
            if(pathPic!="")
            {
                return "{\"result\":\"success\",\"data\":\"" + pathPic + "\"}";
            }
            else
            {
                return "{\"result\":\"fault\",\"data\":\"" + "" + "\"}";
            }
            
        }

    }
}

