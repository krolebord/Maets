using Maets.Attributes;

namespace Maets.Services.Shared.Implementations;

[Dependency(Lifetime = ServiceLifetime.Scoped, Exposes = typeof(IAppUrlProvider))]
public class AppUrlProvider : IAppUrlProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AppUrlProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string GetAppUrl()
    {
        var request = _contextAccessor.HttpContext!.Request;

        return $"{request.Scheme}://{request.Host}{request.PathBase}";
    }
}
