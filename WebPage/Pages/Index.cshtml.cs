using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace WebPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage = string.Empty;

        public string Token = string.Empty;

        public IndexModel(IHttpContextAccessor httpContextAccessor)
        {

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7256/");
            _httpContextAccessor = httpContextAccessor;

        }

        public void OnGet()
        {
            // Code to handle GET request
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["Message"] = "Email or password can not be empty";
                return Page();
            }

            var loginModel = new
            {
                email = Email,
                password = Password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/account/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonDocument.Parse(responseContent);
                    Token = jsonResponse.RootElement.GetProperty("accessToken").GetString();

                    var handler = new JwtSecurityTokenHandler();

                    if (handler.ReadToken(Token) is not JwtSecurityToken jsonToken)
                        return RedirectToPage("CosmeticPage/Index");
                    var accountEmail = jsonToken.Claims.First(claim => claim.Type == "email").Value;
                    var accountId = jsonToken.Claims.First(claim => claim.Type == "sub").Value;
                    var role = jsonToken.Claims.First(claim => claim.Type == "role").Value;

                    if (role == "1" || role == "3" || role =="4")
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("accessToken", Token);

                        AddRoleClaim(role, accountId, accountEmail);

                        return RedirectToPage("CosmeticPage/Index");
                    }
                    else
                    {
                        // Set success message in TempData
                        TempData["Message"] = "You are not allowed to access this function!";
                        return Page();
                    }

                }
                else
                {
                    // Set success message in TempData
                    TempData["Message"] = "Wrong Email or Password";
                }

                ErrorMessage = "Case login unsuccessfully, display: You are not allowed to access this function!";
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = $"Request error: {e.Message}";
            }

            return Page();
        }

        private void AddRoleClaim(string role, string userId, string email)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role),
            new(ClaimTypes.NameIdentifier, userId),
            new(JwtRegisteredClaimNames.Email, email)
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}