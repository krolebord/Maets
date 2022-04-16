namespace Maets.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DependencyAttribute : Attribute
{
    public Type? Exposes { get; init; }

    public ServiceLifetime Lifetime { get; init; } = ServiceLifetime.Transient;
}
