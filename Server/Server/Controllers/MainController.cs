using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Server.DTOs;

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
            ftpRequest.Credentials = new NetworkCredential(body.UserName, body.Password);
            ftpRequest.ContentLength = fileContents.Length;

            using (var requestStream = ftpRequest.GetRequestStream())
            {
                await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
            }

            return StatusCode(201);
        }
    }
}
