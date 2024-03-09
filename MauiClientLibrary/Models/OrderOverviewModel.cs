
namespace MauiClientLibrary.Models
{
    public class OrderOverviewModel
    {
        public string Title { get; set; } = String.Empty;
        public Role RequiredRole { get; set; }
        
        public Func<Task<IEnumerable<PickingOrder>>>? Function { get; set; }
    }
}
