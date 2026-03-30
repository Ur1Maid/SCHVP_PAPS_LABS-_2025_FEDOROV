using CumList.AppService.Constants;
using CumList.AppService.Types;
using Normalize.DocOperNtsCore.Database.Models;
using Normalize.DocOperNtsCore.Handlers;
using Normalize.DocOperNtsCore.Models;
using NTS.Entity.Notifications;
using NTS.Entity.Operations;
using NTS.GraphQL.Subscriptions;

namespace CumList.AppService.Handlers;

internal sealed class NormalizedDocHandler :
    INormalizedDocHandler<EntityIdOperationSuccessNotification<long>>
{
    #region private
    private static readonly Dictionary<int, int> NextResultIds = new()
    {
        [AvailableDocOperResultNts.Sign.Wait] = AvailableDocOperResultNts.Sign.Done,
        [AvailableDocOperResultNts.Reject.Wait] = AvailableDocOperResultNts.Reject.Done
    };

    private readonly INormalizedDocTypeFactory _docTypeFactory;
    private readonly IGraphQLSubscriptionsProducer _subscriptionsProducer;
    #endregion
    public NormalizedDocHandler(
        INormalizedDocTypeFactory docTypeFactory,
        IGraphQLSubscriptionsProducer subscriptionsProducer
    )
    {
        ArgumentNullException.ThrowIfNull(docTypeFactory);
        ArgumentNullException.ThrowIfNull(subscriptionsProducer);

        _docTypeFactory = docTypeFactory;
        _subscriptionsProducer = subscriptionsProducer;
    }

    public bool IsProxyDocUpdateEnabled => true;

    public ValueTask<bool> IsHandleAvailable(
        long docTypeId,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.FromResult(_docTypeFactory.IsDocTypeAvailable(docTypeId));
    }

    public ValueTask<NormalizeDocOperNtsNextResult?> GetNextResultAsync(
        long docTypeId,
        DocOperNts docOperNts,
        CancellationToken cancellationToken = default
    )
    {
        //skip if no result
        if (docOperNts.ResultId == null)
            return ValueTask.FromResult<NormalizeDocOperNtsNextResult?>(null);

        return ValueTask.FromResult<NormalizeDocOperNtsNextResult?>(
            new NormalizeDocOperNtsNextResult(
                Result: NextResultIds[docOperNts.ResultId.Value]
            )
        );
    }

    public ValueTask<EntityIdOperationSuccessNotification<long>> CreateProxyNotificationModelAsync(
        long docTypeId,
        long docId,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.FromResult(new EntityIdOperationSuccessNotification<long>(
            _docTypeFactory.GetName(docTypeId),
            docId,
            EntityOperationType.Update
        ));
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
