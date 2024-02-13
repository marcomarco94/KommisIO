using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class PickingOrderRepository : BaseRepository<PickingOrderEntity> {
        public PickingOrderRepository(DALDbContext context) : base(context, context.PickingOrders) {
        }
    }
}
