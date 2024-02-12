using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public record Article {
        /// <summary>
        /// The number of the article to identify it.
        /// </summary>
        public required int ArticleNumber { get; init; }

        /// <summary>
        /// A the display-name of the article.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// The description of the article.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// The width of the article in [mm]
        /// </summary>
        public float? Width { get; init; }

        /// <summary>
        /// The length of the article in [mm]
        /// </summary>
        public float? Length { get; init; }

        /// <summary>
        /// The height of the article in [mm]
        /// </summary>
        public float? Height { get; init; }

        /// <summary>
        /// The weight of the article in [g]
        /// </summary>
        public float? Weight { get; init; }
    }
}
