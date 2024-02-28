using DataRepoCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Entity {
    public class PickingOrderPositionEntity {

        /// <summary>
        /// The tech. id of the picking order position.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The amount of the article that is to be picked.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int DesiredAmount { get; set; }

        /// <summary>
        /// The amount that was already picked.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int PickedAmount { get; set; }

        /// <summary>
        /// The article that should be picked.
        /// </summary>
        public ArticleEntity? Article { get; set; }

        /// <summary>
        /// The version for Optimistic Concurrency as inspried by: 
        /// https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations
        /// </summary>
        [Timestamp]
        public byte[] Version { get; set; }

        public PickingOrderPositionEntity(int id, int desiredAmount, int pickedAmount) {
            Id = id;
            DesiredAmount = desiredAmount;
            PickedAmount = pickedAmount;
        }

        public PickingOrderPositionEntity(PickingOrderPosition position) {
            Id = position.Id;
            DesiredAmount = position.DesiredAmount;
            PickedAmount = position.PickedAmount;
        }

        public PickingOrderPosition MapToDataModel() {
            return new PickingOrderPosition() { Article = Article.MapToDataModel(), DesiredAmount = DesiredAmount, Id = Id, PickedAmount = PickedAmount };
        }
    }
}
