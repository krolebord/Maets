using System.Reflection;
using Maets.Attributes;

namespace Maets.Extensions;

public static class DIExtensions
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Select(x => new
            {
                Type = x,
                DependencyAttribute = x.GetCustomAttribute<DependencyAttribute>()
            })
            .Where(x => x.DependencyAttribute is not null);

        foreach (var dependency in types)
        {
            services.Add(new ServiceDescriptor(
                dependency.DependencyAttribute!.Exposes ?? dependency.Type,
                dependency.Type,
                dependency.DependencyAttribute!.Lifetime
            ));
        }

        return services;
    }
}
