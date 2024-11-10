using BOs;
using BOs.DTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class AccountDAO
    {
        private readonly Fall24CosmeticsDbContext _context;
        private static AccountDAO instance = null;
        private AccountDAO()
        {
            _context = new Fall24CosmeticsDbContext();
        }
        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    return new AccountDAO();
                }
                return instance;
            }
        }

        public AccountDTO login(String email, String password, JwtAuth jwtAuth)
        {

            try
            {
                SystemAccount existedAccount = _context.SystemAccounts.SingleOrDefault(x => x.EmailAddress.Equals(email) && x.AccountPassword.Equals(password));
                if (existedAccount == null)
                {
                    throw new Exception("Email or Password is invalid");
                }
                AccountDTO account = new AccountDTO();
                account.AccountId = existedAccount.AccountId;
                account.AccountPassword = password;
                account.EmailAddress = email;
                account.AccountNote = existedAccount.AccountNote;
                account.Role = existedAccount.Role;
            
                //Generate token
                return GenerateToken(account, jwtAuth);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private AccountDTO GenerateToken(AccountDTO getAccount, JwtAuth jwtAuth)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuth.Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new ClaimsIdentity(new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, getAccount.AccountId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, getAccount.EmailAddress),
            new Claim(JwtRegisteredClaimNames.Name, getAccount.AccountNote),
            new Claim(ClaimTypes.Role, getAccount.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(120),
                    SigningCredentials = credentials
                };

                var token = jwtTokenHandler.CreateToken(tokenDescription);
                getAccount.AccessToken = jwtTokenHandler.WriteToken(token);

                return getAccount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log it as needed
                throw new ApplicationException("Error generating JWT token", ex);
            }
        }
    }
}
    