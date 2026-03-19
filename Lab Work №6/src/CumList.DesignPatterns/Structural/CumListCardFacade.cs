using System.Threading;
using System.Threading.Tasks;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Structural;

public sealed class CumListCardFacade(
    ICumListRepository cumListRepository,
    IHistoryRepository historyRepository,
    IRulesRepository rulesRepository,
    IRelatedDocumentsRepository relatedDocumentsRepository)
{
    public async Task<CumListCard> GetCardAsync(long docId, CancellationToken cancellationToken = default)
    {
        var document = await cumListRepository.GetByIdAsync(docId, cancellationToken)
            ?? throw new InvalidOperationException($"CumList {docId} not found.");

        var history = await historyRepository.GetHistoryAsync(docId, cancellationToken);
        var rules = await rulesRepository.GetRulesAsync(docId, cancellationToken);
        var relatedDocuments = await relatedDocumentsRepository.GetRelatedDocumentsAsync(docId, cancellationToken);

        return new CumListCard(document, history, rules, relatedDocuments);
    }
}
