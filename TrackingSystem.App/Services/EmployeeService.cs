using TrackingSystem.App.Models;
using TrackingSystem.App.Repositories;

namespace TrackingSystem.App.Services;

public class EmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _employeeRepository.GetAllWithAssetsAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _employeeRepository.GetByIdWithAssetsAsync(id);
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        // Validate that email doesn't exist
        var existing = await _employeeRepository.GetByEmailAsync(employee.Email);
        if (existing != null)
        {
            throw new Exception($"An employee with this email already exists: {employee.Email}");
        }

        return await _employeeRepository.AddAsync(employee);
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        await _employeeRepository.UpdateAsync(employee);
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithAssetsAsync(id);
        if (employee != null && employee.AssignedAssets.Any())
        {
            throw new Exception("Cannot delete an employee with assigned assets. First unassign the assets.");
        }

        await _employeeRepository.DeleteAsync(id);
    }
}
