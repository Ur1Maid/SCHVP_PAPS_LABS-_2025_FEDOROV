using CumList.AppService.Configurations;
using CumList.AppService.Database;
using CumList.AppService.Handlers;
using CumList.AppService.Kafka.Handlers;
using CumList.AppService.Kafka.Models;
using CumList.AppService.Models;
using CumList.AppService.Models.Inputs;
using CumList.AppService.Services.GraphQL;
using CumList.AppService.Types;
using Normalize.DocOperNtsCore.Extensions;
using NTS.Entity.Notifications;
using NTS.Entity.Operations;
using NTS.GraphQL.Extensions;
using NTS.GraphQL.Subscriptions;
using NTS.Logging;
using NTS.Redis.Extensions;

namespace CumList.AppService.Services.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        //Log
        builder.AddSerilog();

        var services = builder.Services;

        //Database
        services.AddNormalizeDocOperNtsDatabaseContext<DatabaseContext>();

        //GraphQL
        builder.AddGraphQLServices(
            "cumlist",
            executorBuilder =>
            {
                executorBuilder
                    .AddMutationType<Mutation>()
                    .AddErrorCodeType<DocumentOperationErrorCode>();
            },
            GraphQLSubscriptionsProvider.Redis()
        );

        //Redis
        services.AddRedisServices();

        //Kafka
        services.AddNormalizeDocOperNtsKafkaServices<
            KafkaConfiguration,
            EntityIdOperationSuccessNotification<long>,
            DatabaseContext
        >(configure =>
        {
            configure.SetNormalizedHandler<NormalizedDocHandler>();

            configure.SetIntegrationReplyHandler<IntegrationModuleReplyHandler>();

            configure.SetDocTypeFactory<INormalizedDocTypeFactory, NormalizedDocTypeFactory>();
        },
        consumerFactoryConfigure =>
        {
            consumerFactoryConfigure.AddManualCommitConsumerPartition<
                CumListOperationsTopicHandler,
                DatabaseContext,
                DocumentOperationTargetModel,
                Exception?
            >(
                x => x.OperationsTopic,
                modelsFactoryConfigure =>
                {
                    modelsFactoryConfigure
                        .AddModel<EntityIdOperationWith<SignDocumentInput, long, CumListOperationType>, SignOperationCumListHandler>(
                            nameof(Mutation.SignDocumentAsync)
                        )
                        .AddModel<EntityIdOperationWith<RejectDocumentInput, long, CumListOperationType>, RejectOperationCumListHandler>(
                            nameof(Mutation.RejectDocumentAsync)
                        );
                });
        });
    }
}
