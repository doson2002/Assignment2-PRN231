using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.DTO
{
    public class AccountDTO
    {
        [Key]
        public int AccountId { get; set; }

        public string AccountPassword { get; set; } = null!;

        public string? EmailAddress { get; set; }

        public string AccountNote { get; set; } = null!;

        public int? Role { get; set; }
        public string AccessToken { get; set; }

    }
}
