using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CumList.DesignPatterns.Behavioral;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Creational;

public interface ICumListOperationHandler
{
    CumListAction Action { get; }
    Task<IntegrationRequest> HandleAsync(OperationEnvelope envelope, CancellationToken cancellationToken = default);
}

public interface ICumListOperationHandlerFactory
{
    ICumListOperationHandler Create(CumListAction action);
}

public sealed class CumListOperationHandlerFactory : ICumListOperationHandlerFactory
{
    private readonly IReadOnlyDictionary<CumListAction, ICumListOperationHandler> _handlers;

    public CumListOperationHandlerFactory(IEnumerable<ICumListOperationHandler> handlers)
    {
        _handlers = handlers.ToDictionary(handler => handler.Action);
    }

    public ICumListOperationHandler Create(CumListAction action)
    {
        return _handlers.TryGetValue(action, out var handler)
            ? handler
            : throw new InvalidOperationException($"Handler for action '{action}' is not registered.");
    }
}

public sealed class SignOperationHandler(IOperationRequestStrategyResolver strategyResolver) : ICumListOperationHandler
{
    public CumListAction Action => CumListAction.Sign;

    public Task<IntegrationRequest> HandleAsync(OperationEnvelope envelope, CancellationToken cancellationToken = default)
    {
        var strategy = strategyResolver.Resolve(Action);
        return Task.FromResult(new IntegrationRequest(
            DocId: envelope.DocId.ToString(),
            CorrelationId: envelope.CorrelationId.ToString(),
            MessageType: "SetCumListAgreement",
            Message: strategy.BuildPayload(envelope)));
    }
}

public sealed class RejectOperationHandler(IOperationRequestStrategyResolver strategyResolver) : ICumListOperationHandler
{
    public CumListAction Action => CumListAction.Reject;

    public Task<IntegrationRequest> HandleAsync(OperationEnvelope envelope, CancellationToken cancellationToken = default)
    {
        var strategy = strategyResolver.Resolve(Action);
        return Task.FromResult(new IntegrationRequest(
            DocId: envelope.DocId.ToString(),
            CorrelationId: envelope.CorrelationId.ToString(),
            MessageType: "SetCumListAgreement",
            Message: strategy.BuildPayload(envelope)));
    }
}
