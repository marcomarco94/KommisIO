namespace MauiClientLibrary.Services;

/// <summary>
/// Localization service provides the localization resources
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Function to get the resource value based on the key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string GetResourceValue(string key);
}