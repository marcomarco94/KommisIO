
namespace MauiClientLibrary.Models;
public class MenuItemModel 
{
    public string Title { get; set; } = String.Empty;   
    public string Icon { get; set; } =String.Empty;
    public string Route { get; set; } = String.Empty;
    public Role RequiredRole { get; set; }
}