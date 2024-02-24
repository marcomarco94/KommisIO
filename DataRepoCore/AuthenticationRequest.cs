using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public record AuthenticationRequestInformation {
        /// <summary>
        /// The Personnel number to identify the employee
        /// </summary>
        public short PersonnelNumber { get; set; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; set; }
    }
}
