using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebCAD;
using System.Messaging;
using System.Configuration;
using IMEWebCAD.Common.WebNet;

namespace IMEWebCAD
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DrawingManager.Engine = DrawingEngine.CADNET;
            // DrawingManager.Engine = DrawingEngine.CADService;
            //string key = "18A7BB917853EA9783B849F17195661F120A9FDC321AD14CFE9A44FB50AAA25D5E09A3DBE4B6BC1BB83D483B1F76C991D630A1B388255158CF1DE5A13C0C68F2|A672B8D480A9D79F60256B20F2ED112F61393B3ECB40FD23614EC46B5E0305D1612F36AD82009C8BB3E7D3A1E91F6A0A8BC4D92063B6D9341B2D784312BD57F|";
           // key = "22D5F4BF34716A83D84D66E03C9BA998802EF2BB61606A685B071BCC4CD083D94238FC54D1C0A605B24C065CCAE17928CCEE200F60C0BF803DBC22732BE3F482|22C9A043D3E423E6E9E794029F754DBE66795B8D4955CB12FFCC3EFDAD6E45972B3E9C00E6915F201083ED7CFC7935229A564E23A126E5299BF6BA6EBC5ACF6D|";
            //CADControl.Register("jibaozhi", "jibaozhi@imefuture.com", key);
            string IMEWebCadToolKey = ConfigurationManager.AppSettings["IMEWebCadToolKey"].ToString();
            string IMEWebCadToolName = ConfigurationManager.AppSettings["IMEWebCadToolName"].ToString();
            string IMEWebCadToolEmail = ConfigurationManager.AppSettings["IMEWebCadToolEmail"].ToString();
            CADControl.Register(IMEWebCadToolName, IMEWebCadToolEmail, IMEWebCadToolKey);
            initMSMQ();
        }
        private void initMSMQ()
        {
            //MessageQueue mq;
            ////新建消息循环队列或连接到已有的消息队列
            //string path = ".\\private$\\imeCadFile";
            //mq = MessageQueue.Exists(path) ? new MessageQueue(path) : MessageQueue.Create(path);
            //mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            //mq.BeginReceive();

        }
    }
}