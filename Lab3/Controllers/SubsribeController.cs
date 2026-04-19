using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Lab3.Controllers
{
    [Route("api/subscribe")]
    [ApiController]
    public class SubsribeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Subscribe(EmailRequest request, IMailService mailService)
        {
            return mailService.SendEmail(request.Email, "Welcome to the Swamp!", "Stay Froggy!\nThis is test message.")
                ? Ok() : StatusCode(500);
        }
    }

    public class EmailRequest
    {
        [Required][EmailAddress] public string Email { get; set; } = "";
    }
}
