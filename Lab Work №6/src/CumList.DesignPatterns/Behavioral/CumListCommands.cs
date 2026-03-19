using System.Threading;
using System.Threading.Tasks;
using CumList.DesignPatterns.Creational;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Behavioral;

public interface ICumListOperationCommand
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public sealed class SignCumListCommand(
    OperationEnvelope envelope,
    ICumListOperationHandlerFactory handlerFactory,
    IOperationBus operationBus,
    IOperationAudit audit) : ICumListOperationCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = await handlerFactory.Create(CumListAction.Sign).HandleAsync(envelope, cancellationToken);
        await operationBus.EnqueueAsync(request, cancellationToken);
        await audit.RegisterAsync(envelope.DocId, "Sign", "Pending", envelope.UserId, envelope.CorrelationId, cancellationToken);
    }
}

public sealed class RejectCumListCommand(
    OperationEnvelope envelope,
    ICumListOperationHandlerFactory handlerFactory,
    IOperationBus operationBus,
    IOperationAudit audit) : ICumListOperationCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = await handlerFactory.Create(CumListAction.Reject).HandleAsync(envelope, cancellationToken);
        await operationBus.EnqueueAsync(request, cancellationToken);
        await audit.RegisterAsync(envelope.DocId, "Reject", "Pending", envelope.UserId, envelope.CorrelationId, cancellationToken);
    }
}

public sealed class CumListOperationInvoker
{
    public Task ExecuteAsync(ICumListOperationCommand command, CancellationToken cancellationToken = default)
        => command.ExecuteAsync(cancellationToken);
}
