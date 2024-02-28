using DataRepoCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Entity {
    public class StockPositionEntity {
        /// <summary>
        /// The tech. id of the stock position
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The amount of items stored.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Amount { get; set; }

        /// <summary>
        /// The shelf number wher this position is stored.
        /// </summary>
        public int ShelfNumber { get; set; }

        /// <summary>
        /// The article in the stock position.
        /// </summary>
        public ArticleEntity? Article { get; set; }

        /// <summary>
        /// The version for Optimistic Concurrency as inspried by: 
        /// https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations
        /// </summary>
        [Timestamp]
        public byte[] Version { get; set; }

        public StockPositionEntity(int id, int amount, int shelfNumber) { 
            Id = id;
            Amount = amount;
            ShelfNumber = shelfNumber;
        }

        public StockPositionEntity(StockPosition stockPosition) { 
            Id = stockPosition.Id;
            Amount = stockPosition.Amount;
            ShelfNumber = stockPosition.ShelfNumber;
        }

        public StockPosition MapToDataModel() {
            return new StockPosition() { Amount = Amount, Article = Article!.MapToDataModel(), Id = Id, ShelfNumber = ShelfNumber };
        }
    }
}
