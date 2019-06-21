using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMEWebCAD.Common.WebNet
{
    public static class NetHelper
    { 
            #region Ip(获取Ip)
            /// <summary>
            /// 获取Ip
            /// </summary>
            public static string Ip
            {
                get
                {
                    var result = string.Empty;
                
                    if (string.IsNullOrWhiteSpace(result))
                        result = GetLanIp();
                    if (HttpContext.Current != null)
                    result = GetWebClientIp();
                     return result;
                }
            }
        public static string LocalIp
        {
            get
            {
                var result = string.Empty;

                if (string.IsNullOrWhiteSpace(result))
                    result = GetLanIp();
                return result;
            }
        }
        /// <summary>
        /// 获取Web客户端的Ip
        /// </summary>
        /// <returns></returns>
        private static string GetWebClientIp()
            {
                var ip = GetWebRemoteIp();
                foreach (var hostAddress in Dns.GetHostAddresses(ip))
                {
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                        return hostAddress.ToString();
                }
                return string.Empty;
            }
            /// <summary>
            /// 获取Web远程Ip
            /// </summary>
            /// <returns></returns>
            private static string GetWebRemoteIp()
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            /// <summary>
            /// 获取局域网IP
            /// </summary>
            /// <returns></returns>
            private static string GetLanIp()
            {
                string ip = string.Empty;
                foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                        //return hostAddress.ToString();
                        ip = ip + ";" + hostAddress.ToString();
                }
                if (!string.IsNullOrEmpty(ip) && ip.Contains(";"))
                {
                    ip = ip.Substring(1);
                }
                return ip;
            }
            #endregion


            #region Host(获取主机名)
            /// <summary>
            /// 获取主机名
            /// </summary>
            public static string Host
            {
                get
                {
                    return HttpContext.Current == null ? Dns.GetHostName() : GetWebClientHostName();
                }
            }
            /// <summary>
            /// 获取Web客户端主机名
            /// </summary>
            /// <returns></returns>
            private static string GetWebClientHostName()
            {
                if (!HttpContext.Current.Request.IsLocal)
                    return string.Empty;
                var ip = GetWebRemoteIp();
                var result = Dns.GetHostEntry(IPAddress.Parse(ip)).HostName;
                if (result == "localhost.localdomain")
                    result = Dns.GetHostName();
                return result;
            }


            #endregion


            #region Browser(获取浏览器信息)
            /// <summary>
            /// 获取浏览器信息
            /// </summary>
            public static string Browser
            {
                get
                {
                    if (HttpContext.Current == null)
                        return string.Empty;
                    var browser = HttpContext.Current.Request.Browser;
                    return string.Format("{0} {1}", browser.Browser, browser.Version);
                }
            }
        #endregion
        #region 获取客户端指纹信息列表
        public static string GetClientWebInfo()
        {
            string strIpInfo = "";
            strIpInfo += "<table border='1'>";
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                strIpInfo += "<tr>";
                strIpInfo += "<td>Request.UrlReferrer.AbsoluteUri</td><td>" + HttpContext.Current.Request.UrlReferrer.AbsoluteUri + "</td>";
                strIpInfo += "</tr>";
                strIpInfo += "<tr>";
                strIpInfo += "<td>Request.UrlReferrer.AbsolutePath</td><td>" + HttpContext.Current.Request.UrlReferrer.AbsolutePath + "</td>";
                strIpInfo += "</tr>";
                strIpInfo += "<tr>";
                strIpInfo += "<td>Request.UrlReferrer.Authority</td><td>" + HttpContext.Current.Request.UrlReferrer.Authority + "</td>";
                strIpInfo += "</tr>";
                strIpInfo += "<tr>";
                strIpInfo += "<td>Request.UrlReferrer.Host</td><td>" + HttpContext.Current.Request.UrlReferrer.Host + "</td>";
                strIpInfo += "</tr>";
            }
            else
            {
                strIpInfo += "<tr>";
                strIpInfo += "<td>" + "Request.UrlReferrer" + "</td>" + "<td>目前为空</td>";
                strIpInfo += "</tr>";
            }
            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "获取客户端IP:" + "</td>" + "<td>" + HttpContext.Current.Request.ServerVariables.Get("Remote_Addr") + "</td>";
            strIpInfo += "</tr>";

            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "获取客户端主机名:" + "</td>" + "<td>" + HttpContext.Current.Request.ServerVariables.Get("Remote_Host") + "</td>";
            strIpInfo += "</tr>";

            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "客户端浏览器:" + "</td>" + "<td>" + HttpContext.Current.Request.Browser.Browser + "</td>";
            strIpInfo += "</tr>";

            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "客户端浏览器 版本号:" + "</td>" + "<td>" + HttpContext.Current.Request.Browser.MajorVersion + "</td>";
            strIpInfo += "</tr>";

            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "客户端操作系统:" + "</td>" + "<td>" + HttpContext.Current.Request.Browser.Platform + "</td>";
            strIpInfo += "</tr>";

            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "获取服务器IP:" + "</td>" + "<td>" + HttpContext.Current.Request.ServerVariables.Get("Local_Addr") + "</td>";
            strIpInfo += "</tr>";

            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "获取服务器名:" + "</td>" + "<td>" + HttpContext.Current.Request.ServerVariables.Get("Server_Name") + "</td>";
            strIpInfo += "</tr>";
            strIpInfo += "<tr>";
            strIpInfo += "<td>" + "浏览器信息:" + "</td>" + "<td>" + HttpContext.Current.Request.UserAgent + "</td>";
            strIpInfo += "</tr>";
            strIpInfo += "</table>";
            return strIpInfo;
        }
        #endregion
        public static string getBrowserInfo()
        {
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string s = "Browser Capabilities\n"
                + "Type = " + browser.Type + "\n"
                + "Name = " + browser.Browser + "\n"
                + "Version = " + browser.Version + "\n"
                + "Major Version = " + browser.MajorVersion + "\n"
                + "Minor Version = " + browser.MinorVersion + "\n"
                + "Platform = " + browser.Platform + "\n"
                + "Is Beta = " + browser.Beta + "\n"
                + "Is Crawler = " + browser.Crawler + "\n"
                + "Is AOL = " + browser.AOL + "\n"
                + "Is Win16 = " + browser.Win16 + "\n"
                + "Is Win32 = " + browser.Win32 + "\n"
                + "Supports Frames = " + browser.Frames + "\n"
                + "Supports Tables = " + browser.Tables + "\n"
                + "Supports Cookies = " + browser.Cookies + "\n"
                + "Supports VBScript = " + browser.VBScript + "\n"
                + "Supports JavaScript = " +
                    browser.EcmaScriptVersion.ToString() + "\n"
                + "Supports Java Applets = " + browser.JavaApplets + "\n"
                + "Supports ActiveX Controls = " + browser.ActiveXControls
                      + "\n"
                + "Supports JavaScript Version = " +
                    browser["JavaScriptVersion"] + "\n";
            return s;
        }
        /// <summary>
        ///  复制当前 协议+host+端口 字符串
        /// </summary>
        /// <returns></returns>
        public static string CopyCurrentHost()
        {
            string postUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port ;
            return postUrl;
        }
        /// <summary>
        /// POST 远程服务
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string PostWebRequest(string postUrl, string paramData)
        {
            string ret = string.Empty;

            byte[] byteArray = Encoding.Default.GetBytes(paramData); //转化
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = byteArray.Length;
            Stream newStream = webReq.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);//写入参数
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            ret = sr.ReadToEnd();
            sr.Close();
            response.Close();
            newStream.Close();
            return ret;
        }
        /// <summary>
        /// POST 远程服务
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string PostJsonWebRequest(string postUrl, string paramData)
        {
            string ret = string.Empty;

            byte[] byteArray = Encoding.Default.GetBytes(paramData); //转化
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            webReq.Method = "POST";
            webReq.ContentType = "application/json";
            webReq.ContentLength = byteArray.Length;
            Stream newStream = webReq.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);//写入参数
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            ret = sr.ReadToEnd();
            sr.Close();
            response.Close();
            newStream.Close();
            return ret;
        }
    }
}
