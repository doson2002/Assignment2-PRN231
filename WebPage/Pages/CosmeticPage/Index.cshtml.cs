using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BOs;
using DAOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebPage.Pages.CosmeticPage
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient httpClient;

        public IndexModel()
        {
            httpClient = new HttpClient();
        }

        public IList<CosmeticInformation> CosmeticInformation { get; set; } = default!;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public bool HasNextPage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Query { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            PageNumber = pageNumber;
            int skip = (PageNumber - 1) * PageSize;

            // Lấy Bearer Token từ session
            var token = HttpContext.Session.GetString("accessToken");

            // Xây dựng URL với OData $top, $skip và $filter
            var url = $"https://localhost:7256/cosmetic/get_all?$top={PageSize}&$skip={skip}";

            // Nếu có từ khóa tìm kiếm, thêm bộ lọc $filter
            if (!string.IsNullOrEmpty(Query))
            {
                url += $"&$filter=contains(tolower(CosmeticName),tolower('{Query}')) or contains(tolower(SkinType),tolower('{Query}')) or contains(tolower(CosmeticSize),tolower('{Query}'))&$orderby=CosmeticId desc";
            }

            // Tạo một yêu cầu HTTP với Bearer Token
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                if (!string.IsNullOrEmpty(token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = await response.Content.ReadAsStringAsync();
                    CosmeticInformation = JsonSerializer.Deserialize<List<CosmeticInformation>>(data, options);

                    // Xác định xem có trang tiếp theo không
                    HasNextPage = CosmeticInformation.Count == PageSize;
                }
                else
                {
                    // Xử lý lỗi nếu cần
                }
            }
        }

    }
}
