using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmployeeService
    {
        #region CRUD
        Task<List<Employee>> GetEmployeesAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> Insert(Employee Employee);
        Task<Employee> Update(Employee Employee);
        Task Delete(Employee Employee);
        bool EmployeeExists(int id);
        Task<Employee> FindByEmailAsync(string email);
        #endregion
        //Auth
        #region Auth
        Employee CreatePasswordHash(Employee Employee);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        public string CreateToken(Employee Employee);
        public void SetRefreshToken(Employee Employee);
        #endregion
    }
}
