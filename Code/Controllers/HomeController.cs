using CADImport;
using CADImport.DWG;
using Code.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.DrawingFile = Path.Combine(Server.MapPath("~/App_Data"), "Gasket.dwg");
            ViewBag.DrawingID = DrawingManager.Add(null, ViewBag.DrawingFile).Id;
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var dir = Directory.CreateDirectory(Server.MapPath("~/files/uploads/")).FullName;
                var path = Path.Combine(dir+"/cad/", fileName);
                file.SaveAs(path);
                ViewBag.DrawingFile = path;
                ViewBag.DrawingID = DrawingManager.Add(null, path).Id;

                //DrawingState ds = DrawingManager.Get(ViewBag.DrawingID);
                //Bitmap bmp = new Bitmap((int)300, (int)300);
   
                //double[,] matrix =new double[4, 4];
                //System.Drawing.Rectangle rectangle = new Rectangle(0,0,300,300);
                //ds.Drawing.Draw(ref bmp, rectangle, Color.Transparent, Color.Transparent);
                //string strThumbnailName = "Thumbnail" + DateTime.Now.Ticks + "_300_300.bmp";
                //var pathPic = Path.Combine(dir + "/thumbnail/", strThumbnailName);
                //bmp.Save(pathPic, ImageFormat.Bmp);
                ViewBag.DrawingFile = path;
                ViewBag.DrawingID = DrawingManager.Add(null, ViewBag.DrawingFile).Id;
               // ViewBag.Thumbnail = strThumbnailName;
                return View("Index");
            }

            return View();
        }

        public ActionResult LoadFromHDD()
        {
            ViewBag.DrawingFile = Path.Combine(Server.MapPath("~/App_Data"), "ford_cobra.dwg");
            ViewBag.DrawingID = DrawingManager.Add(null, ViewBag.DrawingFile).Id;
            //////////////////////////////////////////////////////////////////////////////
            /////DWGImage class is used only for reading DWG. To read other formatsuse the corresponding classes. 
            //E.g. for DXF: CADImage class, for PLT/HPGL: HPGLImage class.
            DWGImage vDrawing = new DWGImage();
            vDrawing.LoadFromFile(Path.Combine(Server.MapPath("~/App_Data"), "ford_cobra.dwg"));
            //vDrawing.Draw(Image1.CreateGraphics(), new RectangleF(0, 0,(float)vDrawing.AbsWidth, (float)vDrawing.AbsHeight));
            //vDrawing.Draw(DrawingManager.SHXDefaultPath., new RectangleF(0, 0, (float)vDrawing.AbsWidth, (float)vDrawing.AbsHeight));
            // zooming and panning of the drawing are implemented in the demo Viewer
            // via a special viewing control class CADPictureBox

            //////////////////////////////////////////////////////////////////////////////
            return View("Index");
        }

        public ActionResult LoadFromWeb()
        {
            string file = Path.GetTempPath() + Guid.NewGuid().ToString() + "_floorplan.dwg";
            string url = "https://www.cadsofttools.com/dwgviewer/floorplan.dwg";
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, file);

                ViewBag.DrawingFile = file;
                ViewBag.DrawingID = DrawingManager.Add(null, ViewBag.DrawingFile).Id;
            }
            //string connetStr = "server=127.0.0.1;port=3306;user=jimi;password=jimi; database=test;";
            //IDbConnection conn = new SqlConnection(connetStr);
            //string selectSql = "SELECT * FROM t_test";
            //List<t_test> basNameList = conn.Query<t_test>(selectSql).ToList();
            //string str="总共查询到："+basNameList.Count()+"条记录";
            return View("Index");
        }
       


        public struct AttributesExport
        {
            public string BlockName;
            public Dictionary<string, string> Tags;
        }

        public ActionResult GetCSV(string guid)
        {
            string resp = "";

            DrawingState ds = DrawingManager.Get(guid);
            if (ds != null)
            {
                List<AttributesExport> toExcel = new List<AttributesExport>();

                if (DrawingManager.Engine == DrawingEngine.CADNET)
                {
                    CADImage cadImage = ds.Drawing.GetInstance() as CADImage;
                    foreach (CADEntity ent in cadImage.Converter.Entities)
                        if ((ent is CADInsert) && ((ent as CADInsert).Attribs.Count == 3))
                        {
                            AttributesExport atrExp = new AttributesExport();
                            atrExp.Tags = new Dictionary<string, string>();
                            foreach (CADAttrib attr in (ent as CADInsert).Attribs)
                                atrExp.Tags.Add(attr.Tag, attr.Value);
                            atrExp.BlockName = (ent as CADInsert).Block.Name;
                            toExcel.Add(atrExp);
                        }
                }
                else
                {
                    string xml = ds.Drawing.ProcessXML("<?xml version=\"1.0\" encoding=\"UTF-8\"?><cadsofttools version=\"2\"><get mode=\"5\" /></cadsofttools>");

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    XmlNodeList nodes = doc.SelectNodes("//cstInsert");

                    foreach (XmlNode node in nodes)
                        if (node.ChildNodes.Count == 3)
                        {
                            AttributesExport attrExp = new AttributesExport();
                            attrExp.Tags = new Dictionary<string, string>();
                            XmlAttribute attr = node.Attributes["BlockName"];
                            if (attr != null)
                                attrExp.BlockName = attr.Value;
                            foreach (XmlNode cNode in node.ChildNodes)
                            {
                                XmlAttribute atrTag = cNode.Attributes["Tag"];
                                XmlAttribute atrValue = cNode.Attributes["Value"];
                                if ((atrTag != null) && (atrValue != null))
                                    attrExp.Tags.Add(atrTag.Value, atrValue.Value);
                            }
                            toExcel.Add(attrExp);
                        }
                }

                foreach (AttributesExport attr in toExcel)
                {
                    resp += attr.BlockName + "; ";
                    foreach (var tag in attr.Tags)
                        resp += tag.Key + "; " + tag.Value + "; ";
                    resp += "\r\n";
                }
            }
            Response.AddHeader("Content-Disposition", "attachment;filename=attribs.csv");
            return Content(resp, "text/csv");
        }
        #region Dapper test
        //https://localhost:56928/Home/testMySqlConnection
        public string testMySqlConnection()
        {
            string strResult = "";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            // server=127.0.0.1/localhost 代表本机，端口号port默认是3306可以不写
            MySqlConnection conn = new MySqlConnection(connetStr);
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            try
            {
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                strResult+="已经建立连接";
                cmd = new MySqlCommand("select * from t_gamer LIMIT 5;", conn); //③使用指定的SQL命令和连接对象创建SqlCommand对象  
                reader = cmd.ExecuteReader(); //④执行Command的ExecuteReader()方法  
                                              //⑤将DataReader绑定到数据控件中   
                DataTable dt = new DataTable();
                dt.Load(reader);
                strResult += "<br/>dt.Rows.Count:" + dt.Rows.Count;
                //⑥关闭DataReader   
                reader.Close();
            }
            catch (Exception e)
            {
                strResult += "<br/>Exception:" + e.Message;
            }
            finally
            {

                //⑦关闭连接   
                conn.Close();
                strResult += "<br/>关闭连接";
            }
            return strResult;
        }
        //https://localhost:56928/Home/testDapperSelect
        public string testDapperSelect()
        {
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            IDbConnection conn2 = new MySqlConnection(connetStr);
            string selectSql = "SELECT * FROM t_test";
            List<t_test> basNameList = conn2.Query<t_test>(selectSql).ToList();
            string str = "总共查询到：" + basNameList.Count() + "条记录";
            return str;
        }
       // https://localhost:56928/Home/testDapperInsert
        public string testDapperInsert()
        {
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            IDbConnection conn = new MySqlConnection(connetStr);
            string tempName = "小王" + DateTime.Now.Ticks;
            string insetSql = "INSERT t_test( NAME)VALUES('" + tempName + "')";
            var result = conn.Execute(insetSql, null);
            string str = "添加成功：" + result+"["+ tempName + "]";
            return str;
        }
       // https://localhost:56928/Home/testDapperInsert2
        public string testDapperInsert2()
        {
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            IDbConnection conn = new MySqlConnection(connetStr);
            string tempName = "小陈" + DateTime.Now.Ticks;
            string insetSql = "INSERT t_test( NAME)VALUES(@name)";
            var result = conn.Execute(insetSql, new { NAME = tempName });
            string str = "添加成功：" + result + "[" + tempName + "]";
            return str;
        }
        // https://localhost:56928/Home/testDapperInsert3
        public string testDapperInsert3()
        {
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            IDbConnection conn = new MySqlConnection(connetStr);
            string tempName = "小吴" + DateTime.Now.Ticks;
            string insetSql = "INSERT t_test( NAME)VALUES(@name)";
            t_test obj_t_test = new t_test();
            obj_t_test.name = tempName;
            var result = conn.Execute(insetSql, obj_t_test);
            string str = "添加成功：" + result + "[" + tempName + "]";
            return str;
        }
        // https://localhost:56928/Home/testDapperUpdate
        public string testDapperUpdate()
        {
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            IDbConnection conn = new MySqlConnection(connetStr);
            string tempName = "小李" + DateTime.Now.Ticks;
            string updateSql = "update t_test set NAME=CONCAT_WS('',id,'"+ tempName + "','te')    where name='小李636893628359866028+id' ";
            var result = conn.Execute(updateSql, null);
            string str = "修改成功：" + result + "[" + tempName + "]";
            return str;
        }
        // https://localhost:56928/Home/testProc
        public string testProc()
        {
            string str = "";
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            MySqlConnection myConn = new MySqlConnection(connetStr);
            MySqlCommand myComm = new MySqlCommand("proc_add", myConn);
            try
            {
                myComm.Connection.Open();
                myComm.CommandType = CommandType.StoredProcedure;
                MySqlParameter myParameter;
                myParameter = new MySqlParameter("@a", MySqlDbType.Int32);
                myParameter.Value = 2;
                myParameter.Direction = ParameterDirection.Input;
                myComm.Parameters.Add(myParameter);
                myParameter = new MySqlParameter("@b", MySqlDbType.Int32);
                myParameter.Value = 5;
                myParameter.Direction = ParameterDirection.Input;
                myComm.Parameters.Add(myParameter);
                int result= myComm.ExecuteNonQuery();
                str += "结果值：" + result;
            }
            catch(Exception e)
            {
                str += "异常：" + e.Message;
                myComm.Connection.Close();
                myComm.Dispose();
            }
            finally
            {
                myComm.Connection.Close();
                myComm.Dispose();
            }
            return str;
        }
        public string testList()
        {
            string connetStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            //MySqlDAO msd = new MySqlDAO(connetStr);
            IDbConnection conn = new MySqlConnection(connetStr);
            string tempName = "小李" + DateTime.Now.Ticks;
            string updateSql = "update t_test set NAME=CONCAT_WS('',id,'" + tempName + "','te')    where name='小李636893628359866028+id' ";
            var result = conn.Execute(updateSql, null);
            string str = "修改成功：" + result + "[" + tempName + "]";
            return str;
        }
        #endregion
    }
}
