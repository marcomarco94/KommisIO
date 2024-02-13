using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class StockPositionRepository : BaseRepository<StockPositionEntity> {
        public StockPositionRepository(DALDbContext context) : base(context, context.StockPositions) {
        }
    }
}
