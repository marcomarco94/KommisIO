using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public struct DamageReport {
        /// <summary>
        /// The employee who has filed the damage report.
        /// </summary>
        public required Employee Employee { get; init; }

        /// <summary>
        /// The article damaged.
        /// </summary>
        public required Article Article { get; init; }

        /// <summary>
        /// The message of the employee describing the damage.
        /// </summary>
        public required string Message { get; init; }
    }
}
