using BOs.DTO;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPOs
{
    public class AccountRepo : IAccountRepo
    {
        public AccountDTO GetAccount(String email, String password, JwtAuth jwtAuth) => AccountDAO.Instance.login(email, password, jwtAuth);
    }
}
