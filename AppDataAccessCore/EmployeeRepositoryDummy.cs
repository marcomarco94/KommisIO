using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public class EmployeeRepositoryDummy : IEmployeeRepository {
        /// <summary>
        /// Protected dictionary to dummy the authentication.
        /// </summary>
        protected Dictionary<short, (Employee employee, string password)> employees = new Dictionary<short, (Employee employee, string password)>(){
            { 0, (employee: new Employee(){ FirstName="Max", LastName="Musteradmin", PersonnelNumber=0, 
                Role=Role.Administrator }, password: "admin") },
            { 1, (employee: new Employee(){ FirstName="Max", LastName="Musteradminuser", PersonnelNumber=1, 
                Role=Role.User|Role.Administrator }, password: "adminuser") },
            { 2, (employee: new Employee(){ FirstName="Max", LastName="Musteruser", PersonnelNumber=2, 
                Role=Role.User}, password: "user") }
            };


        ///<inheritdoc/>
        public Task<bool> DeleteAsync(int id) {
            throw new InvalidOperationException();
        }

        ///<inheritdoc/>
        public Task<Employee?> GetElementByIDAsync(int id) {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Identify an User by the personnel number and password.
        /// </summary>
        /// <param name="personnelNumber">The number identifying the employee.</param>
        /// <param name="password">The password</param>
        /// <returns>Returns the employee if the personnel number exists and the password is correct, otherwise null.</returns>
        public async Task<Employee?> IdentifyAndAuthenticateAysnc(short personnelNumber, string password) {
            employees.TryGetValue(personnelNumber, out var empEntry);
            if (empEntry.password.Equals(password)) {
                return empEntry.employee;
            }
            return null;
        }

        ///<inheritdoc/>
        public Task<IEnumerable<Employee>> GetElementsAsync() {
            throw new InvalidOperationException();
        }

        ///<inheritdoc/>
        public Task<bool> InsertAsync(Employee element) {
            throw new InvalidOperationException();
        }

        ///<inheritdoc/>
        public Task<bool> UpdateAsync(Employee element) {
            throw new InvalidOperationException();
        }
    }
}
