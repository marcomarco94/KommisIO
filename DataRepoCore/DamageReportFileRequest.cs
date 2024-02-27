using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public record DamageReportFileRequest {
        /// <summary>
        /// The article damaged.
        /// </summary>
        public required int ArticleNumber { get; init; }

        /// <summary>
        /// The message of the employee describing the damage.
        /// </summary>
        public required string Message { get; init; }
    }
}
