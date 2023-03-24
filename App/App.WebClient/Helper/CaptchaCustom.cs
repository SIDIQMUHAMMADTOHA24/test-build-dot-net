using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace App.WebClient.Helper
{
    public static class CaptchaCustom
    {
        public static string GenerateCaptchaCode(int charCount)
        {
            Random r = new Random();
            string s = "";
            for (int i = 0; i < charCount; i++)
            {
                int a = r.Next(3);
                int chr;
                switch (a)
                {
                    case 0:
                        chr = r.Next(0, 9);
                        s = s + chr.ToString();
                        break;
                    case 1:
                        chr = r.Next(65, 90);
                        s = s + Convert.ToChar(chr).ToString();
                        break;
                    case 2:
                        chr = r.Next(97, 122);
                        s = s + Convert.ToChar(chr).ToString();
                        break;
                }
            }
            return s;
        }

        public static string GenerateCaptchaImage(string captchaText)
        {
            int width = 520;
            int height = 100;
            Random rnd = new Random();
            //First declare a bitmap and declare graphic from this bitmap
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            //And create a rectangle to delegete this image graphic 
            Rectangle rect = new Rectangle(0, 0, width, height);
            //And create a brush to make some drawings
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.DottedGrid, Color.Aqua, Color.White);
            g.FillRectangle(hatchBrush, rect);

            //here we make the text configurations
            GraphicsPath graphicPath = new GraphicsPath();
            //add this string to image with the rectangle delegate
            graphicPath.AddString(captchaText, FontFamily.GenericMonospace, (int)FontStyle.Bold, 90, rect, null);
            //And the brush that you will write the text
            hatchBrush = new HatchBrush(HatchStyle.Percent20, Color.Black, Color.Green);
            g.FillPath(hatchBrush, graphicPath);
            //We are adding the dots to the image
            for (int i = 0; i < (int)(rect.Width * rect.Height / 50F); i++)
            {
                int x = rnd.Next(width);
                int y = rnd.Next(height);
                int w = rnd.Next(10);
                int h = rnd.Next(10);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            //Remove all of variables from the memory to save resource
            hatchBrush.Dispose();
            g.Dispose();
            //return the image to the related component
            return ToBase64String(bitmap, ImageFormat.Png);
        }

        private static string ToBase64String(this Bitmap bmp, ImageFormat imageFormat)
        {
            string base64String = string.Empty;


            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, imageFormat);


            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();


            memoryStream.Close();


            base64String = Convert.ToBase64String(byteBuffer);
            byteBuffer = null;


            return base64String;
        }
    }
}