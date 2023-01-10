using System.Diagnostics;
using System.Reflection;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;

namespace WebApplication2.Extensions;

public static class GraphQlExtensions
{
    public static IRequestExecutorBuilder ConfigureGraphQl(this IServiceCollection serviceCollection)
    {
        var server = serviceCollection.AddGraphQLServer();

        var queryExtensions = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<ExtendObjectTypeAttribute>() != null)
            .ToArray();

        foreach (var type in queryExtensions)
        {
            server = server.AddTypeExtension(type);
        }
        
        return server;
    }
}