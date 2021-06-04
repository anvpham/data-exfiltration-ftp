using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class FtpRequest
    {
        [Required]
        public string FilePath { get; set; }
    }
}