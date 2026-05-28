using Microsoft.Extensions.Localization;
using MockSrv.Web.Resources;
using System.Reflection;

namespace MockSrv.Web.Services;

public class LocalisationService
{
    private readonly IStringLocalizer _localizer;
    public LocalisationService(IStringLocalizerFactory factory)
    {
        var assembly = typeof(RessourcesPartagees).GetTypeInfo();
        if (assembly != null && assembly.Assembly != null && assembly.Assembly.FullName != null)
        {
            var assemblyName = new AssemblyName(assembly.Assembly.FullName);
            if (assemblyName.Name != null)
                _localizer = factory.Create(nameof(RessourcesPartagees), assemblyName.Name);
        }

    }

    public string this[string name]
    {
        get
        {
            return _localizer[name];
        }
    }
}
