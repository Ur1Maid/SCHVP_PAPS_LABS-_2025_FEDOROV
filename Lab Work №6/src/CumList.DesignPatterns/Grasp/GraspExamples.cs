using System.Threading;
using System.Threading.Tasks;
using CumList.DesignPatterns.Behavioral;
using CumList.DesignPatterns.Creational;
using CumList.DesignPatterns.Domain;
using CumList.DesignPatterns.Structural;

namespace CumList.DesignPatterns.Grasp;

// Controller
public sealed class CumListOperationsController(CumListOperationInvoker invoker)
{
    public Task SignAsync(ICumListOperationCommand command, CancellationToken cancellationToken = default)
        => invoker.ExecuteAsync(command, cancellationToken);
}

// Creator
public static class FilterTemplateCreator
{
    public static CumListFilterTemplate CreateDefaultForSigning(FilterGroup rootFilter)
        => new("На подпись сегодня", rootFilter, [new ColumnSetting("docId", true, 0), new ColumnSetting("state", true, 1)]);
}

// Information Expert
public static class CumListRulesExpert
{
    public static bool RequiresRejectReason(CumListAction action) => action == CumListAction.Reject;
}

// Pure Fabrication
public sealed class CumListIntegrationFacade(
    IIntegrationReplyAdapter adapter,
    ICumListOperationHandlerFactory handlerFactory)
{
    public async Task<InternalOperationResult> CreateAndAdaptAsync(OperationEnvelope envelope, ExternalOperationReply reply, CancellationToken cancellationToken = default)
    {
        var request = await handlerFactory.Create(envelope.Action).HandleAsync(envelope, cancellationToken);
        return adapter.Adapt(reply, previousState: "На подписи");
    }
}

// Protected Variations + Low Coupling
public sealed class CumListApplicationService(
    ICumListCardService cardService,
    CumListOperationsController controller)
{
    public Task<CumListCard> GetCardAsync(long docId, CancellationToken cancellationToken = default)
        => cardService.GetCardAsync(docId, cancellationToken);
}
