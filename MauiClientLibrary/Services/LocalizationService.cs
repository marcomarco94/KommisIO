using System.Resources;
using System.Globalization;
using System.Reflection;

namespace MauiClientLibrary.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class LocalizationService : ILocalizationService
{
    private readonly ResourceManager _resourceManager;

    /// <summary>
    /// LocalizationService constructor sets up the resource manager
    /// </summary>
    public LocalizationService()
    {
        _resourceManager = new ResourceManager("LocalizationsLibrary.Localizations.AppResources", Assembly.Load("LocalizationsLibrary"));
    }
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string GetResourceValue(string key)
    {
        return _resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? string.Empty;
    }
}