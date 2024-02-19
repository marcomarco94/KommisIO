using System.Resources;
using System.Globalization;
using System.Reflection;

namespace MauiClientLibrary.Services;

public class LocalizationService : ILocalizationService
{
    private readonly ResourceManager _resourceManager;

    public LocalizationService()
    {
        _resourceManager = new ResourceManager("LocalizationsLibrary.Localizations.AppResources", Assembly.Load("LocalizationsLibrary"));
    }

    public string GetResourceValue(string key)
    {
        return _resourceManager.GetString(key, CultureInfo.CurrentCulture);
    }
}