using System;
using System.Collections.Generic;
using System.Linq;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Behavioral;

public interface IOperationRequestStrategy
{
    CumListAction Action { get; }
    string BuildPayload(OperationEnvelope envelope);
}

public interface IOperationRequestStrategyResolver
{
    IOperationRequestStrategy Resolve(CumListAction action);
}

public sealed class OperationRequestStrategyResolver(IEnumerable<IOperationRequestStrategy> strategies)
    : IOperationRequestStrategyResolver
{
    private readonly IReadOnlyDictionary<CumListAction, IOperationRequestStrategy> _strategies =
        strategies.ToDictionary(strategy => strategy.Action);

    public IOperationRequestStrategy Resolve(CumListAction action)
    {
        return _strategies.TryGetValue(action, out var strategy)
            ? strategy
            : throw new InvalidOperationException($"Strategy for action '{action}' is not registered.");
    }
}

public sealed class SignOperationRequestStrategy : IOperationRequestStrategy
{
    public CumListAction Action => CumListAction.Sign;

    public string BuildPayload(OperationEnvelope envelope) => "{\"action\":1}";
}

public sealed class RejectOperationRequestStrategy : IOperationRequestStrategy
{
    public CumListAction Action => CumListAction.Reject;

    public string BuildPayload(OperationEnvelope envelope)
    {
        return $$"{\"action\":2,\"discordId\":{{envelope.RejectReasonId ?? 0}},\"discordText\":\"{{envelope.RejectComment ?? string.Empty}}\"}";
    }
}
