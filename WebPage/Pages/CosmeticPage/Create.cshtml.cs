using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BOs;
using DAOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace WebPage.Pages.CosmeticPage
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateModel(IHttpContextAccessor httpContextAccessor)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7256/");
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public List<CosmeticCategory> CategoryList { get; set; } = default!;
        public async Task<IActionResult> OnGet()
        {
            // Lấy token từ session
            var token = _httpContextAccessor.HttpContext.Session.GetString("accessToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Index");
            }

            // Kiểm tra role trong token
            var handler = new JwtSecurityTokenHandler();
            if (handler.ReadToken(token) is JwtSecurityToken jwtToken)
            {
                // Lấy giá trị của claim "role"
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                // Kiểm tra nếu role = 1 hoặc role = 2
                if (role == "1")
                {
                    // Tạo yêu cầu HTTP với Bearer Token
                    using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "category/get_all"))
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        // Gọi API để lấy danh sách category
                        var categoryResponse = await httpClient.SendAsync(requestMessage);

                        if (categoryResponse.IsSuccessStatusCode)
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };
                            var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                            CategoryList = JsonSerializer.Deserialize<List<CosmeticCategory>>(categoryData, options);

                            // Chuyển đổi danh sách category thành SelectList
                            ViewData["CategoryId"] = new SelectList(CategoryList, "CategoryId", "CategoryName");
                        }
                        else
                        {
                            ViewData["CategoryId"] = new SelectList(new List<CosmeticCategory>(), "CategoryId", "CategoryName");
                        }
                    }
                }
                else
                {
                    return RedirectToPage("/Error");
                }
            }
            else
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }


        [BindProperty]
        public CosmeticInformation CosmeticInformation { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGet();
                return Page();
            }

            // Serialize Cosmetic object to JSON
            var content = new StringContent(JsonSerializer.Serialize(CosmeticInformation), Encoding.UTF8, "application/json");

            // Lấy Bearer Token từ session
            var token = _httpContextAccessor.HttpContext.Session.GetString("accessToken");

            // Tạo yêu cầu HTTP với Bearer Token
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "cosmetic/create"))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                requestMessage.Content = content;

                // Call API to create Cosmetic
                var response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    // Set success message in TempData
                    TempData["SuccessMessage"] = "Create successful!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    // Đọc nội dung phản hồi lỗi từ API
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Phân tích cú pháp JSON để lấy message
                    using (JsonDocument doc = JsonDocument.Parse(responseContent))
                    {
                        if (doc.RootElement.TryGetProperty("message", out JsonElement messageElement))
                        {
                            var errorMessage = messageElement.GetString();
                            ModelState.AddModelError(string.Empty, errorMessage);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Unable to create Cosmetic.");
                        }
                    }

                    await OnGet();
                    return Page();
                }
            }
        }

    }
}
