using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Service
{
    public class EmployeeService : IEmployeeService
    {
        #region Fields
        private readonly EmployeeDataBaseContext _context;
        private readonly IConfiguration _configuration;
        //private readonly HttpContext _httpContext;
        private DbSet<Employee> table = null;
        private string _token = "";
        #endregion

        #region constructor 
        public EmployeeService(EmployeeDataBaseContext applicationDbContext, IConfiguration configuration/*, HttpContext HttpContext*/)
        {
            _context = applicationDbContext;
            _configuration = configuration;
            table = _context.Set<Employee>();
            // _httpContext = HttpContext;

        }

        #endregion

        #region CRUD Fun
        public bool EmployeeExists(int id)
        {
            return table.Any(e => e.Id == id);
        }
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await table.ToListAsync();
        }
        public async Task<Employee> GetByIdAsync(int id)
        {
            return await table.FindAsync(id);
        }
        public async Task<Employee> Insert(Employee entity)
        {
            try
            {
                CreatePasswordHash(entity);
                table.Add(entity);
                await _context.SaveChangesAsync();
                //transaction.Commit();
                return entity;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                throw ex;
            }
        }
        public async Task<Employee> Update(Employee entity)
        {
            CreatePasswordHash(entity);
            try
            {
                table.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                throw ex;
            }
        }
        public async Task Delete(Employee entity)
        {
            try
            {
                table.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                throw ex;
            }
        }
        #endregion
        //Auth
        #region Auth
        public Employee CreatePasswordHash(Employee Employee)
        {
            if (EmployeeExists(Employee.Id))
            {
                return Employee;
            }
            using (var hmac = new HMACSHA512())
            {
                Employee.PasswordSalt = hmac.Key;
                Employee.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Employee.Password));
            }
            return Employee;
        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        public string CreateToken(Employee Employee)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Employee.Name),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            // Employee.TokenCreated = DateTime.Now;
            //Employee.TokenExpires = DateTime.Now.AddDays(1);
            return jwt;

        }
        //Login
        public async Task<string> Login(Employee Employee)
        {
            var emp = await GetByIdAsync(Employee.Id);
            if (emp.Name != Employee.Name)
            {
                return ("User not found.");
            }

            if (!VerifyPasswordHash(Employee.Password, emp.PasswordHash, emp.PasswordSalt))
            {
                return "Wrong password.";
            }

            // _token = CreateToken(emp);

            //// RefreshToken
            // SetRefreshToken(emp);
            return ("");
        }
        public void SetRefreshToken(Employee Employee)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(7),
            };
            CreateToken(Employee);
            _token = Convert.ToString(RandomNumberGenerator.GetInt32(26));
            //  _httpContext.Response.Cookies.Append("refreshToken", _token, cookieOptions);
            Employee.TokenCreated = DateTime.Now;
            Employee.TokenExpires = DateTime.Now.AddDays(7);
            var Emp = Update(Employee);
        }
        #endregion
        public async Task<Employee> FindByEmailAsync(string email)
        {
            return await table.Where(e => e.Email == email).FirstOrDefaultAsync();
        }
    }
}
