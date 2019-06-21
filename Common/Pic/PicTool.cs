using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMEWebCAD.Common.Pic
{
    public static class PicTool
    {
        /// <summary>
        /// 根据指定尺寸得到按比例缩放的尺寸,返回true表示以更改尺寸
        /// </summary>
        /// <param name="picWidth">图片宽度</param>
        /// <param name="picHeight">图片高度</param>
        /// <param name="specifiedWidth">指定宽度</param>
        /// /// <param name="specifiedHeight">指定高度</param>
        /// <returns>返回true表示以更改尺寸</returns>
        public static bool GetPicZoomSize(ref int picWidth, ref int picHeight, int specifiedWidth, int specifiedHeight)
        {
            int sW = 0, sH = 0;
            Boolean isZoomSize = false;
            //按比例缩放
            Size tem_size = new Size(picWidth, picHeight);
            if (tem_size.Width > specifiedWidth || tem_size.Height > specifiedHeight) //将**改成c#中的或者操作符号
            {
                if ((tem_size.Width * specifiedHeight) > (tem_size.Height * specifiedWidth))
                {
                    sW = specifiedWidth;
                    sH = (specifiedWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = specifiedHeight;
                    sW = (tem_size.Width * specifiedHeight) / tem_size.Height;
                }
                isZoomSize = true;
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }
            picHeight = sH;
            picWidth = sW;
            return isZoomSize;
        }
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="dHeight">高度</param>
        /// <param name="dWidth">宽度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <returns></returns>

        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int sW = iSource.Width, sH = iSource.Height;

            GetPicZoomSize(ref sW, ref sH, dWidth, dHeight);

            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();

                ImageCodecInfo jpegICIinfo = null;

                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();

            }
        }
    }
}
