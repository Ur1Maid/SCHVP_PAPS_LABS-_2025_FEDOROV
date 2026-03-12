using CumList.DataService.Context;
using CumList.DataService.Services.GraphQL;
using NTS.Database.Extensions;
using NTS.GraphQL.Extensions;
using NTS.Logging;

namespace CumList.DataService.Services.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void GetAddServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        //Log
        builder.AddSerilog();

        //Database
        builder.Services.AddDatabaseContext<DatabaseContext>();

        //GraphQL
        builder.AddGraphQLServices<DatabaseContext>(
            "cumlist",
            executorBuilder =>
            {
                executorBuilder
                    .AddQueryType<Query>()
                    .AddDataServiceTypes();
            });
    }
}
