using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CumList.DesignPatterns.Domain;

public enum CumListAction
{
    Sign,
    Reject
}

public enum LogicalOperator
{
    And,
    Or
}

public sealed record CumListDue(long Id, decimal AmountRub, string ChargeCode);

public sealed record CumListDocument(long Id, string Number, string StateName, IReadOnlyCollection<CumListDue> Dues);

public sealed record CumListCard(
    CumListDocument Document,
    IReadOnlyCollection<string> History,
    IReadOnlyCollection<string> Rules,
    IReadOnlyCollection<string> RelatedDocuments);

public sealed record OperationEnvelope(
    Guid CorrelationId,
    CumListAction Action,
    long DocId,
    long UserId,
    int? RejectReasonId = null,
    string? RejectComment = null);

public sealed record IntegrationRequest(string DocId, string CorrelationId, string MessageType, string Message);

public sealed record ExternalOperationReply(string Status, string? ErrorText, string? ExternalState);

public sealed record InternalOperationResult(bool IsSuccess, string InternalState, string? ErrorCode, string? Message);

public sealed record DocumentStateChanged(long DocId, string StateName, Guid CorrelationId);

public interface ICumListRepository
{
    Task<CumListDocument?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task SaveAsync(CumListDocument document, CancellationToken cancellationToken = default);
}

public interface IHistoryRepository
{
    Task<IReadOnlyCollection<string>> GetHistoryAsync(long docId, CancellationToken cancellationToken = default);
}

public interface IRulesRepository
{
    Task<IReadOnlyCollection<string>> GetRulesAsync(long docId, CancellationToken cancellationToken = default);
}

public interface IRelatedDocumentsRepository
{
    Task<IReadOnlyCollection<string>> GetRelatedDocumentsAsync(long docId, CancellationToken cancellationToken = default);
}

public interface ICacheStore
{
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value, TimeSpan ttl);
}

public interface INotificationPublisher
{
    Task PublishAsync(DocumentStateChanged @event, CancellationToken cancellationToken = default);
}

public interface IOperationBus
{
    Task EnqueueAsync(IntegrationRequest request, CancellationToken cancellationToken = default);
}

public interface IOperationAudit
{
    Task RegisterAsync(long docId, string action, string result, long userId, Guid correlationId, CancellationToken cancellationToken = default);
}

public interface ICumListCardService
{
    Task<CumListCard> GetCardAsync(long docId, CancellationToken cancellationToken = default);
}
