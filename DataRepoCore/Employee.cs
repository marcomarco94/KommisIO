using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public record Employee {
        /// <summary>
        /// The tech. id of the employee.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The first name of the employee
        /// </summary>
        public required string FirstName { get; init; }

        /// <summary>
        /// The last name of the employee.
        /// </summary>
        public required string LastName { get; init; }

        /// <summary>
        /// A individual number to identify the employee (unique Key), it is Unique but not the tech. primary key. (Seperation of tech. and buis. logic)
        /// </summary>
        public required short PersonnelNumber { get; init; }

        /// <summary>
        /// The assigned roles, an enum because the product is intended as demo, so no further customization is needed. 
        /// This strips away the not necessary complexity of the authentication system and database. 
        /// </summary>
        public required Role Role { get; init; }
    }
}
