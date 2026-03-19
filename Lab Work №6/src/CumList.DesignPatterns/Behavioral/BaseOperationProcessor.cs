using System;
using System.Threading;
using System.Threading.Tasks;
using CumList.DesignPatterns.Creational;
using CumList.DesignPatterns.Domain;
using CumList.DesignPatterns.Structural;

namespace CumList.DesignPatterns.Behavioral;

public abstract class BaseOperationProcessor(
    ICumListRepository repository,
    ICumListOperationHandlerFactory handlerFactory,
    IOperationBus operationBus,
    IOperationAudit audit,
    DocumentStateChangedSubject subject)
{
    public async Task<InternalOperationResult> ProcessAsync(OperationEnvelope envelope, CancellationToken cancellationToken = default)
    {
        var document = await LoadAsync(envelope.DocId, cancellationToken);
        EnsureAllowed(document);

        var request = await CreateRequestAsync(envelope, cancellationToken);
        await RegisterPendingAsync(document, envelope, cancellationToken);
        await SendRequestAsync(request, cancellationToken);
        await NotifyAsync(document, envelope, cancellationToken);

        return new InternalOperationResult(true, "Выполнение операции", null, null);
    }

    protected virtual async Task<CumListDocument> LoadAsync(long docId, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(docId, cancellationToken)
            ?? throw new InvalidOperationException($"CumList {docId} not found.");
    }

    protected abstract void EnsureAllowed(CumListDocument document);
    protected abstract CumListAction Action { get; }

    protected virtual Task<IntegrationRequest> CreateRequestAsync(OperationEnvelope envelope, CancellationToken cancellationToken)
        => handlerFactory.Create(Action).HandleAsync(envelope, cancellationToken);

    protected virtual Task RegisterPendingAsync(CumListDocument document, OperationEnvelope envelope, CancellationToken cancellationToken)
        => audit.RegisterAsync(document.Id, Action.ToString(), "Pending", envelope.UserId, envelope.CorrelationId, cancellationToken);

    protected virtual Task SendRequestAsync(IntegrationRequest request, CancellationToken cancellationToken)
        => operationBus.EnqueueAsync(request, cancellationToken);

    protected virtual Task NotifyAsync(CumListDocument document, OperationEnvelope envelope, CancellationToken cancellationToken)
        => subject.NotifyAsync(new DocumentStateChanged(document.Id, "Выполнение операции", envelope.CorrelationId), cancellationToken);
}

public sealed class SignOperationProcessor(
    ICumListRepository repository,
    ICumListOperationHandlerFactory handlerFactory,
    IOperationBus operationBus,
    IOperationAudit audit,
    DocumentStateChangedSubject subject)
    : BaseOperationProcessor(repository, handlerFactory, operationBus, audit, subject)
{
    protected override CumListAction Action => CumListAction.Sign;

    protected override void EnsureAllowed(CumListDocument document)
    {
        if (!string.Equals(document.StateName, "На подписи", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Подписание доступно только в состоянии 'На подписи'.");
        }
    }
}

public sealed class RejectOperationProcessor(
    ICumListRepository repository,
    ICumListOperationHandlerFactory handlerFactory,
    IOperationBus operationBus,
    IOperationAudit audit,
    DocumentStateChangedSubject subject)
    : BaseOperationProcessor(repository, handlerFactory, operationBus, audit, subject)
{
    protected override CumListAction Action => CumListAction.Reject;

    protected override void EnsureAllowed(CumListDocument document)
    {
        if (!string.Equals(document.StateName, "На подписи", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Отклонение доступно только в состоянии 'На подписи'.");
        }
    }
}
