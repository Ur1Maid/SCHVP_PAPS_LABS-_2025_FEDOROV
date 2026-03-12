using CumList.NormalizeService.Configurations;
using CumList.NormalizeService.Constants;
using CumList.NormalizeService.GraphQL.Requests;
using CumList.NormalizeService.GraphQL.Responses;
using CumList.NormalizeService.Types;
using Normalize.DocCore.Extensions;
using Normalize.DocCore.Handlers;
using Normalize.DocCore.Loaders;
using NTS.GraphQL.Client;
using NTS.Kafka.Producers;

namespace CumList.NormalizeService.Handlers;

internal sealed class NormalizeSyncHandler : INormalizeDocSyncHandler
{
    #region private
    private readonly ICumListQueryFactory _queryFactory;
    private readonly IGraphQLClient _client;
    private readonly INormalizeDocLoaderFactory _loaderFactory;
    private readonly INormalizeDocTypeFactory _docTypeFactory;
    private readonly IKafkaProducerFactory<KafkaConfiguration> _kafkaProducerFactory;

    private Task NormalizeCumListsAsync(IEnumerable<ImpCumList> cumLists, CancellationToken cancellationToken)
    {
        return _kafkaProducerFactory.SendDocForNormalizeAsync(
            cumLists,
            _docTypeFactory.GetCumListType,
            static (cumList, cumListType) => cumListType.GetSubType(cumList.CumTypeId)?.AsNormalizeDocModel(cumList.CumId),
            cancellationToken
        );
    }
    #endregion
    public NormalizeSyncHandler(
        ICumListQueryFactory queryFactory,
        IGraphQLClient client,
        INormalizeDocLoaderFactory loaderFactory,
        INormalizeDocTypeFactory docTypeFactory,
        IKafkaProducerFactory<KafkaConfiguration> kafkaProducerFactory
    )
    {
        ArgumentNullException.ThrowIfNull(queryFactory);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(loaderFactory);
        ArgumentNullException.ThrowIfNull(docTypeFactory);
        ArgumentNullException.ThrowIfNull(kafkaProducerFactory);

        _queryFactory = queryFactory;
        _client = client;
        _loaderFactory = loaderFactory;
        _docTypeFactory = docTypeFactory;
        _kafkaProducerFactory = kafkaProducerFactory;
    }

    public async Task HandleAsync(CancellationToken cancellationToken = default)
    {
        if (await _loaderFactory.IsDocumentNormalizedForTypeAsync(_docTypeFactory.DocTypeIdArray, cancellationToken))
            return;

        using var queryVariables = new QueryVariables();
        queryVariables.AddOrUpdate(JsonPropertyName.ImpСumList.CumTypeId, _docTypeFactory.SubTypeIdArray);
        queryVariables.AddOrUpdate(JsonPropertyNamePagination.Skip, 0);

        var asyncEnumerator = _client.GetItemsAsync<ImpCumList>(
            _queryFactory.GetImpCumListIdsByTypes,
            queryVariables,
            cancellationToken
        );

        await foreach (var items in asyncEnumerator)
        {
            await _loaderFactory.IsDocumentsNormalizedAsync(
                items,
                cumList => cumList.CumId,
                NormalizeCumListsAsync,
                cancellationToken
            );
        }
    }
}
