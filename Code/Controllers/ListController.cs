using Business;
using CADImport;
using CADImport.DWG;
using Code.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebCAD;

namespace IMEWebCAD.Controllers
{
    [HandleError]
    public class ListController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //https://localhost:2019/List/HandleList
        public ActionResult HandleList()
        {
            return View();
        }
        //https://localhost:2019/List/getHandleList/?pageIndex=2&pageSize=10
        public string getHandleList(string pageIndex,string pageSize)
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            CADOperator co = new CADOperator();

            return co.getHandleList(pageIndex, pageSize, " order by id desc  ");
        }
        //https://localhost:2019/List/ReadingList
        public ActionResult ReadingList()
        {
            return View();
        }
        [HttpPost]
        //https://localhost:2019/List/getReadingList/?pageIndex=1&pageSize=10
        public string getReadingList()
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string draw = Request.Form["draw"];
            string start = Request.Form["start"];
            string length = Request.Form["length"];
            double indexPage = double.Parse(start)/ double.Parse(length);
            decimal pageIndex = Math.Ceiling((decimal)indexPage)+1;
            string key = Request.Form["search[value]"];
            CADOperator co = new CADOperator();

            return co.getReadingList(draw,pageIndex + "", length, "order by id desc", key);
        }
        [HttpPost]
        //https://localhost:2019/List/deleteReadingByCode/
        public string deleteReadingByCode()
        {
            string IMEWebCadModle = ConfigurationManager.AppSettings["IMEWebCadModle"].ToString();
            if (IMEWebCadModle != "dev")
            {
                return "";
            }
            string code = Request.Form["code"];
            CADOperator co = new CADOperator();
            return co.deleteReadingByCode(code);
        }
    }
}
