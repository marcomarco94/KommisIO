using BackendDataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public interface IEmployeeRepository : IRepository<EmployeeEntity> {
        /// <summary>
        /// Get the employee by his personnel number.
        /// </summary>
        /// <param name="number">The personnel number of the employee.</param>
        /// <returns>Returns the employee with the given Personnel Number, null if not found.</returns>
        public Task<EmployeeEntity?> GetEmployeeByPersonnelNumberAsync(int number);
    }
}
