using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace EmployeeCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : Controller
    {
        readonly IEmployeeService _Employee;
        public EmployeeController(IEmployeeService Employee)
        {
            _Employee = Employee;
        }

        #region CRUD
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            try
            {
                return await _Employee.GetEmployeesAsync();
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var Employee = await _Employee.GetByIdAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }
            return Employee;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee Employee)
        {
            if (id != Employee.Id)
            {
                return BadRequest();
            }
            try
            {
                await _Employee.Update(Employee);
            }
            catch (Exception ex)
            {
                if (!_Employee.EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee Employee)
        {
            var employee = await _Employee.Insert(Employee);
            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var Employee = await _Employee.GetByIdAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }
           await _Employee.Delete(Employee);

            return NoContent();
        }
        #endregion
      
        #region Login


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(Employee Employee)
        {
            return Ok(await Login(Employee));
        }
        #endregion
    }
}
