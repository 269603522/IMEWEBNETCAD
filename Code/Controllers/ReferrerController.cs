using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMEWebCAD.Controllers
{
    public class ReferrerController : Controller
    {
        //
        // GET: /Referrer/

        public string Index()
        {
            
            string strIpInfo = "";
            strIpInfo += "<table border='1'>";
            if (Request.UrlReferrer != null)
            {

                    strIpInfo += "<tr>";
                    strIpInfo += "<td>" + "来源情况" + "</td>" + "<td>合法</td>";
                    strIpInfo += "</tr>";
                    strIpInfo += "<tr>";
                    strIpInfo += "<td>Request.UrlReferrer.AbsoluteUri</td><td>" + Request.UrlReferrer.AbsoluteUri + "</td>";
                    strIpInfo += "</tr>";
                    strIpInfo += "<tr>";
                    strIpInfo += "<td>Request.UrlReferrer.AbsolutePath</td><td>" + Request.UrlReferrer.AbsolutePath + "</td>";
                    strIpInfo += "</tr>";
                    strIpInfo += "<tr>";
                    strIpInfo += "<td>Request.UrlReferrer.Authority</td><td>" + Request.UrlReferrer.Authority + "</td>";
                    strIpInfo += "</tr>";
                    strIpInfo += "<tr>";
                    strIpInfo += "<td>Request.UrlReferrer.Host</td><td>" + Request.UrlReferrer.Host + "</td>";
                    strIpInfo += "</tr>";

                    strIpInfo += "<tr>";
                    strIpInfo += "<td>Headers Authorization</td><td>" + Request.Headers.Get("Authorization") + "</td>";
                    strIpInfo += "</tr>";


            }
            else
            {
                strIpInfo += "<tr>";
                strIpInfo += "<td>" + "来源情况" + "</td>" + "<td>未知来源</td>";
                strIpInfo += "</tr>";
            }
            strIpInfo += "</table>";
            return strIpInfo;
        }

    }
}
