using MimeDetective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.WebClient.Models
{
    public class CheckHeaderFile
    {
        public static bool ProcessImage(string base64_file)
        {
            try
            {
                var Inspector = new ContentInspectorBuilder()
                {
                    Definitions = MimeDetective.Definitions.Default.All()
                }.Build();
                var Results = Inspector.Inspect(Convert.FromBase64String(base64_file));
                var r = Results.ByMimeType().SingleOrDefault();

                string[] list_type_image = { "image/jpeg", "image/png" };
                var a = list_type_image.Where(o => o == r.MimeType);
                if (a.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ProcessPdf(string base64_file)
        {
            try
            {
                var Inspector = new ContentInspectorBuilder()
                {
                    Definitions = MimeDetective.Definitions.Default.All()
                }.Build();
                var Results = Inspector.Inspect(Convert.FromBase64String(base64_file));
                var r = Results.ByMimeType().SingleOrDefault();

                string[] list_type_image = { "application/pdf" };
                var a = list_type_image.Where(o => o == r.MimeType);
                if (a.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ProcessZip(string base64_file)
        {
            try
            {
                var Inspector = new ContentInspectorBuilder()
                {
                    Definitions = MimeDetective.Definitions.Default.All()
                }.Build();
                var Results = Inspector.Inspect(Convert.FromBase64String(base64_file));
                var r = Results.ByMimeType().SingleOrDefault();

                string[] list_type_image = { "application/zip" };
                var a = list_type_image.Where(o => o == r.MimeType);
                if (a.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}