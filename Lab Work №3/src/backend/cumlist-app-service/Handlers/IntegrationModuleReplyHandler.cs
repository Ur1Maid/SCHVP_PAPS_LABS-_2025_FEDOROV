using CumList.AppService.Constants;
using CumList.AppService.Types;
using Normalize.DocOperNtsCore.Database.Models;
using Normalize.DocOperNtsCore.Handlers;
using Normalize.DocOperNtsCore.Models;
using NTS.Entity.Notifications;
using NTS.Entity.Operations;
using NTS.GraphQL.Subscriptions;

namespace CumList.AppService.Handlers;

internal sealed class IntegrationModuleReplyHandler :
    IIntegrationModuleReplyHandler<EntityIdOperationSuccessNotification<long>>
{
    #region private
    private static readonly Dictionary<string, long> MessageTypeDocIds = new(StringComparer.OrdinalIgnoreCase)
    {
        [IntergrationModuleMessageType.SetCumListAgreement] = AvailableDocType.CumList
    };

    private static readonly Dictionary<int, int> NextResultIds = new()
    {
        [AvailableDocOperResultNts.Sign.Wait] = AvailableDocOperResultNts.Sign.Error,
        [AvailableDocOperResultNts.Reject.Wait] = AvailableDocOperResultNts.Reject.Error
    };

    private readonly INormalizedDocTypeFactory _docTypeFactory;
    private readonly IGraphQLSubscriptionsProducer _subscriptionsProducer;
    #endregion
    public IntegrationModuleReplyHandler(
        INormalizedDocTypeFactory docTypeFactory,
        IGraphQLSubscriptionsProducer subscriptionsProducer
    )
    {
        ArgumentNullException.ThrowIfNull(docTypeFactory);
        ArgumentNullException.ThrowIfNull(subscriptionsProducer);

        _docTypeFactory = docTypeFactory;
        _subscriptionsProducer = subscriptionsProducer;
    }

    public ValueTask<long?> GetDocTypeIdAsync(
        string messageType,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.FromResult<long?>(
            MessageTypeDocIds.TryGetValue(messageType, out var docTypeId) ? docTypeId : null
        );
    }

    public ValueTask<NormalizeDocOperNtsNextResult?> GetNextResultAsync(
        long docTypeId,
        DocOperNts docOperNts,
        int? errorCode,
        string? errorMessage,
        CancellationToken cancellationToken = default
    )
    {
        //skip if no errors
        if (errorCode == null && errorMessage == null)
            return ValueTask.FromResult<NormalizeDocOperNtsNextResult?>(null);

        //skip if no result
        if (docOperNts.ResultId == null)
            return ValueTask.FromResult<NormalizeDocOperNtsNextResult?>(null);

        return ValueTask.FromResult<NormalizeDocOperNtsNextResult?>(
            new NormalizeDocOperNtsNextResult(
                Result: NextResultIds[docOperNts.ResultId.Value]
            )
        );
    }

    public ValueTask<EntityIdOperationSuccessNotification<long>> CreateNotificationModelAsync(
        long docTypeId,
        long docId,
        NormalizeDocOperNtsNextResult nextResult,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.FromResult(new EntityIdOperationSuccessNotification<long>(
            _docTypeFactory.GetName(docTypeId),
            docId,
            EntityOperationType.Update
        ));
    }

    public Task SendNotificationAsync(
        EntityIdOperationSuccessNotification<long> model,
        CancellationToken cancellationToken = default
    )
    {
        return _subscriptionsProducer.SendMessageAsJsonAsync(model, cancellationToken);
    }
}
