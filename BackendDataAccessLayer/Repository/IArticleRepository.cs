using BackendDataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public interface IArticleRepository : IRepository<ArticleEntity> {
        /// <summary>
        /// Get the article by its article number.
        /// </summary>
        /// <param name="number">The article number of the employee.</param>
        /// <returns>Returns the artcile with the given Article Number, null if not found.</returns>
        public Task<ArticleEntity?> GetArticleByArticleNumberAsync(int number);
    }
}
