using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class ArticleRepository : BaseRepository<ArticleEntity>, IArticleRepository {
        public ArticleRepository(DALDbContext context) : base(context, context.Article) {
        }

        public async Task<ArticleEntity?> GetArticleByArticleNumberAsync(int number) {
            return await _dbSet.Where(e => e.ArticleId.Equals(number)).FirstAsync();
        }
    }
}
