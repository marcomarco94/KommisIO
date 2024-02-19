using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer {
    public interface IDemoDataBuilder {
        /// <summary>
        /// Reset the database to the selected demo-dataset.
        /// </summary>
        /// <returns>Returns true if operation was sucessful otherwise false.</returns>
        public Task<bool> BuildDemoDataAsync();
    }
}
