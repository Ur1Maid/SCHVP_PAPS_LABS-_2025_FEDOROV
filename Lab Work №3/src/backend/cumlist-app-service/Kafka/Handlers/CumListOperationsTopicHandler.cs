using CumList.AppService.Configurations;
using CumList.AppService.Database;
using CumList.AppService.Kafka.Models;
using CumList.AppService.Models;
using Microsoft.EntityFrameworkCore;
using NTS.Database.Outbox.Kafka;
using NTS.Entity.Notifications;
using NTS.EtranGateway.Library.KafkaModel.Models;
using NTS.GraphQL.Subscriptions;
using NTS.Kafka.Consumers;
using NTS.Kafka.Consumers.ModelHandlers;
using NTS.Kafka.Consumers.ModelHandlers.Models;
using NTS.Kafka.Producers;
using NTS.Kafka.Records;

namespace CumList.AppService.Kafka.Handlers;

internal sealed class CumListOperationsTopicHandler :
    IKafkaConsumerPartitionTargetModelHandler<DatabaseContext, DocumentOperationTargetModel, Exception?>
{
    #region private
    private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
    private readonly IGraphQLSubscriptionsProducer _subscriptionsProducer;
    private readonly IKafkaProducerFactory<KafkaConfiguration> _kafkaProducerFactory;
    private readonly ILogger<CumListOperationsTopicHandler> _logger;
    private readonly KafkaOutboxHandler _outboxHandler;

    private sealed class KafkaOutboxHandler(CumListOperationsTopicHandler owner) : KafkaOutboxHandler<
        DatabaseContext,
        RecordAsModelTarget<
            DatabaseContext,
            DocumentOperationTargetModel,
            Exception?
        >,
        DocumentOperationTargetModel?
    >(owner._dbContextFactory, owner._logger)
    {
        #region private
        private readonly SemaphoreSlim _sendResultMessageLock = new(1, 1);

        private async Task SendToIntegrationModelAsync(
            IntegrationModuleRequestModel? requestModel,
            CancellationToken cancellationToken
        )
        {
            if(requestModel == null)
                return;

            await _sendResultMessageLock.WaitAsync(cancellationToken);

            try
            {
                await owner._kafkaProducerFactory.SendRecordAsync(
                    x => x.IntegrationModuleRequestTopic,
                    requestModel,
                    requestModel.DocId,
                    cancellationToken: cancellationToken
                );
            }
            finally
            {
                _sendResultMessageLock.Release();
            }
        }

        private Task SendToNotificationAsync(
            EntityIdOperationNotifications<DocumentOperationErrorCode, long, CumListOperationType> notification,
            CancellationToken cancellationToken
        )
        {
            return notification.SendNotificationsAsync(
                factorySendResultNotification: owner._subscriptionsProducer.SendMessageAsJsonAsync,
                factorySendSuccessNotification: owner._subscriptionsProducer.SendMessageAsJsonAsync,
                cancellationToken
            );
        }
        #endregion
        #region protected
        protected override Task HandleModelAsync(
            RecordAsModelTarget<DatabaseContext, DocumentOperationTargetModel, Exception?> model,
            DatabaseContext dbContext,
            CancellationToken cancellationToken
        )
        {
            return model.HandleAsync(dbContext, cancellationToken);
        }

        protected override DocumentOperationTargetModel? CreateMessage(
            RecordAsModelTarget<DatabaseContext, DocumentOperationTargetModel, Exception?> model,
            Exception? exception
        )
        {
            return model.CreateTarget(exception);
        }

        protected override async Task SendMessageAsync(
            DocumentOperationTargetModel? message,
            CancellationToken cancellationToken
        )
        {
            if (message == null)
                return;

            var msg = message.Value;

            await SendToIntegrationModelAsync(msg.IntegrationModuleRequest, cancellationToken);

            await SendToNotificationAsync(msg.Notification, cancellationToken);
        }
        #endregion
    }
    #endregion
    public CumListOperationsTopicHandler(
        IDbContextFactory<DatabaseContext> dbContextFactory,
        IGraphQLSubscriptionsProducer subscriptionsProducer,
        IKafkaProducerFactory<KafkaConfiguration> kafkaProducerFactory,
        ILogger<CumListOperationsTopicHandler> logger
    )
    {
        ArgumentNullException.ThrowIfNull(dbContextFactory);
        ArgumentNullException.ThrowIfNull(subscriptionsProducer);
        ArgumentNullException.ThrowIfNull(kafkaProducerFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _dbContextFactory = dbContextFactory;
        _subscriptionsProducer = subscriptionsProducer;
        _kafkaProducerFactory = kafkaProducerFactory;
        _logger = logger;

        _outboxHandler = new KafkaOutboxHandler(this);
    }

    public Task HandleAsync(
        KafkaConsumerRecord<EntityOperationRecord> consumerRecord,
        RecordAsModelTarget<
            DatabaseContext,
            DocumentOperationTargetModel,
            Exception?
        > recordAsModel,
        CancellationToken cancellationToken = default
    )
    {
        return _outboxHandler.HandleAsync(KafkaOutboxModel.Create(
            recordAsModel,
            consumerRecord.GroupId,
            consumerRecord.TopicName,
            consumerRecord.PartitionNumber,
            consumerRecord.Offset,
            () => consumerRecord.RawData
        ), cancellationToken);
    }

    public void PartitionAssigned(KafkaConsumerPartition partition)
    {
        _outboxHandler.HandleChangePartition(new KafkaOutboxPartition(
            partition.GroupId,
            partition.TopicName,
            partition.PartitionNumber
        ));
    }
}
