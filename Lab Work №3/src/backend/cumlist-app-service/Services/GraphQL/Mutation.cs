using CumList.AppService.Configurations;
using CumList.AppService.Models;
using CumList.AppService.Models.Inputs;
using HotChocolate;
using NTS.Entity.Operations;
using NTS.GraphQL.Extensions;
using NTS.Kafka.Producers;

namespace CumList.AppService.Services.GraphQL;

public sealed class Mutation
{
    #region private
    private readonly IKafkaProducerFactory<KafkaConfiguration> _kafkaProducerFactory;
    #endregion
    public Mutation(IKafkaProducerFactory<KafkaConfiguration> kafkaProducerFactory)
    {
        ArgumentNullException.ThrowIfNull(kafkaProducerFactory);

        _kafkaProducerFactory = kafkaProducerFactory;
    }

    /// <summary>
    /// Подписать накопительную ведомость
    /// </summary>
    /// <param name="input">Описание подписи накопительной ведомости</param>
    /// <param name="contextAccessor">Http контекс вызова</param>
    /// <returns>Идентификатор накопительной ведомости</returns>
    public Task<long> SignDocumentAsync(
        SignDocumentInput input,
        [Service] IHttpContextAccessor contextAccessor)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(contextAccessor);

        return _kafkaProducerFactory.SendOperationAsync(
            configuration => configuration.OperationsTopic,
            nameof(SignDocumentAsync),
            new EntityIdOperationWith<SignDocumentInput, long, CumListOperationType>(
                input.DocId,
                CumListOperationType.Sign,
                input,
                contextAccessor.GetUserId()),
            input.DocId
        );
    }

    /// <summary>
    /// Отклонить накопительную ведомость
    /// </summary>
    /// <param name="input">Описание отклонение накопительной ведомости</param>
    /// <param name="contextAccessor">Http Context Accessor</param>
    /// <returns>Идентификатор накопительной ведомости</returns>
    public Task<long> RejectDocumentAsync(
        RejectDocumentInput input,
        [Service] IHttpContextAccessor contextAccessor)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(contextAccessor);

        return _kafkaProducerFactory.SendOperationAsync(
            configuration => configuration.OperationsTopic,
            nameof(RejectDocumentAsync),
            new EntityIdOperationWith<RejectDocumentInput, long, CumListOperationType>(
                input.DocId,
                CumListOperationType.Reject,
                input,
                contextAccessor.GetUserId()),
            input.DocId
        );
    }
}
