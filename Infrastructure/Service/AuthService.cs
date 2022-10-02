using Core.Interfaces;
using Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.IdentityModel.Tokens;
using Core.ViewModels;

namespace Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly JWT _jwt;
        readonly IEmployeeService _Employee;

        public AuthService(IOptions<JWT> jwt, IEmployeeService Employee)
        {
            _Employee = Employee;
            _jwt = jwt.Value;
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {

            var Emp = await _Employee.FindByEmailAsync(model.Email);
            var authModel = new AuthModel();
            if (Emp != null)
            {
                var jwtSecurityToken = await CreateJwtToken(Emp);
                // var rolesList = await _userManager.GetRolesAsync(Emp);

                authModel.IsAuthenticated = true;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.Email = Emp.Email;
                authModel.Username = Emp.Name;
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;
                // authModel.Roles = rolesList.ToList();

                return authModel;
            }

            //if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            //{
            //    authModel.Message = "Email or Password is incorrect!";
            //    return authModel;
            //}

            authModel.IsAuthenticated = false;
            return authModel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(Employee Employee)
        {
            //var userClaims = await _userManager.GetClaimsAsync(user);
            //var roles = await _userManager.GetRolesAsync(user);
            //var roleClaims = new List<Claim>();

            //foreach (var role in roles)
            //    roleClaims.Add(new Claim("roles", role));

            //var claims = new[]
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //    new Claim(JwtRegisteredClaimNames.Email, user.Email),
            //    new Claim("uid", user.Id)
            //}
            //.Union(userClaims)
            //.Union(roleClaims);
            List<Claim> claims = new List<Claim>
            {
                new Claim("Name", Employee.Name),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
