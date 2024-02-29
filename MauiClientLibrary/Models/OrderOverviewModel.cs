using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiClientLibrary.Models
{
    public class OrderOverviewModel
    {
        public string Title { get; set; } = String.Empty;
        public Role RequiredRole { get; set; }
    }
}
