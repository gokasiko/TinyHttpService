﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.ActionResults.Interface;
using TinyHttpService.HttpData;

namespace TinyHttpService.ActionResults
{
    public class DownloadResult : ActionResult
    {
        public string FilePath { get; set; }

        public DownloadResult(string path)
        {
            this.FilePath = path;
        }

        public override void Execute(HttpContext context)
        {
            var fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath.TrimStart('/', '\\'));
            if (File.Exists(fullpath))
            {
                var response = context.Response;
                var filename = Path.GetFileName(fullpath);
                var ext = Path.GetExtension(fullpath);

                response.StatusCode = 200;
                response.ContentType = Mime.Get(ext.ToLower()) ?? "application/octet-stream";
                response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename));

                var buffer = new byte[4096];
                int readBytes = 0;
                using (var fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    response.AddHeader("Content-Length", fs.Length.ToString());
                    while ((readBytes = fs.Read(buffer, 0, 4096)) > 0)
                    {
                        response.Write(buffer);
                    }
                }

                response.End();
            }
            else
            {
                (new Http404NotFoundResult()).Execute(context);
            }
        }
    }
}
