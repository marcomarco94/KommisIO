using BackendDataAccessLayer.Entity;
using DataRepoCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class EmployeeRepository : BaseRepository<EmployeeEntity>, IEmployeeRepository {
        protected IPasswordHasher<EmployeeEntity> _hasher;

        public EmployeeRepository(DALDbContext context, IPasswordHasher<EmployeeEntity> hasher) : base(context, context.Employees) {
            _hasher = hasher;
        }

        ///<inheritdoc/>
        public async Task<EmployeeEntity?> GetEmployeeByPersonnelNumberAsync(int number) {
            var emp = await _dbSet.Where(e => e.PersonnelNumber == number).FirstOrDefaultAsync();
            return emp;
        }
    }
}
