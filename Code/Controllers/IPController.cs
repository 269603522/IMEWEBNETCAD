using CADImport;
using CADImport.RasterImage;
using Common.Encrypt;
using IMEWebCAD.Common.Encrypt;
using IMEWebCAD.Common.WebNet;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebCAD;


namespace IMEWebCAD.Controllers
{
    public class IPController : Controller
    {
        //
        // GET: /IP/

        public string Index()
        {
            string strIpInfo = "";
            strIpInfo = NetHelper.getBrowserInfo();
            ViewData["UserInfo"] = strIpInfo;

            string webURLOfLocalSitePort = ConfigurationManager.AppSettings["webURLOfLocalSitePort"].ToString();
            string webURLOfLocalSite = Request.ServerVariables.Get("Local_Addr").ToString();
            if(webURLOfLocalSite== "::1")
            {
                webURLOfLocalSite = "localhost";
            }
            string tempFileTimeoutNum = ConfigurationManager.AppSettings["tempFileTimeoutNum"].ToString();
            string webURLOfPublicSite = ConfigurationManager.AppSettings["webURLOfPublicSite"].ToString();
            string webURLOfPublicSitePort = ConfigurationManager.AppSettings["webURLOfPublicSitePort"].ToString();

            

            strIpInfo += "<Br/>公网地址：" + webURLOfPublicSite;
            strIpInfo += "<Br/>内网地址：" + webURLOfLocalSite;
            strIpInfo += "<Br/>内网端口：" + webURLOfLocalSitePort;
            strIpInfo += "<Br/>公网端口：" + webURLOfPublicSitePort;
            strIpInfo += "<Br/>超时分值：" + tempFileTimeoutNum;
            string strReferrerUrl = "https://" + webURLOfLocalSite + ":" + webURLOfLocalSitePort + "/Referrer/";
            strIpInfo += "<Br/>来源测试:<a href='" + strReferrerUrl + "'>"+ strReferrerUrl + "</a>";
            strIpInfo += "<Br/>EncodedCredentials：" + GetEncodedCredentials();

            string strReferrerUrlPostFile = "https://" + webURLOfLocalSite + ":" + webURLOfLocalSitePort + "/PostFile.html";
            strIpInfo += "<Br/>测试传文件接口:<a href='" + strReferrerUrlPostFile + "'>" + strReferrerUrlPostFile + "</a>";
            //Request.Headers.Get["Authorization"];
            //Request.Headers.GetKey("Authorization");
            string strReferrerUrlPath = "https://" + webURLOfLocalSite + ":" + webURLOfLocalSitePort + "/Path.html";
            strIpInfo += "<Br/>测试传路径接口:<a href='" + strReferrerUrlPath + "'>" + strReferrerUrlPath + "</a>";
            return strIpInfo;
        }

        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", "Jimi", "Jimiao");
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
        //https://localhost:2019/ip/testReflection
        public string testReflection()
        {

            string strResult = "";
            #region 反射查看内容
            string pathCADFileId = DrawingManager.Add(null, @"E:\jimi\work\NET CAD\IMEWebCAD\Code\App_Data\seal\seal1.dwg").Id;
            strResult += "<br/>CAD文件插件ID:" + pathCADFileId;
            DrawingState ds = DrawingManager.Get(pathCADFileId);
            Type tempType = ds.Drawing.GetInstance().GetType();


            //2.获取Person类中的所有的方法

            //（通过Type对象的GetMethods()可以获取指定类型的所有的方法其中包括编译器自动生成的方法以及从父类中继承来的方法，但是不包含private方法）
            strResult += "<br/><h1>Methods:</h1>";
            MethodInfo[] methods = tempType.GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                strResult += "<br/>" + methods[i].Name;
                Console.WriteLine(methods[i].Name);
            }



            //3.获取某个类型的所有属性
            strResult += "<br/><h1>Properties:</h1>";
            PropertyInfo[] properties = tempType.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                strResult += "<br/>" + properties[i].Name;
                Console.WriteLine(properties[i].Name);
            }
            //Console.ReadKey();


            //4.获取类中的所有字段,私有字段无法获取
            strResult += "<br/><h1>Fields:</h1>";
            FieldInfo[] fields = tempType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                strResult += "<br/>" + fields[i].Name;
                Console.WriteLine(fields[i].Name);
            }
            //Console.ReadKey();


            //5.获取所有成员，不包含私有成员
            strResult += "<br/><h1>Members:</h1>";
            MemberInfo[] members = tempType.GetMembers();
            for (int i = 0; i < members.Length; i++)
            {
                strResult += "<br/>" + members[i].Name;
                Console.WriteLine(members[i].Name);
            }
            List<WebCAD.LayerData> listText = ds.Drawing.GetLayers();
            if (listText != null)
            {
                foreach (var item in listText)
                {
                    strResult += "<br/>[" + item.Name + "]:" + item.GetType();
                }
            }
            else
            {
                strResult += "<br/>未找到ds.Drawing.GetInitData()信息";
            }
            #endregion
            return strResult;
        }
       // https://localhost:2019/ip/testGetTextCount
        public string testGetTextCount()
        {
            string strResult = "";

            DrawingState ds = DrawingManager.Get(DrawingManager.Add(null, @"E:\jimi\work\NET CAD\IMEWebCAD\Code\App_Data\budweiser.dwg").Id);
            if (ds == null) return "加载原始文件失败！";
            CADImage image = ds.Drawing.GetInstance() as CADImage;
            if (image == null) return "创建实例失败！";
            if (image is CADRasterImage) return "创建CADRasterImage实例失败！";

            int vCount = 0;
            for (int i = 0; i < image.CurrentLayout.Count; i++)
                if (image.CurrentLayout.Entities[i].EntType == EntityType.Text)
                    vCount++;
            strResult = "There are " + vCount + " Text entities on the current layout of the drawing";
            return strResult;
        }
        // https://localhost:2019/ip/testAccessLayers
        public string testAccessLayers()
        {
            string strResult = "";

            DrawingState ds = DrawingManager.Get(DrawingManager.Add(null, @"E:\jimi\work\NET CAD\IMEWebCAD\Code\App_Data\budweiser.dwg").Id);
            if (ds == null) return "加载原始文件失败！";
            CADImage image = ds.Drawing.GetInstance() as CADImage;
            if (image == null) return "创建实例失败！";
            if (image is CADRasterImage) return "创建CADRasterImage实例失败！";

            string S = "Layers contained in Entities.dxf : ";
            for (int i = 0; i < image.Converter.GetSection(CADImport.FaceModule.ConvSection.Layers).Count; i++)
                S = S + ((CADLayer)image.Converter.Layers[i]).Name + ", ";
            strResult = "Layers count: " + image.Converter.Layers.Count + " " + S;
            return strResult;
        }
        // https://localhost:2019/ip/testAddPic
        public string testAddPic()
        {
            string strResult = "";
            string srcSeal = @"E:\jimi\work\NET CAD\IMEWebCAD\Code\App_Data\seal\seal1.dwg";
            DrawingState ds_Seal = DrawingManager.Get(DrawingManager.Add(null, srcSeal).Id);
            if (ds_Seal == null) return "加载水印文件失败！";
            CADImage image_Seal = ds_Seal.Drawing.GetInstance() as CADImage;
            if (image_Seal == null) return "创建水印实例失败！";

            string srcFile = @"E:\jimi\work\NET CAD\IMEWebCAD\Code\Files\uploads\20190409\Out\CAD/71002小立柱油缸装配图.dwg";
            DrawingState dsTemp = DrawingManager.Get(DrawingManager.Add(null, srcFile).Id);
            CADImage image = dsTemp.Drawing.GetInstance() as CADImage;
            CADEntityCollection cadEntitys_Seal = image_Seal.CurrentLayout.Entities;
            if(cadEntitys_Seal!=null)
            {
                strResult += "印章图层元素数：" + cadEntitys_Seal.Count;
            }
            else
            {
                strResult += "印章图层无元素数" ;
            }
            CADEntityCollection cadEntitys = image.CurrentLayout.Entities;
            if (cadEntitys != null)
            {
                strResult += "当前图层元素数：" + cadEntitys.Count;
            }
            else
            {
                strResult += "当前图层无元素数";
            }
            double i_min_x = 1000000000000000;
            double i_min_y = 1000000000000000;
            double i_max_x = 0;
            double i_max_y = 0;
            foreach (CADEntity cadEntity in cadEntitys)
            {
                if (i_min_x > cadEntity.Box.left)
                {
                    i_min_x = cadEntity.Box.left;
                }
                if (i_min_y > cadEntity.Box.bottom)
                {
                    i_min_y = cadEntity.Box.bottom;
                }
                if (i_max_x < cadEntity.Box.right)
                {
                    i_max_x = cadEntity.Box.right;
                }
                if (i_max_y < cadEntity.Box.top)
                {
                    i_max_y = cadEntity.Box.top;
                }
                if(cadEntity.EntType== EntityType.Line)
                {
                    cadEntity.Color = Color.Blue;
                    cadEntity.LineWeight = 2;
                    image.Converter.Loads(cadEntity);
                }
            }
          
            //DPoint APos = new DPoint(i_min_x, i_min_y - 50, 0);
            //DPoint AScale = new DPoint(i_min_x+10000, i_min_y - 50+10000, 0);
            //image.AddScaledDXF(image_Seal, "Seal", APos, AScale, 10);
            if (cadEntitys_Seal != null)
            {
                foreach(CADEntity ent in cadEntitys_Seal)
                {
                    if(ent.EntType==EntityType.Ellipse)
                    {
                        CADEllipse tempEmtity =(CADEllipse) ent;
                        DPoint APos = new DPoint(i_min_x+ tempEmtity.Box.Center.X, i_min_y + tempEmtity.Box.Center.Y, 0);
                        tempEmtity.Point = APos;
                        image.CurrentLayout.AddEntity(tempEmtity);
                    }
                    else
                    if (ent.EntType == EntityType.MText)
                    {
                        CADMText tempEmtity = (CADMText)ent;
                        DPoint APos = new DPoint(i_min_x + tempEmtity.Box.left, i_min_y  + tempEmtity.Box.Center.Y, 0);
                        tempEmtity.Point = APos;
                        image.CurrentLayout.AddEntity(tempEmtity);
                    }

                }
            }
            DPoint APos_center = new DPoint((i_max_x -i_min_x)/2, (i_max_y - i_min_y)/2, 0);
            image.Center = APos_center;
            image.GetExtents();
            string strTestDxfName = "d:/TestDxfSeal" + DateTime.Now.Ticks + ".dxf";
            //保存文件
            dsTemp.Drawing.SaveAsDXF(strTestDxfName);
            strResult += "<br/>给文件：" + srcFile;
            strResult += "<br/>加上水印文件：" + srcSeal;
            strResult += "<br/>成功保存至：" + strTestDxfName;
            return strResult;
        }
        // https://localhost:2019/ip/testAddText?markTxt="智造家上海"
        public string testAddText(string markTxt)
        {
            string strResult = "";
            string srcFile = @"E:\jimi\work\NET CAD\IMEWebCAD\Code\Files\uploads\20190409\Out\CAD/71002小立柱油缸装配图.dwg";
            if(markTxt==null|| markTxt=="")
            {
                markTxt = "IMF Future ShangHai Office";
            }
            DrawingState dsTemp = DrawingManager.Get(DrawingManager.Add(null, srcFile).Id);
            CADImage image = dsTemp.Drawing.GetInstance() as CADImage;
            CADEntityCollection cadEntitys = image.CurrentLayout.Entities;

            double i_min_x = 1000000000000000;
            double i_min_y = 1000000000000000;
            foreach (CADEntity cadEntity in cadEntitys)
            {
                if (i_min_x > cadEntity.Box.left)
                {
                    i_min_x = cadEntity.Box.left;
                }
                if (i_min_y > cadEntity.Box.bottom)
                {
                    i_min_y = cadEntity.Box.bottom;
                }
            }
            CADText vText = new CADText();
            vText.Text = markTxt;
            vText.Height = 20;
            vText.Point = new DPoint(i_min_x, i_min_y - 50, 0);
            vText.Color = Color.FromName("#ef2381");
            if (image != null)
            {
                image.Converter.Loads(vText);
                image.CurrentLayout.AddEntity(vText);
                // recalculation of drawing extents
                image.GetExtents();
            }
            string strTestDxfName = "d:/TestDxf" + DateTime.Now.Ticks + ".dxf";
            //保存文件
            dsTemp.Drawing.SaveAsDXF(strTestDxfName);
            strResult += "<br/>给文件：" + srcFile;
            strResult += "<br/>加上水印文字：" + markTxt;
            strResult += "<br/>成功保存至：" + strTestDxfName;
            return strResult;
        }
        // https://localhost:2019/ip/testAddText2?markTxt="智造家上海"
        public string testAddText2(string markTxt)
        {
            string strResult = "";
            string srcFile = @"E:\jimi\work\NET CAD\测试CAD文件/71002小立柱油缸装配图.dwg";
            if (markTxt == null || markTxt == "")
            {
                markTxt = "IMF Future ShangHai Office";
            }
            DrawingState dsTemp = DrawingManager.Get(DrawingManager.Add(null, srcFile).Id);
            CADImage image = dsTemp.Drawing.GetInstance() as CADImage;
            CADEntityCollection cadEntitys = image.CurrentLayout.Entities;

            double i_min_x = 1000000000000000;
            double i_min_y = 1000000000000000;
            foreach (CADEntity cadEntity in cadEntitys)
            {
                if (i_min_x > cadEntity.Box.left)
                {
                    i_min_x = cadEntity.Box.left;
                }
                if (i_min_y > cadEntity.Box.bottom)
                {
                    i_min_y = cadEntity.Box.bottom;
                }
            }
            CADText vText = new CADText();
            vText.Text = markTxt;
            vText.Height = 20;
            vText.Point = new DPoint(i_min_x, i_min_y - 50, 0);
            vText.Color = Color.FromName("#ef2381");
            if (image != null)
            {
                image.Converter.Loads(vText);
                image.CurrentLayout.AddEntity(vText);
                image.Converter.Loads(vText);

            }
            string strTestDxfName = "d:/TestDxf" + DateTime.Now.Ticks + ".dxf";
            //保存文件
            dsTemp.Drawing.SaveAsDXF(strTestDxfName);
            strResult += "<br/>给文件：" + srcFile;
            strResult += "<br/>加上水印文字：" + markTxt;
            strResult += "<br/>成功保存至：" + strTestDxfName;
            return strResult;
        }

        // https://localhost:2019/ip/testGetPDFPic
        public string testGetPDFPic()
        {
            string  strResult = "";
            string filePath = @"E:\jimi\work\Rule\内部推荐奖励制.pdf";
            string localThumbnailDir = "D:/";

           
            string strThumbnailName = localThumbnailDir+"ThumbnailPDFappreciate" + DateTime.Now.Ticks + "_300_300.bmp";
            PdfDocument doc = new PdfDocument();

            doc.LoadFromFile(filePath);



            //遍历PDF每一页

            for (int i = 0; i < doc.Pages.Count; i++)

            {

                //将PDF页转换成Bitmap图形

                System.Drawing.Image bmp = doc.SaveAsImage(i);



                //将Bitmap图形保存为Png格式的图片

                string fileName = strThumbnailName+ string.Format("Page-{0}.png", i + 1);

                bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

            }
            return strResult;
        }
        /**//// <summary> 
            /// 生成缩略图 
            /// </summary> 
            /// <param name="originalImagePath">源图路径（物理路径）</param> 
            /// <param name="thumbnailPath">缩略图路径（物理路径）</param> 
            /// <param name="width">缩略图宽度</param> 
            /// <param name="height">缩略图高度</param> 
            /// <param name="mode">生成缩略图的方式</param>     
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                 
                    break;
                case "W"://指定宽，高按比例                     
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例 
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                 
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片 
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板 
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充 
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分 
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图 
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        public string testGetPDFPic2()
        {
            string strResult = "";
            string filePath = @"E:\jimi\work\Rule\内部推荐奖励制.pdf";
            string localThumbnailDir = "D:/";


            string strThumbnailName = localThumbnailDir + "ThumbnailPDFappreciate" + DateTime.Now.Ticks + "_300_300.bmp";
            PdfDocument doc = new PdfDocument();

            doc.LoadFromFile(filePath);




            //遍历PDF每一页

            for (int i = 0; i < doc.Pages.Count; i++)

            {

                //将PDF页转换成Bitmap图形

                System.Drawing.Image bmp = doc.SaveAsImage(i);



                //将Bitmap图形保存为Png格式的图片

                string fileName = strThumbnailName + string.Format("Page-{0}.png", i + 1);

                bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

            }
            return strResult;
        }
        // https://localhost:2019/ip/testAddPic2

        // https://localhost:2019/ip/testAddPic3
        /// <summary>
        /// 测试添加本地印章图片
        /// </summary>
        /// <returns></returns>
        public string testAddPic3()
        {
            string strResult = "";
            string srcFile = @"E:\jimi\work\NET CAD\WebCAD SDK 12\Demos\GettingStarted2008\Files/Gasket2.dwg";
            DrawingState dsTemp = DrawingManager.Get(DrawingManager.Add(null, srcFile).Id);
            CADImage image = dsTemp.Drawing.GetInstance() as CADImage;
            CADEntityCollection cadEntitys = image.CurrentLayout.Entities;
            double i_min_x = 1000000000000000;
            double i_min_y = 1000000000000000;
            double i_max_x = 0;
            double i_max_y = 0;
            foreach (CADEntity cadEntity in cadEntitys)
            {
                if (i_min_x > cadEntity.Box.left)
                {
                    i_min_x = cadEntity.Box.left;
                }
                if (i_min_y > cadEntity.Box.bottom)
                {
                    i_min_y = cadEntity.Box.bottom;
                }
                if (i_max_x < cadEntity.Box.right)
                {
                    i_max_x = cadEntity.Box.right;
                }
                if (i_max_y < cadEntity.Box.top)
                {
                    i_max_y = cadEntity.Box.top;
                }
            }
            string srcSeal = @"F:\ink.gif";
            CADImageDef imgDef = new CADImageDef();

            imgDef.FileName = srcSeal;

            if (new Bitmap(imgDef.FileName) != null)

            {

                image.Converter.Loads(imgDef);

                image.Converter.GetSection(CADImport.FaceModule.ConvSection.ImageDefs).AddEntity(imgDef);



                CADImageEnt imgEnt = new CADImageEnt();

                imgEnt.ImageDef = imgDef;

                imgEnt.Point = new DPoint(0, 0, 0);

                imgEnt.UVector = CADConst.XOrtAxis;

                imgEnt.VVector = CADConst.YOrtAxis;

                //imgEnt.Size = imgEnt.ImageDef.Size;
                imgEnt.Size = new DPoint((i_max_x- i_min_x)/4, (i_max_x - i_min_x) / 4, 0);


                image.Converter.OnCreate(imgEnt);

                image.CurrentLayout.Entities.Add(imgEnt);

                image.GetExtents();

                string strTestDxfName = "d:/TestDxfSeal" + DateTime.Now.Ticks + ".dxf";
                dsTemp.Drawing.SaveAsDXF(strTestDxfName);


                strResult += "<br/>给文件：" + srcFile;
                strResult += "<br/>成功保存至：" + strTestDxfName;

                dsTemp.Drawing.SaveAsDXF(strTestDxfName);

            }
           
            return strResult;
        }
        // https://localhost:2019/ip/testAddPic4
        /// <summary>
        /// 测试添加网上印章图片
        /// </summary>
        /// <returns></returns>
        public string testAddPic4()
        {
            string strResult = "";
           // string srcFile = Server.MapPath("") + @"\Files\Gasket2.dwg";
            string srcFile = @"E:\jimi\work\NET CAD\WebCAD SDK 12\Demos\GettingStarted2008\Files/Gasket2.dwg";
            DrawingState dsTemp = DrawingManager.Get(DrawingManager.Add(null, srcFile).Id);

            CADImage image = dsTemp.Drawing.GetInstance() as CADImage;



            string imageURL = "https://img.sccnn.com/simg/338/55581.jpg";

            Stream tmpStream = CADConst.LoadDataFromWeb(imageURL);

            string imageFilePath = Server.MapPath("/") + @"App_Data\" + Path.GetFileName(imageURL);



            FileStream fs = System.IO.File.OpenWrite(imageFilePath);

            try

            {

                tmpStream.CopyTo(fs);

            }

            finally

            {

                tmpStream.Close();

                fs.Close();

            }



            CADImageDef imgDef = new CADImageDef();

            imgDef.FileName = imageFilePath;

            if (new Bitmap(imgDef.FileName) != null)

            {

                image.Converter.Loads(imgDef);

                image.Converter.GetSection(CADImport.FaceModule.ConvSection.ImageDefs).AddEntity(imgDef);



                CADImageEnt imgEnt = new CADImageEnt();

                imgEnt.ImageDef = imgDef;

                imgEnt.Point = new DPoint(100, 100, 0);

                imgEnt.UVector = CADConst.XOrtAxis;

                imgEnt.VVector = CADConst.YOrtAxis;

                imgEnt.Size = imgEnt.ImageDef.Size;



                image.Converter.OnCreate(imgEnt);

                image.CurrentLayout.Entities.Add(imgEnt);

                image.GetExtents();

                string strTestDxfName = Server.MapPath("/") + @"App_Data\Gasket2.dxf";

                dsTemp.Drawing.SaveAsDXF(strTestDxfName);

            }
            return strResult;
        }
        // https://localhost:2019/ip/testHttpClient
        public string testHttpClient()
        {
            string strResult = "";
            var client = new HttpClient();
             strResult = ( client.GetStringAsync("https://192.168.255.103:2019/Handle/getViewURL?handleCode=232323&viewType=Download&inOrOut=In")).Result;

            return strResult;
        }
        // https://localhost:2019/ip/testHttpResponseMessage
        public string testHttpResponseMessage()
        {
            string strResult = "";
            var client = new HttpClient();
            HttpResponseMessage response;
            response =  client.GetAsync("https://192.168.255.103:2019/Handle/getViewURL?handleCode=232323&viewType=Download&inOrOut=In").Result;
            HttpContent content = response.Content;
           
            strResult =  content.ReadAsStringAsync().Result;

            return strResult;
        }
        // https://192.168.255.103:2019/ip/tesDwg2Img
        public string tesDwg2Img()
        {
            string strImg = "F:/outFile" + DateTime.Now.Ticks + ".png";
            System.Drawing.Image imgObj = GetDWGImg(400,400,@"E:\jimi\work\NET CAD\测试CAD文件\焊接支架.dwg");
            using (Bitmap bitmap = new Bitmap(imgObj))
            {
                bitmap.Save(strImg);
            }
            return strImg;
        }

        struct BITMAPFILEHEADER
        {
            public short bfType;
            public int bfSize;
            public short bfReserved1;
            public short bfReserved2;
            public int bfOffBits;
        }
        public static System.Drawing.Image GetDwgImage(string FileName)
        {
            if (!(System.IO.File.Exists(FileName)))
            {
                throw new FileNotFoundException("文件没有被找到");
            }

            FileStream DwgF = null;   //文件流
            int PosSentinel;   //文件描述块的位置
            BinaryReader br = null;   //读取二进制文件
            int TypePreview;   //缩略图格式
            int PosBMP;    //缩略图位置 
            int LenBMP;    //缩略图大小
            short biBitCount; //缩略图比特深度 
            BITMAPFILEHEADER biH; //BMP文件头，DWG文件中不包含位图文件头，要自行加上去
            byte[] BMPInfo;    //包含在DWG文件中的BMP文件体
            MemoryStream BMPF = new MemoryStream(); //保存位图的内存文件流
            BinaryWriter bmpr = new BinaryWriter(BMPF); //写二进制文件类
            System.Drawing.Image myImg = null;
            try
            {

                DwgF = new FileStream(FileName, FileMode.Open, FileAccess.Read); //文件流

                br = new BinaryReader(DwgF);
                DwgF.Seek(13, SeekOrigin.Begin); //从第十三字节开始读取
                PosSentinel = br.ReadInt32();   //第13到17字节指示缩略图描述块的位置
                DwgF.Seek(PosSentinel + 30, SeekOrigin.Begin);   //将指针移到缩略图描述块的第31字节
                TypePreview = br.ReadByte();   //第31字节为缩略图格式信息，2 为BMP格式，3为WMF格式
                if (TypePreview == 1)
                {
                }
                else if (TypePreview == 2 || TypePreview == 3)
                {
                    PosBMP = br.ReadInt32(); //DWG文件保存的位图所在位置
                    LenBMP = br.ReadInt32(); //位图的大小
                    DwgF.Seek(PosBMP + 14, SeekOrigin.Begin); //移动指针到位图块
                    biBitCount = br.ReadInt16(); //读取比特深度
                    DwgF.Seek(PosBMP, SeekOrigin.Begin); //从位图块开始处读取全部位图内容备用
                    BMPInfo = br.ReadBytes(LenBMP); //不包含文件头的位图信息
                    br.Close();
                    DwgF.Close();
                    biH.bfType = 19778; //建立位图文件头
                    if (biBitCount < 9)
                    {
                        biH.bfSize = 54 + 4 * (int)(Math.Pow(2, biBitCount)) + LenBMP;
                    }
                    else
                    {
                        biH.bfSize = 54 + LenBMP;
                    }
                    biH.bfReserved1 = 0; //保留字节
                    biH.bfReserved2 = 0; //保留字节
                    biH.bfOffBits = 14 + 40 + 1024; //图像数据偏移
                    //以下开始写入位图文件头
                    bmpr.Write(biH.bfType); //文件类型
                    bmpr.Write(biH.bfSize);   //文件大小
                    bmpr.Write(biH.bfReserved1); //0
                    bmpr.Write(biH.bfReserved2); //0
                    bmpr.Write(biH.bfOffBits); //图像数据偏移
                    bmpr.Write(BMPInfo); //写入位图
                    BMPF.Seek(0, SeekOrigin.Begin); //指针移到文件开始处 
                    myImg = System.Drawing.Image.FromStream(BMPF); //创建位图文件对象                    
                    bmpr.Close();
                    BMPF.Close();
                }
                return myImg;
            }
            catch (EndOfStreamException)
            {
                throw new EndOfStreamException("文件不是标准的DWG格式文件，无法预览！");
            }
            catch (IOException ex)
            {
                if (ex.Message == "试图将文件指针移到文件开头之前。/r/n")
                {
                    throw new IOException("文件不是标准的DWG格式文件，无法预览！");
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (DwgF != null)
                {
                    DwgF.Close();
                }
                if (br != null)
                {
                    br.Close();
                }
                bmpr.Close();
                BMPF.Close();

            }
        }

        ///<summary>
        ///显示DWG文件
        ///</summary>
        ///<param name="Pwidth">要显示的宽度</param>
        ///<param name="PHeight">要显示的高度</param>
        ///<returns></returns>
        public static System.Drawing.Image GetDWGImg(int Pwidth, int PHeight, string FilePath)
        {
            System.Drawing.Image image = GetDwgImage(FilePath);
            Bitmap bitmap = new Bitmap(image);
            int Height = bitmap.Height;
            int Width = bitmap.Width;
            Bitmap newbitmap = new Bitmap(Width, Height);
            Bitmap oldbitmap = (Bitmap)bitmap;
            Color pixel;
            for (int x = 1; x < Width; x++)
            {
                for (int y = 1; y < Height; y++)
                {

                    pixel = oldbitmap.GetPixel(x, y);
                    int r = pixel.R, g = pixel.G, b = pixel.B;
                    if (pixel.Name == "ffffffff" || pixel.Name == "ff000000")
                    {
                        r = 255 - pixel.R;
                        g = 255 - pixel.G;
                        b = 255 - pixel.B;
                    }

                    newbitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            Bitmap bt = new Bitmap(newbitmap, Pwidth, PHeight);

            return bt;
        }


        // https://192.168.255.103:2019/ip/testEncrypt
        public string testEncrypt()
        {
            string TempCode = EncryptAndDecryptHelper.Locker.Encrypt("data", "123");
            return TempCode;
        }
        // https://192.168.255.103:2019/ip/testDecrypt
        public string testDecrypt(string txt = "C6eGeYUGneoQcUfy726TjqmpZKrv4Q7qommbTGQJio4=", string key = "123")
        {
            string TempCode =  EncryptAndDecryptHelper.Locker.Decrypt("C6eGeYUGneoQcUfy726TjqmpZKrv4Q7qommbTGQJio4=", "123");
            return TempCode;
        }

        // https://192.168.255.103:2019/ip/testAESEncrypt
        public string testAESEncrypt()
        {
            string TempCode = IMEAESTool.Encrypt("data测试", "123");
            TempCode += "<br/>" + testAESDecrypt();

            TempCode += "<br/>系统中：" + testEncrypt();
            return TempCode;
        }
        // https://192.168.255.103:2019/ip/testAESDecrypt
        public string testAESDecrypt()
        {
            string TempCode = IMEAESTool.Decrypt("npVIz/JZnXRq7HJyERYLRw==", "123");
            return TempCode;
        }


    }
}
