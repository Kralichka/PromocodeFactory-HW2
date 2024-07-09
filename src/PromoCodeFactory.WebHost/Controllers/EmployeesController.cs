using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }
        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeByIdAsync(EmployeeCreateModel employee)
        {
            try
            {
                if (employee == null)
                {
                    return BadRequest();
                }

                var createdEmployee = await _employeeRepository.CreateNewAsync(new Employee
                {
                    Id = new Guid(),
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    AppliedPromocodesCount = employee.AppliedPromocodesCount,
                    Roles = employee.Roles.Select(s => new Role
                    {
                        Id = new Guid(),
                        Name = s.Name,
                        Description = s.Description
                    }).ToList(),
                });


                return this.Ok(createdEmployee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ошибка при создании работника");
            }
        }
        /// <summary>
        /// Обновить данные о сотруднике по Id
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateEmployeeByIdAsync(Guid id, EmployeeUpdateModel employee)
        {
            var oldEmployee = await _employeeRepository.GetByIdAsync(id);

            if (oldEmployee == null)
                return NotFound();

            var updatedEmployee = new Employee() { 
                Id = id, 
                FirstName = employee.FirstName, 
                LastName = employee.LastName, 
                Roles = oldEmployee.Roles, 
                Email = employee.Email, 
                AppliedPromocodesCount = employee.AppliedPromocodesCount};


            var employeeModel = _employeeRepository.UpdateEmployeeAsync(updatedEmployee);

            return Ok(employeeModel);
        }

        /// <summary>
        /// Удалить данные о сотруднике по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Employee>> DeleteEmployeeByIdAsync(Guid id)
        {
            return await this._employeeRepository.DeleteByIdAsync(id) ? this.Ok(id) : this.BadRequest(id);
        }
    }
}