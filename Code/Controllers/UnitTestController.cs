using Business;
using Common.Encrypt;
using IMEWebCAD.Common.Encrypt;
using IMEWebCAD.Common.Text;
using IMEWebCAD.Common.WebNet;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMEWebCAD.Controllers
{
    public class UnitTestController : Controller
    {
        //
        // GET: /UnitTest/
        // https://192.168.255.103:2019/UnitTest/Index
        public ActionResult Index()
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return CreateResultImg("~/Content/images/Invalid Access.png");
            }
            return View();
        }
        public FileContentResult CreateResultImg(string path, bool needMapPath = true)
        {
            string strPath = path;
            if (needMapPath == true)
            {
                strPath = Server.MapPath(path);
            }
            if(path=="")
            {
                strPath = Server.MapPath("~/Content/images/Invalid Access.png");
            }
            Image img = Image.FromFile(strPath);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return File(ms.ToArray(), "image/jpeg");
        }
        //https://192.168.255.103:2019//UnitTest/testCreateDummyData?keyWord=5w&fileType=cad&testFile=&count=2
        public string testCreateDummyData(string keyWord = "7", string fileType = "cad", string count = "1", string testFile = "")
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string ftpStorage = ConfigurationManager.AppSettings["ftpStorage"].ToString();
            //string webFileFinalStorage = ConfigurationManager.AppSettings["webFileFinalStorage"].ToString();
            HandleController handleController = new HandleController();
            int loopCount = int.Parse(count);
            string strResult = "";
            try
            {
                if (fileType == "cad")
                {
                    string OrignFile = ftpStorage + "/testFile/焊接支架.dwg";
                    testFile = testFile.ToLower();
                    if (testFile != "" && testFile.LastIndexOf(".dwg") > 0)
                    {
                        OrignFile = testFile;
                    }
                    for (int i = 0; i < loopCount; i++)
                    {
                        string fileId = "imeTestCADFile_" + DateTime.Now.Ticks + "_" + keyWord + "_" + i;
                        string NewFile = ftpStorage + "" + fileId + "_" + ".dwg";
                        System.IO.File.Copy(OrignFile, NewFile);
                        StringBuilder strData = new StringBuilder();
                        strData.AppendFormat("\"fileJavaID\":\"" + fileId + "\",");
                        strData.AppendFormat("\"filePath\":\"" + NewFile + "\",");
                        strData.AppendFormat("\"isDeleteAfterHandle\":\"" + "true" + "\",");
                        strData.AppendFormat("\"handleResult\":\"" + "" + "\",");
                        strData.AppendFormat("\"handleCount\":\"" + "0" + "\"");

                        string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                        string paramDataSrc = "{" + strData.ToString() + "}";
                        string paramData = "{\"data\":\"" + IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey) + "\"}";

                        strResult += NewFile + "<br/> " + handleController.addHandleFileRequest(IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey)) + "<br/>" + NetHelper.CopyCurrentHost() + "<br/>" + paramData;
                    }
                }
                if (fileType == "dxf")
                {
                    string OrignFile = ftpStorage + "/testFile/test.dxf";
                    testFile = testFile.ToLower();
                    if (testFile != "" && testFile.LastIndexOf(".dxf") > 0)
                    {
                        OrignFile = testFile;
                    }
                    for (int i = 0; i < loopCount; i++)
                    {
                        string fileId = "imeTestDXFFile_20190614_" + DateTime.Now.Ticks + "_" + keyWord + "_" + i;
                        string NewFile = ftpStorage + "" + fileId + ".dxf";
                        System.IO.File.Copy(OrignFile, NewFile);
                        StringBuilder strData = new StringBuilder();
                        strData.AppendFormat("\"fileJavaID\":\"" + fileId + "\",");
                        strData.AppendFormat("\"filePath\":\"" + NewFile + "\",");
                        strData.AppendFormat("\"isDeleteAfterHandle\":\"" + "true" + "\",");
                        strData.AppendFormat("\"handleResult\":\"" + "" + "\",");
                        strData.AppendFormat("\"handleCount\":\"" + "0" + "\"");

                        string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                        string paramDataSrc = "{" + strData.ToString() + "}";
                        string paramData = "{\"data\":\"" + IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey) + "\"}";

                        strResult += NewFile + "<br/> " + handleController.addHandleFileRequest(IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey)) + "<br/>" + NetHelper.CopyCurrentHost() + "<br/>" + paramData;
                    }
                }
                else if (fileType == "img")
                {
                    string OrignFile = ftpStorage + "/testFile/img.png";
                    testFile = testFile.ToLower();
                    if (testFile != "" && (testFile.LastIndexOf(".png") > 0 || testFile.LastIndexOf(".bmp") > 0 || testFile.LastIndexOf(".jpg") > 0 || testFile.LastIndexOf(".jpeg") > 0))
                    {
                        OrignFile = testFile;
                    }
                    for (int i = 0; i < loopCount; i++)
                    {
                        string fileId = "imeTestIMGFile_20190604_" + DateTime.Now.Ticks + "-" + keyWord + "_" + i;
                        string NewFile = ftpStorage + "/" + fileId + "_" + ".png";
                        System.IO.File.Copy(OrignFile, NewFile);
                        StringBuilder strData = new StringBuilder();
                        strData.AppendFormat("\"fileJavaID\":\"" + fileId + "\",");
                        strData.AppendFormat("\"filePath\":\"" + NewFile + "\",");
                        strData.AppendFormat("\"isDeleteAfterHandle\":\"" + "true" + "\",");
                        strData.AppendFormat("\"handleResult\":\"" + "" + "\",");
                        strData.AppendFormat("\"handleCount\":\"" + "0" + "\"");
                        string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                        string paramDataSrc = "{" + strData.ToString() + "}";
                        string paramData = "{\"data\":\"" + IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey) + "\"}";
                        strResult += NewFile + "<br/> " + handleController.addHandleFileRequest(IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey)) + "<br/>" + NetHelper.CopyCurrentHost() + "<br/>" + paramData;
                    }
                }
                else if (fileType == "pdf")
                {
                    string OrignFile = ftpStorage + "/testFile/考勤管理制度.pdf";
                    if (testFile != "" && testFile.LastIndexOf(".pdf") > 0)
                    {
                        OrignFile = testFile;
                    }
                    for (int i = 0; i < loopCount; i++)
                    {
                        string fileId = "imeTestPDFFile_20190604_" + DateTime.Now.Ticks + "_" + keyWord + "_" + i;
                        string NewFile = ftpStorage + "/" + fileId + "_" + ".pdf";
                        System.IO.File.Copy(OrignFile, NewFile);
                        StringBuilder strData = new StringBuilder();
                        strData.AppendFormat("\"fileJavaID\":\"" + fileId + "\",");
                        strData.AppendFormat("\"filePath\":\"" + NewFile + "\",");
                        strData.AppendFormat("\"isDeleteAfterHandle\":\"" + "true" + "\",");
                        strData.AppendFormat("\"handleResult\":\"" + "" + "\",");
                        strData.AppendFormat("\"handleCount\":\"" + "0" + "\"");
                        string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
                        string paramDataSrc = "{" + strData.ToString() + "}";
                        string paramData = "{\"data\":\"" + IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey) + "\"}";
                        strResult += NewFile + "<br/> " + handleController.addHandleFileRequest(IMEAESTool.Encrypt(paramDataSrc, IMEJavaKey)) + "<br/>" + NetHelper.CopyCurrentHost() + "<br/>" + paramData;
                    }
                }
            }
            catch (Exception e)
            {
                strResult = "异常:" + e.Message + "|" + ftpStorage;
            }

            return strResult;
        }
        // https://192.168.255.103:2019/UnitTest/ListhandleFileRequest?maxCount=1000
        public string ListhandleFileRequest(string maxCount = "100", string tag = "1")
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            if (tag == "0")
            {
                tag = "";
            }
            if (tag == "1")
            {
                tag = "1";
            }
            else if (tag == "2")
            {
                tag = "2";
            }
            else if (tag == "-1")
            {
                tag = "Final";
            }
            string strResult = "";
            string msmqName = ".\\private$\\imeCadFile" + tag;
            //连接到本地队列 
            MessageQueue myQueue = new MessageQueue(msmqName);
            myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            myQueue.MessageReadPropertyFilter.Priority = true;
            try
            {
                int countMax = int.Parse(maxCount);
                //从队列中接收消息 
                // System.Messaging.Message myMessage = myQueue.Receive();

                // strResult = myMessage.Body.ToString(); //获取消息的内容 

                System.Messaging.Message[] messages = myQueue.GetAllMessages();
                strResult += "<div class='myQueueCount'>" + msmqName + "总计：" + messages.Length + "个记录</div>";
                for (int index = 0; index < messages.Length && index < countMax; index++)
                {
                    string id = messages[index].Id;
                    string body = messages[index].Body.ToString();
                    string priority = messages[index].Priority.ToString();
                    strResult += "<div class='myQueueItem' tag='" + id + "' title='" + id + "'>" + body + "</div>";
                }
            }
            catch (Exception e)
            {
                strResult = "异常：" + e.Message + "";
            }
            return strResult;
        }

        public string aesEncrypt(string data, string key)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string strResult = "";
            try
            {
                strResult = IMEAESTool.Encrypt(data, key);
            }
            catch (Exception e)
            {
                strResult = "加密异常：" + e.Message + "<br/>" + data + "<br/>" + key;
            }
            return strResult;
        }
        public string aesDecrypt(string data, string key)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string strResult = "";
            try
            {
                strResult = IMEAESTool.Decrypt(data, key);
            }
            catch (Exception e)
            {
                strResult = "解密异常：" + e.Message + "<br/>" + data + "<br/>" + key;
            }
            return strResult;
        }

        public string getViewURL(string handleCode, string fileNewPath, string fileJavaID)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string strResult = "";
            if (handleCode != null && handleCode.Length > 0)
            {
                #region 生成查看下载CAD文件URL
                if (fileNewPath != null && fileNewPath.Length > 0 && System.IO.File.Exists(fileNewPath))
                {
                    string tempFileCode = ConfigurationManager.AppSettings["tempFileCode"].ToString();
                    string TempCode_View = EncryptAndDecryptHelper.Locker.Encrypt(handleCode, tempFileCode + "|View");
                    string TempCode_DownLoad = EncryptAndDecryptHelper.Locker.Encrypt(handleCode, tempFileCode + "|DownLoad");
                    string TempCode_Thumbnail = EncryptAndDecryptHelper.Locker.Encrypt(handleCode, tempFileCode + "|Thumbnail");
                    string tempViewCADUrl = "/CADViewer/viewCAD/?code=" + TempCode_View;
                    string fileType = fileNewPath.Substring(fileNewPath.LastIndexOf(".") + 1, (fileNewPath.Length - fileNewPath.LastIndexOf(".") - 1)).ToLower();
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
                    strResult += "\"adId\": \"" + fileJavaID + "\",";
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
                    strResult += "\"status\": \"-2\",";
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
            return strResult;
        }
        // https://192.168.255.103:2019/UnitTest/testDB
        public string testDB()
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            MySqlParameter[] paramters = new MySqlParameter[] {

            };
            DataSet ds = MySqlHelper.ExecuteDataset(connetStr, "select * from t_cadfilesreadinglog;", paramters);
            if (ds != null && ds.Tables.Count > 0)
            {
                return "总共" + ds.Tables[0].Rows.Count + "条数据";
            }
            else
            {
                return "未查到数据:" + connetStr;
            }
        }
        public string getDoViewLog(string action = "", string subDir = "")
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            // FTP目录
            string ftpStorage = ConfigurationManager.AppSettings["ftpStorage"].ToString();
            //最终文件目录
            string webFileFinalStorage = ConfigurationManager.AppSettings["webFileFinalStorage"].ToString();
            //网站日志目录
            string webLogDir = ConfigurationManager.AppSettings["webLogDir"].ToString();
            //服务目录
            string IMEWebCadServerPath = ConfigurationManager.AppSettings["IMEWebCadServerPath"].ToString();
            if (action == "actionLog")
            {
                string dir = webLogDir + "/ActionLog/"+ subDir;
                return "" + getFoldsAndFiles(dir, true);
            }
            else if (action == "exceptionLog")
            {
                string dir = webLogDir + "/ExceptionLog/"+ subDir;
                return "" + getFoldsAndFiles(dir, true);
            }
            else if (action == "serverLog")
            {
                string dir = IMEWebCadServerPath + "/log/"+ subDir;
                return "" + getFoldsAndFiles(dir, true);
            }
            else if (action == "tempFtp")
            {
                string dir = ftpStorage + "/"+ subDir;
                return "" + getFoldsAndFiles(dir, false);
            }
            else if (action == "finalDir")
            {
                string dir = webFileFinalStorage + "/"+subDir;
                return "" + getFoldsAndFiles(dir, false);
            }
            return "";
        }

        #region get folds and files
        public string getFoldsAndFiles(string inPath, bool needOpt)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string str = "";
            str += "<div class='UPICTop' Path='" + inPath + "' >当前目录<b>" + inPath + "</b></div>";
            string strDirlist = inPath;
            DirectoryInfo thisOne = new DirectoryInfo(strDirlist);
            DirectoryInfo[] subDirectories = thisOne.GetDirectories();//获得目录 
            foreach (DirectoryInfo dirinfo in subDirectories)
            {
                str += "<div class='spDir' Path='" + inPath + "' name='" + dirinfo.Name.ToString() + "' ondblclick='OpenDir(this)'><img src='Content/UnitTestImg/fold.png'  style='width: 88px;height:88px; ' /><br/><b>" + dirinfo.Name.ToString() + "</b><br/><span  class='spOptString spDel'  onclick='doOptDelDir(this)' Path='" + inPath + "' name='" + dirinfo.Name.ToString() + "'>[删除]</span><br/></div>";
                str += "";
            }
            FileInfo[] fileInfo = thisOne.GetFiles();   //目录下的文件 
            foreach (FileInfo fInfo in fileInfo)
            {
                if (fInfo.Name.ToString().IndexOf("jimiDel") == -1)
                {
                    str += "<div class='spFile' Path='" + inPath + "'  name='" + fInfo.Name.ToString() + "'> ";
                    if(fInfo.Name.EndsWith(".png")|| fInfo.Name.EndsWith(".bmp") || fInfo.Name.EndsWith(".gif") || fInfo.Name.EndsWith(".jpg"))
                    {
                        str += "<img src='/UnitTest/CreateResultImg/?path=" + (inPath + "/" + fInfo.Name) + "&needMapPath=false' style='width: 88px;height:88px; ' /><br/>";
                    }
                    else if (fInfo.Name.EndsWith(".dxf"))
                    {
                        str += "<img src='Content/UnitTestImg/dxf.png' style='width: 88px;height:88px; ' /><br/>";
                    }
                    else if (fInfo.Name.EndsWith(".dwg"))
                    {
                        str += "<img src='Content/UnitTestImg/dwg.png' style='width: 88px;height:88px; ' /><br/>";
                    }
                    else if (fInfo.Name.EndsWith(".pdf"))
                    {
                        str += "<img src='Content/UnitTestImg/pdf.png' style='width: 88px;height:88px; ' /><br/>";
                    }
                    else
                    {
                        str += "<img src='Content/UnitTestImg/file.png' style='width: 88px;height:88px; ' /><br/>";
                    }
                    if (needOpt&& fInfo.Name.LastIndexOf(".txt")== fInfo.Name.Length-4)
                    {
                        str += "<span  class='spRename spOptString' onclick='doOptView(this)'>[查看]</span>&nbsp;&nbsp;&nbsp;&nbsp;<span  class='spOptString spDel'  onclick='doOptDel(this)'>[删除]</span><br/>";
                    }
                    str += "<input type='text' class='nameText' value='"+fInfo.Name.ToString() + "'/><br/>";
                    str += fInfo.LastWriteTime.ToString() + "<br/>";
                    str += Math.Round((double)fInfo.Length/1024,3)+"k";
                    str += " </div>";
                }
            }
            return str;
        }
        #endregion
        public string delFile(string path = "")
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return "done";
                }
            }
            catch (Exception e)
            {
                return "Exception:" + e.Message;
            }
            return "";
        }
        public string doOptDelDir(string path = "")
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            try
            {
                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path);
                    return "done";
                }
            }
            catch (Exception e)
            {
                return "Exception:" + e.Message;
            }
            return "";
        }
        
        public string editTextFile(string path = "", string text = "")
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.WriteAllText(path, text);
                    return "done";
                }
            }
            catch (Exception e)
            {
                return "Exception:" + e.Message;
            }

            return "";
        }
        [HttpPost]
        public string ReadTxtContent(string path)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string result = "";
            string strReadFilePath = path;
            if (path.ToLower().IndexOf("HandleLog".ToLower()) > 0)
            {
                FileStream fs = new FileStream(strReadFilePath, FileMode.Open, FileAccess.Read);
                StreamReader srReadFile = new StreamReader(fs, Encoding.Default);

                // 读取流直至文件末尾结束
                while (!srReadFile.EndOfStream)
                {
                    string strReadLine = srReadFile.ReadLine(); //读取每行数据
                    result += "" + strReadLine + "\n";
                }

                // 关闭读取流文件
                srReadFile.Close();
            }
            else
            {
                FileStream fs = new FileStream(strReadFilePath, FileMode.Open, FileAccess.Read);
                StreamReader srReadFile = new StreamReader(fs, Encoding.UTF8);

                // 读取流直至文件末尾结束
                while (!srReadFile.EndOfStream)
                {
                    string strReadLine = srReadFile.ReadLine(); //读取每行数据
                    result += "" + strReadLine + "\n";
                }

                // 关闭读取流文件
                srReadFile.Close();
            }

            return result;
        }
        // https://192.168.255.103:2019/UnitTest/delMSMQ?tag=0
        public string delMSMQ(string tag)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            if (tag == "0")
            {
                tag = "";
            }
            if (tag == "1")
            {
                tag = "1";
            }
            else if (tag == "2")
            {
                tag = "2";
            }
            else if (tag == "-1")
            {
                tag = "Final";
            }
            string strResult = "";
            string msmqName = ".\\private$\\imeCadFile" + tag;
            //连接到本地队列 
            MessageQueue myQueue = new MessageQueue(msmqName);
            myQueue.Purge();
            //myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            //myQueue.MessageReadPropertyFilter.Priority = true;
            //MessageEnumerator enumerator = myQueue.GetMessageEnumerator2();
            //int count = 0;
            //while (enumerator.MoveNext())
            //{
            //    enumerator.RemoveCurrent();
            //    count++;
            //}
            strResult = "清空队列" + msmqName + "";
            return strResult;
        }
        [HttpPost]
        public string checkMSMQ(string javaId, string fileFTPPath, string isDeleteAfterHandle)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string strResult = "";
            CADOperator co = new CADOperator();
            string strHandleLog = co.getCADFileHandleLogByJavaCode(javaId);

            if (strHandleLog != null && strHandleLog != "" && strHandleLog.IndexOf(StringTool.strSubCut) > 0)
            {
                strResult += "JavaID【" + javaId + "】";
                string[] handleInfo = StringTool.Split(strHandleLog, StringTool.strSubCut);
                strResult += "最终路径【" + handleInfo[0] + "】存在否：" + System.IO.File.Exists(handleInfo[0]) + "<br/>";
                strResult += "缩略图路径【" + handleInfo[2] + "】存在否：" + System.IO.File.Exists(handleInfo[2]) + "<br/>";
            }
            else
            {
                strResult += "JavaID【" + javaId + "】未找到有效数据库记录<br/>";
            }
            bool isExists = System.IO.File.Exists(fileFTPPath);
            strResult += "FileFTPPath【" + fileFTPPath + "】存在否：" + isExists + " " + (isExists ? "<input type='button' value='删除该文件' onclick='mq_delFile(this)' tag='" + fileFTPPath + "' class='btn-info' /><input type='button' value='加回原始队列' onclick='mq_sendBackToMQ(this)' isDeleteAfterHandle='" + isDeleteAfterHandle + "' javaId='" + javaId + "' fileFTPPath='" + fileFTPPath + "' class='btn-info' />" : "") + "<input type='button' value='出队不处理' onclick='mq_popMQ(this)' tag='" + fileFTPPath + "' javaId='" + javaId + "' class='btn-info' /><br/><br/>";
            return strResult;
        }
        [HttpPost]
        public string MQ_delFile(string fileFTPPath)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            try
            {
                System.IO.File.Delete(fileFTPPath);
                return "done";
            }
            catch (Exception e)
            {
                return "Exception:" + e.Message;
            }
        }
        [HttpPost]
        public string backToMQ(string fileJavaID, string fileFTPPath, string isDeleteAfterHandle)
        {

            try
            {
                StringBuilder strData = new StringBuilder();
                strData.AppendFormat("\"action\":\"handleFileRequest\",");
                strData.AppendFormat("\"fileJavaID\":\"" + fileJavaID + "\",");
                strData.AppendFormat("\"fileFTPPath\":\"" + fileFTPPath + "\",");
                strData.AppendFormat("\"isDeleteAfterHandle\":\"" + isDeleteAfterHandle + "\",");
                strData.AppendFormat("\"handleResult\":\"" + "" + "\",");
                strData.AppendFormat("\"from\":\"" + "UnitTest" + "\",");
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
                return "done";
            }
            catch (Exception e)
            {
                return "Exception:" + e.Message;
            }
        }
        [HttpPost]
        public string delFromMSMQ(string mqTag, string mqid)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            if (mqTag == "0")
            {
                mqTag = "";
            }
            if (mqTag == "1")
            {
                mqTag = "1";
            }
            else if (mqTag == "2")
            {
                mqTag = "2";
            }
            else if (mqTag == "-1")
            {
                mqTag = "Final";
            }
            string strResult = "";
            string msmqName = ".\\private$\\imeCadFile" + mqTag;
            //连接到本地队列 
            MessageQueue myQueue = new MessageQueue(msmqName);
            myQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            myQueue.MessageReadPropertyFilter.Priority = true;
            int count = myQueue.GetAllMessages().Length;
            int countDel = 0;
            while (count > 0)
            {
                Message myMessage = myQueue.Receive();
                count--;
                //如果该Label是符合当前文本框可以接受的Id，删除该Msg
                if (myMessage.Id != mqid)
                {
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
                }
                else
                {
                    countDel++;
                }
            }
            if (countDel > 0)
            {
                strResult = "共从队列" + msmqName + "中移除" + countDel + "条记录";
            }
            else
            {
                strResult = "";
            }

            return strResult;
        }

     
        // https://192.168.255.103:2019/UnitTest/delHandle?code=0
        public string delHandle(string code)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            CADOperator co = new CADOperator();
            return co.deleteHandleByCode(code);
        }
    }
}
