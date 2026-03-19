using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Behavioral;

public interface IDocumentStateObserver
{
    Task UpdateAsync(DocumentStateChanged @event, CancellationToken cancellationToken = default);
}

public sealed class DocumentStateChangedSubject
{
    private readonly List<IDocumentStateObserver> _observers = [];

    public void Attach(IDocumentStateObserver observer) => _observers.Add(observer);
    public void Detach(IDocumentStateObserver observer) => _observers.Remove(observer);

    public async Task NotifyAsync(DocumentStateChanged @event, CancellationToken cancellationToken = default)
    {
        foreach (var observer in _observers)
        {
            await observer.UpdateAsync(@event, cancellationToken);
        }
    }
}

public sealed class CardRefetchObserver(ICumListCardService cardService) : IDocumentStateObserver
{
    public async Task UpdateAsync(DocumentStateChanged @event, CancellationToken cancellationToken = default)
    {
        _ = await cardService.GetCardAsync(@event.DocId, cancellationToken);
    }
}

public sealed class ToastNotificationObserver(INotificationPublisher publisher) : IDocumentStateObserver
{
    public Task UpdateAsync(DocumentStateChanged @event, CancellationToken cancellationToken = default)
        => publisher.PublishAsync(@event, cancellationToken);
}
