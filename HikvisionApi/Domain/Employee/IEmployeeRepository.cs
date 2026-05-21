namespace Domain.Employee;

public interface IEmployeeRepository
{
	/// <summary>
	/// Obtiene un empleado por número de empleado
	/// </summary>
	/// <returns>Empleado</returns>
	Task<Employee?> GetEmployeeAsync(string identificationNumber);


	/// <summary>
	/// Obtiene todos los empleados
	/// </summary>
	/// <returns>Lista de empleados</returns>
	Task<List<Employee>> GetEmployeesAsync();


	/// <summary>
	/// <param name="employee"></param>
	/// Agrega un empleado, si el número de empleado ya existe se retorna false
	/// </summary>
	/// <returns>employee number was added</returns>
	Task<string> AddEmployeeAsync(Employee employee);


	/// <summary>
	/// <param name="employee"></param>
	/// Agrega un empleado, si el número de empleado ya existe se retorna false
	/// </summary>
	/// <returns>employee number was added</returns>
	Task AddEmployeeDbAsync(Employee employee);


	/// <summary>
	/// <param name="employee"></param>
	/// Actualiza el empleado, nombre, tipo de empleado, género y fechas de validez
	/// </summary>
	/// <returns>True if the employee was updated</returns>
	Task<bool> UpdateEmployeeAsync(Employee employee);


	/// <summary>
	/// <param name="employee"></param> 
	/// Actualiza el empleado, nombre, tipo de empleado, género y fechas de validez
	/// <returns>True if the employee was updated</returns>
	/// Borrar el empleado, si el numero de empleado no existe se retorna false
	/// </summary>
	/// <returns>True if the employee was deleted</returns>
	Task<string> DeleteEmployeeAsync(Employee employee);


	/// <summary>
	/// <param name="identificationNumber"></param>
	/// Borrar el empleado, si el numero de empleado no existe se retorna false
	/// </summary>
	/// <returns>True if the employee was deleted</returns>
	Task DeleteEmployeeDbAsync(Employee employee);


	/// <summary>
	/// <param name="employee"></param>
	/// <param name="fingerIndex">Debes reemplazar "1" con el número de dedo real (1-10) si la huella ya existe sera reemplazada</param>	
	/// </summary>
	/// <returns>EmployeeFingerPrint</returns>
	Task<EmployeeFingerPrint> AddFingerPrintAsync(Employee employee, int fingerIndex = 1);


	/// <summary>
	/// <param name="employee"></param>
	/// <param name="fingerIndex">Debes reemplazar "1" con el número de dedo real (1-10) si la huella ya existe sera reemplazada</param>	
	/// </summary>
	/// <returns>True if the employee was deleted</returns>
	/// <exception cref="Exception"></exception>
	Task<bool> DeleteFingerPrintAsync(Employee employee, int fingerIndex = 1);
}
