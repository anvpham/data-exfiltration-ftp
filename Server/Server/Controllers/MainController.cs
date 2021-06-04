using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Server.DTOs;
using System.Collections.Generic;

namespace Server.Controllers
{
    [ApiController, Route("api")]
    public class MainController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public MainController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("uploadfile"), HttpPost]
        public async Task<IActionResult> ExecuteFTPRequest([FromBody] FtpRequest body)
        {
            if (!System.IO.File.Exists(@$"f:\{body.FilePath}"))
                return StatusCode(404);
            
            byte[] fileContents;
            using (var fileStreamReader = new StreamReader(@$"f:\{body.FilePath}"))
            {
                fileContents = Encoding.UTF8.GetBytes(await fileStreamReader.ReadToEndAsync());
            }

            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create($"ftp://172.22.214.141/{body.FilePath.Split("\\")[^1]}");
            
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.Credentials = new NetworkCredential(_configuration["ftpUserName"], _configuration["ftpPassword"]);
            ftpRequest.ContentLength = fileContents.Length;

            using (var requestStream = ftpRequest.GetRequestStream())
            {
                await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
            }

            return StatusCode(201);
        }

        [Route("listdirectory"), HttpPost]
        public IActionResult ListDirectory([FromBody] ListDirectory body)
        {
            body.Path ??= "";
        
            if (!Directory.Exists($@"f:\{body.Path}"))
                return StatusCode(404);

            IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(@$"f:\{body.Path}");
            var stringBuilder = new StringBuilder();

            foreach (var entry in entries)
            {
                stringBuilder.AppendLine(entry);
            }

            return StatusCode(200, new {result = stringBuilder.ToString()});
        }
    }
}
