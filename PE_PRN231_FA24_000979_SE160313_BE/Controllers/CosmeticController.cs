using BOs;
using BOs.DTO;
using DAOs.Authentications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Options;
using REPOs;

namespace PE_PRN231_FA24_000979_SE160313_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmeticController : ControllerBase
    {
        private readonly ICosmeticRepo cosmeticRepo;
        private readonly IAccountRepo accountRepo;
        private readonly ICategoryRepo categoryRepo;
        private IOptions<JwtAuth> jwtAuthOptions;

        public CosmeticController(ICosmeticRepo cosmeticRepo, IAccountRepo accountRepo, ICategoryRepo categoryRepo, IOptions<JwtAuth> jwtAuthOptions)
        {
            this.cosmeticRepo = cosmeticRepo;
            this.accountRepo = accountRepo;
            this.categoryRepo = categoryRepo;
            this.jwtAuthOptions = jwtAuthOptions;
        }

        [HttpGet("/category/get_all")]
        [EnableQuery]
        [PermissionAuthorize(1, 3, 4)]
        public IActionResult GetAllCategory()
        {
            return Ok(categoryRepo.GetCategories());
        }

        [HttpGet("/category/get_category_by_id")]
        [EnableQuery]
        [PermissionAuthorize(1, 3, 4)]
        public IActionResult GetCategoryById([FromODataUri] string id)
        {
            var entity = categoryRepo.GetCategory(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPost("/account/login")]
        [EnableQuery]
        public IActionResult Post([FromBody] AccountLoginDTO accountLoginDTO)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                AccountDTO getAccount = accountRepo.GetAccount(accountLoginDTO.Email, accountLoginDTO.Password, jwtAuthOptions.Value);
                return Ok(getAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                });

            }
        }

        [HttpPost("/cosmetic/create")]
        [EnableQuery]
        [PermissionAuthorize(1)]
        public IActionResult CreateSilver([FromBody] CosmeticInformation cosmetic)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Model is not valid",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                bool isAdded = this.cosmeticRepo.AddCosmetic(cosmetic);

                if (isAdded)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Cosmetic added successfully",
                        Data = cosmetic
                    });
                }
                else
                {
                    return Conflict(new
                    {
                        Success = false,
                        Message = "Cosmetic with this ID already exists."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while adding the jewelry.",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("/cosmetic/update")]
        [EnableQuery]
        [PermissionAuthorize(1)]
        public IActionResult UpdateCosmetic([FromBody] CosmeticInformation cosmetic)
        {
            try
            {
                // Kiểm tra Model có hợp lệ không
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                // Gọi hàm cập nhật
                var result = cosmeticRepo.UpdateCosmetic(cosmetic);

                // Kiểm tra kết quả cập nhật
                if (result)
                {
                    return Ok(new { Message = "Cosmetic updated successfully." });
                }
                else
                {
                    return NotFound(new { Message = "Cosmetic not found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpDelete("/cosmetic/delete")]
        [EnableQuery]
        [PermissionAuthorize(1)]
        public IActionResult deleteCosmetic([FromODataUri] string id)
        {
            try
            {
                var entity = cosmeticRepo.GetCosmetic(id);
                if (entity == null)
                {
                    return NotFound();
                }

                // Gọi hàm cập nhật
                var result = cosmeticRepo.RemoveCosmetic(id);

                // Kiểm tra kết quả cập nhật
                if (result)
                {
                    return Ok(new { Message = "Cosmetic deleted successfully." });
                }
                else
                {
                    return NotFound(new { Message = "Cosmetic not found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("/cosmetic/get_all")]
        [EnableQuery]
        [PermissionAuthorize(1, 3,4)]
        public IActionResult GetAllCosmetic()
        {
            return Ok(cosmeticRepo.GetCosmetics());
        }

        [HttpGet("/cosmetic/get_cosmetic_by_id")]
        [EnableQuery]
        [PermissionAuthorize(1, 3, 4)]
        public IActionResult GetById([FromODataUri] string id)
        {
            var entity = cosmeticRepo.GetCosmetic(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }
    }
}
