using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class FtpRequest
    {
        [Required]
        public string FilePath { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}