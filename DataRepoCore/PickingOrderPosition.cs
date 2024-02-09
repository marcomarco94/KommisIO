using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public struct PickingOrderPosition {
        /// <summary>
        /// The id of the picking-order-position.
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// The article of the picking-position.
        /// </summary>
        public required Article Article { get; init; }

        /// <summary>
        /// The amount to be picked.
        /// </summary>
        public required int DesiredAmount { get; init; }

        /// <summary>
        /// The amount that was already picked.
        /// </summary>
        public required int PickedAmount { get; init; }
    }
}
