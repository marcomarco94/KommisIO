using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public interface IEmployeeRepository : IRepository<Employee> {

        /// <summary>
        /// Identify an User by the personnel number and password.
        /// </summary>
        /// <param name="personnelNumber">The number identifying the employee.</param>
        /// <param name="password">The password</param>
        /// <returns>Returns the employee if the personnel number exists and the password is correct, otherwise null.</returns>
        public Task<Employee?> IdentifyAndAuthenticateAysnc(short personnelNumber, string password);
    }
}
