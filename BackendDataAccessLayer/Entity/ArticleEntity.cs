using DataRepoCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendDataAccessLayer.Entity {
    public class ArticleEntity {
        /// <summary>
        /// The tech. id of the article.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The bus. identifyer for the article
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// The name of the article.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description describing the article.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The width of the article.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The height of the article.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// The length of the article.
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// The weight of the article.
        /// </summary>
        public float Weight { get; set; }

        public ArticleEntity(int id, int articleId, string name, string description, float width, float height, float length, float weight) {
            Id = id; 
            ArticleId = articleId; 
            Name = name; 
            Description = description; 
            Width = width; 
            Height = height; 
            Length = length; 
            Weight = weight;
        }

        public ArticleEntity(Article article) {
            ArticleId = article.Id;
            Name = article.Name;
            Description = article.Description ?? string.Empty;
            Width = article.Width;
            Height = article.Height;
            Length = article.Length;
            Weight = article.Weight;
        }

        public Article MapToDataModel() {
            return new Article() {
                ArticleNumber = ArticleId,
                Name = Name,
                Description = Description,
                Width = Width,
                Height = Height,
                Weight = Weight,
                Length = Length,
                Id = Id
            };
        }
    }
}