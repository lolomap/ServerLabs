using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Lab3.Pages
{
    public class ContactModel(ICsvService<ContactForm> csv, ILogger<ContactModel> logger) : PageModel
    {
        [BindProperty] public required ContactForm ContactForm { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await csv.SaveAsync(ContactForm);
                TempData["SuccessMessage"] = true;
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to save contact form message: {message}", ex.Message);
                ModelState.AddModelError(string.Empty, "Failed to save contact form message");
                return Page();
            }

            return RedirectToPage();
        }
    }

    public class ContactForm : ICsvSerializable
    {
        [Required] public required string Name { get; set; }
        [Required] [EmailAddress] [EmailDomain("edu")] public required string Email { get; set; }
        [Required] public required string Subject { get; set; }
        [Required] public required string Message { get; set; }
    }

    public class EmailDomainAttribute(string domain) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString() ?? "";
                if (email.EndsWith($".{domain}", StringComparison.OrdinalIgnoreCase))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? $"Email must end with .{domain}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
