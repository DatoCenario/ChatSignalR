using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.GraphQL;

namespace WebApplication2.Extensions;

public static class GraphQlExtensions
{
    public static IRequestExecutorBuilder ConfigureGraphQl(this IServiceCollection serviceCollection)
    {
        var server = serviceCollection.AddGraphQLServer()
            .AddErrorFilter<ErrorFilter>()
            .AddAuthorization();

        var queryExtensions = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<ExtendObjectTypeAttribute>() != null)
            .ToArray();

        foreach (var type in queryExtensions)
        {
            server = server.AddTypeExtension(type);
        }

        server = server
            .BindRuntimeType<DateTime, DateType>()
            .AddQueryType(d => d.Name("Query"))
            .AddMutationType(d => d.Name("Mutation"));
        
        return server;
    }
}