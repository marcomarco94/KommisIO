using DataRepoCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Entity {
    public class DamageReportEntity {
        /// <summary>
        /// The tech. id of the damage report.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// A note of the employee that discovered the damage.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The employee who filed the report.
        /// </summary>
        public EmployeeEntity? Employee { get; set; }

        /// <summary>
        /// The damaged article.
        /// </summary>
        public ArticleEntity? Article { get; set; }

        public DamageReportEntity(int id, string note) { 
            Id = id;
            Note = note;
        }

        public DamageReportEntity(DamageReport report) {
            Id = report.Id;
            Note = report.Message;
        }

        public DamageReport MapToDataModel() {
            return new DamageReport() { Article = Article!.MapToDataModel(), Employee = Employee!.MapToDataModel(), Id = Id, Message = Note };
        }
    }
}
