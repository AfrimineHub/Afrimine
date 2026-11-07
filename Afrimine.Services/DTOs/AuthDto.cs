using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class AuthDto
    {
        public class ProfilePhotoUploadDto
        {
            [Required]
            public IFormFile Photo { get; set; } = default!;
        }
    }
}
