using Common.Encrypt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMEWebCAD.Controllers
{
    public class TestJavaSiteController : Controller
    {
        //
        // GET: /TestJavaSite/

        public ActionResult Index()
        {
            return View();
        }
        // http://192.168.255.103:2018/TestJavaSite/checkDrawToken
        /// <summary>
        /// 图纸操作token验证
        /// 请求URL： http://192.168.85.85:8082/imefuture_drawingcenter_web/drawOptIf/checkDrawToken
        /// </summary>
        /// <param name="adId">图纸云的图纸id</param>
        /// <param name="tokenType">token的类型 1:大图展示token, 2: 大图流获取(下载)token, 3: 缩略图流获取(下载)token</param>
        /// <param name="token">url上获取的token</param>
        /// <returns></returns>
        [HttpPost]
        public string checkDrawToken(string adId, string tokenType, string token)
        {
            string strResult = "{\"code\": \"200\",\"message\": \"\"}";
            return strResult;
        }
        [HttpPost]
        public string drawTransferCallback(string enterpriseNo, string data)
        {
            string strResult = "{\"code\": \"200\",\"message\": \"\"}";
            //string IMEJavaKey = ConfigurationManager.AppSettings["IMEJavaKey"].ToString();
            //WriteLog("input data:" + data);
            //try
            //{
            //    //data = HttpUtility.UrlDecode(data);
            //    //WriteLog("step2:" + data);
            //    //data = data.Replace("%2b", "+");
            //    //WriteLog("step3:" + data);
            //    string TempCode = IMEAESTool.Decrypt(data, IMEJavaKey);
            //    WriteLog(enterpriseNo + "|" + data + "|" + TempCode);
            //}
            //catch(Exception ee)
            //{
            //    WriteLog("Exception:"+ee.Message+"|"+ data);
            //}

            return strResult;
        }
        public void WriteLog(string logTxt)
        {
            string serviceLogFileDir = "e:\\TestLog\\";
            string serviceLogFilePath = "e:\\TestLog\\TestLog" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
            if (!Directory.Exists(serviceLogFileDir))
            {
                Directory.CreateDirectory(serviceLogFileDir);
            }
            if (!System.IO.File.Exists(serviceLogFilePath))
            {
                System.IO.File.Create(serviceLogFilePath).Close();
            }
            using (FileStream stream = new FileStream(serviceLogFilePath, FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"{DateTime.Now}," + logTxt + "");
                writer.Close();
            }
        }
        // https://192.168.255.103:2019/TestJavaSite/testEncryptData
        public string testEncryptData()
        {
            string data = "{\"adId\":\"imeTestFile_20190529_8_0\",\"tokenType\":\"1\",\"token\":\"test\"}";
            string TempSrcCode = IMEAESTool.Encrypt(data, "123");
            string TempCode = IMEAESTool.Decrypt(TempSrcCode, "123");
            return data + "<br/>" + TempSrcCode + "<br/>" + TempCode;
        }
    }
}
