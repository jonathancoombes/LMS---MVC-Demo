using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LMS.Models;
using LMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace LMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        private readonly RoleManager<IdentityRole> _roleManager;


        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }
            [Required]
            public string Surname { get; set; }


            [Required]
            [MaxLength(13, ErrorMessage = "Your identity number must be 13 digits long.")]
            [MinLength(13, ErrorMessage = "Your identity number must be 13 digits long.")]
            [Display(Name = "ID Number")]
            public string IdentityNumber { get; set; }

            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Display(Name = "Contact Number")]
            [MinLength(10, ErrorMessage = "Your contact number must be at least 10 characters long.")]
            [MaxLength(12, ErrorMessage = "Your contact number must be no more than 12 characters long.")]
            public string ContactNumber { get; set; }

        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            //Getting the value of the selected role in the Employee registration option

            string role = Request.Form["rdUserRole"].ToString();


            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {

                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    Surname = Input.Surname,
                    IdentityNumber = Input.IdentityNumber,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    Province = Input.Province,
                    PostalCode = Input.PostalCode,
                    ContactNumber = Input.ContactNumber

                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    // Creating the roles for the first time if it doesnt exists
                    if (!await _roleManager.RoleExistsAsync(SD.Administrator))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Administrator));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Designer))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Designer));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Facilitator))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Facilitator));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Assessor))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Assessor));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Learner))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Learner));
                    }

                    // !!Adding Administrator role to user to enable top level user!!
                    // !! ONLY DONE 1ST TIME AROUND !!
                    // await _userManager.AddToRoleAsync(user, SD.Administrator);


                    switch (role)
                    {
                        case SD.Administrator:
                            await _userManager.AddToRoleAsync(user, SD.Administrator);
                            break;
                        case SD.Designer:
                            await _userManager.AddToRoleAsync(user, SD.Designer);
                            break;
                        case SD.Facilitator:
                            await _userManager.AddToRoleAsync(user, SD.Facilitator);
                            break;
                        case SD.Assessor:
                            await _userManager.AddToRoleAsync(user, SD.Assessor);
                            break;
                        default:
                            await _userManager.AddToRoleAsync(user, SD.Learner);

                            //Signs in user after registration if the role selected is a learner & not created by Administator. 
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                    }


                    _logger.LogInformation("User created a new account with password.");


                    return RedirectToAction("Index", "Home", new { area = "Learner" });

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
