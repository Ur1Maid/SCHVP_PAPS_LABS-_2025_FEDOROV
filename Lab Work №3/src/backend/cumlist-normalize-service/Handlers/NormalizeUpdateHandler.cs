using CumList.NormalizeService.Constants;
using CumList.NormalizeService.GraphQL.Requests;
using CumList.NormalizeService.GraphQL.Responses;
using CumList.NormalizeService.Types;
using Normalize.Doc.KafkaModels;
using Normalize.DocCore.Handlers;
using NTS.GraphQL.Client;
using NTS.GraphQL.Extensions;

namespace CumList.NormalizeService.Handlers;

internal sealed class NormalizeUpdateHandler : INormalizeDocUpdateHandler
{
    #region private
    private readonly Dictionary<string, CreateModelAsyncDelegate> _messageTypeHandlers;
    private readonly ICumListQueryFactory _queryFactory;
    private readonly IGraphQLClient _client;
    private readonly INormalizeDocTypeFactory _docTypeFactory;

    private async ValueTask<NormalizeDocModel?> GetModelFromGetCumulativeListAsync(
        string message,
        long docId,
        CancellationToken cancellationToken
    )
    {
        using var queryVariables = new QueryVariables();
        queryVariables.AddOrUpdate(JsonPropertyName.ImpСumList.CumId, docId);

        var cumList = await _client.GetItemsAsync<ImpCumList>(
            _queryFactory.GetImpСumListForCheckFromList,
            queryVariables,
            cancellationToken
        ).FirstOrDefaultAsync();

        if (cumList == null)
            return null;

        return _docTypeFactory
            .GetCumListType
            .GetSubType(cumList.CumTypeId)?
            .AsNormalizeDocModel(cumList.CumId);
    }
    #endregion
    public NormalizeUpdateHandler(
        ICumListQueryFactory queryFactory,
        IGraphQLClient client,
        INormalizeDocTypeFactory docTypeFactory
    )
    {
        ArgumentNullException.ThrowIfNull(queryFactory);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(docTypeFactory);

        _queryFactory = queryFactory;
        _client = client;
        _docTypeFactory = docTypeFactory;

        _messageTypeHandlers = new Dictionary<string, CreateModelAsyncDelegate>(StringComparer.OrdinalIgnoreCase)
        {
            ["GetCumulativeList"] = GetModelFromGetCumulativeListAsync
        };
    }

    public ValueTask<NormalizeDocModel?> CreateModelAsync(
        string messageType,
        string message,
        long? docId,
        CancellationToken cancellationToken = default
    )
    {
        if (docId != null && _messageTypeHandlers.TryGetValue(messageType, out var handler))
            return handler(message, docId.Value, cancellationToken);

        return ValueTask.FromResult<NormalizeDocModel?>(null);
    }
}
