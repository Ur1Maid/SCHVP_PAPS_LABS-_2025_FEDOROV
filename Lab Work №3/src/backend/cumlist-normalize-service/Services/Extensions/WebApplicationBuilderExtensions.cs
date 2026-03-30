using System.Numerics;
using CumList.NormalizeService.Configurations;
using CumList.NormalizeService.Database;
using CumList.NormalizeService.GraphQL.Requests;
using CumList.NormalizeService.Handlers;
using CumList.NormalizeService.Mappers;
using CumList.NormalizeService.Models;
using CumList.NormalizeService.Types;
using Normalize.DocCore.Extensions;
using NTS.Logging;

namespace CumList.NormalizeService.Services.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        //Log
        builder.AddSerilog();

        var services = builder.Services;

        //Database
        services.AddNormalizeDocDatabaseContext<DatabaseContext>();

        //Federation
        services.AddNormalizeDocGraphQLClientServices<ICumListQueryFactory, CumListQueryFactory>(configure =>
        {
            configure.EnableDocLoader<DatabaseContext, IMapperFactory, MapperFactory>();
        });

        //Kafka
        services.AddNormalizeDocKafkaServices<KafkaConfiguration, NormalizeCumList, DatabaseContext>(configure =>
        {
            configure.SetDocHandler<NormalizeDocHandler>();

            configure.SetSyncHandler<NormalizeSyncHandler>();

            configure.SetUpdateHandler<NormalizeUpdateHandler>();

            configure.SetDocTypeFactory<INormalizeDocTypeFactory, NormalizeDocTypeFactory>();
        });
    }
}
