using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public record PickingOrder {
        /// <summary>
        /// The id of the picking order.
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// The priority of the order.
        /// </summary>
        public required int Priority { get; init; }

        /// <summary>
        /// A note for the picking-order, special instructions, etc?
        /// </summary>
        public required string Note { get; init; }

        /// <summary>
        /// The assigned employee.
        /// </summary>
        public Employee? Assignee { get; init; }

        /// <summary>
        /// The positions of the picking-order.
        /// </summary>
        public required IEnumerable<PickingOrderPosition> OrderPositions { get; init; }
    }
}
