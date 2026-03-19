using System;
using System.Threading;
using System.Threading.Tasks;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Structural;

public sealed class CumListCardService(CumListCardFacade facade) : ICumListCardService
{
    public Task<CumListCard> GetCardAsync(long docId, CancellationToken cancellationToken = default)
        => facade.GetCardAsync(docId, cancellationToken);
}

public sealed class CachedCumListCardServiceDecorator(ICumListCardService inner, ICacheStore cache) : ICumListCardService
{
    public Task<CumListCard> GetCardAsync(long docId, CancellationToken cancellationToken = default)
    {
        var key = $"cumlist-card:{docId}";
        if (cache.TryGet<CumListCard>(key, out var cached) && cached is not null)
        {
            return Task.FromResult(cached);
        }

        return LoadAndCacheAsync(key, docId, cancellationToken);
    }

    private async Task<CumListCard> LoadAndCacheAsync(string key, long docId, CancellationToken cancellationToken)
    {
        var card = await inner.GetCardAsync(docId, cancellationToken);
        cache.Set(key, card, TimeSpan.FromMinutes(5));
        return card;
    }
}

public sealed class LoggedCumListCardServiceDecorator(ICumListCardService inner, IOperationAudit audit) : ICumListCardService
{
    public async Task<CumListCard> GetCardAsync(long docId, CancellationToken cancellationToken = default)
    {
        var card = await inner.GetCardAsync(docId, cancellationToken);
        await audit.RegisterAsync(docId, "OpenCard", "Read", 0, Guid.Empty, cancellationToken);
        return card;
    }
}
