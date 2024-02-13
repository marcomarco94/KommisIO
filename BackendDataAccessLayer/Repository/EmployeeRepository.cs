using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class EmployeeRepository : BaseRepository<EmployeeEntity>, IEmployeeRepository {
        public EmployeeRepository(DALDbContext context) : base(context, context.Employees) {
        }

        ///<inheritdoc/>
        public async Task<EmployeeEntity?> GetEmployeeByPersonnelNumberAsync(int number) {
            return await _dbSet.Where(e => e.PersonnelNumber.Equals(number)).FirstAsync();
        }
    }
}
