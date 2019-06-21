using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace IMEWebCAD
{
    public class WebAutoCADFilter : System.Web.Mvc.IAuthorizationFilter, System.Web.Mvc.IActionFilter, IResultFilter, System.Web.Mvc.IExceptionFilter
    {

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            string referer = "";
            if (filterContext.RequestContext.HttpContext.Request.UrlReferrer != null)
            {
                referer = filterContext.RequestContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
            }
            string strIP = filterContext.RequestContext.HttpContext.Request.ServerVariables.Get("Remote_Addr");
            string strTDs = "<td>" + DateTime.Now + "</td>";
            strTDs += "<td>" + strIP + "</td>";
            strTDs += "<td>" + filterContext.Result.ToString() + "</td>";
            strTDs += "<td>" + actionName + "</td>";
            strTDs += "<td>" + filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri + "</td>";
            strTDs += "<td>" + referer + "</td>";
            WriteLog("<tr>" + strTDs + "</tr>", "ActionLog");
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            Console.WriteLine("WebAutoCADFilter[OnActionExecuting]:" + actionName);
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            Console.WriteLine("WebAutoCADFilter[OnAuthorization]:" + actionName);
        }

        public void OnException(ExceptionContext filterContext)
        {
            Exception innerException = filterContext.Exception;
            string strHttpContext = filterContext.HttpContext.Request.UserAgent;
            Console.WriteLine("WebAutoCADFilter[OnException]:" + strHttpContext);
            string referer = "";
            if (filterContext.RequestContext.HttpContext.Request.UrlReferrer != null)
            {
                referer = filterContext.RequestContext.HttpContext.Request.UrlReferrer.Host + "|" + filterContext.RequestContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
            }
            string strIP = filterContext.RequestContext.HttpContext.Request.ServerVariables.Get("Remote_Addr");
            string strTDs = "<td>" + DateTime.Now + "</td>";
            strTDs += "<td>" + strIP + "</td>";
            strTDs += "<td>" + filterContext.Result.ToString() + "</td>";
            strTDs += "<td>" + innerException.Message + "</td>";
            strTDs += "<td>" + filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri + "</td>";
            strTDs += "<td>" + referer + "</td>";
            WriteLog("<tr>" + strTDs + "</tr>", "ExceptionLog");

        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string strHttpContext = filterContext.Result.ToString();
            if (filterContext.Result is ContentResult)
            {
                ContentResult result = (ContentResult)filterContext.Result;
                ////序列化json数据
                //strHttpContext = JsonConvert.SerializeObject(result.Content);
                ////输出新的数据
                //filterContext.HttpContext.Response.Write(strHttpContext);
                strHttpContext = result.Content;
                Console.WriteLine("[OnResultExecuted]:" + strHttpContext);
            }
            else
            {
                Console.WriteLine("[Executed]:" + strHttpContext);
            }

        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            string strHttpContext = filterContext.Result.ToString();

            Console.WriteLine("WebAutoCADFilter[OnResultExecuting]:" + strHttpContext);
        }
        public void WriteLog(string logTxt, string subdir)
        {
            try
            {
                string webLogDir = ConfigurationManager.AppSettings["webLogDir"].ToString();
                webLogDir = webLogDir + "/" + subdir + "/";
                if (!Directory.Exists(webLogDir))
                {
                    Directory.CreateDirectory(webLogDir);
                }
                string serviceLogFilePath = webLogDir + "\\ActionLog" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                using (FileStream stream = new FileStream(serviceLogFilePath, FileMode.Append))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(logTxt + "");
                }
            }
            catch
            {

            }

        }
    }
}