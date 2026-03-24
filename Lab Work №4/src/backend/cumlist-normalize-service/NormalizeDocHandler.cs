using CumList.NormalizeService.Constants;
using CumList.NormalizeService.Database;
using CumList.NormalizeService.Database.Models;
using CumList.NormalizeService.GraphQL.Requests;
using CumList.NormalizeService.GraphQL.Responses;
using CumList.NormalizeService.Mappers;
using CumList.NormalizeService.Models;
using CumList.NormalizeService.Types;
using Microsoft.EntityFrameworkCore;
using Normalize.Doc.KafkaModels;
using Normalize.DocCore.Converters;
using Normalize.DocCore.Handlers;
using Normalize.DocCore.Loaders;
using Normalize.DocCore.Models;
using NTS.GraphQL.Client;
using NTS.GraphQL.Extensions;

namespace CumList.NormalizeService.Handlers;

internal sealed class NormalizeDocHandler : NormalizeDocHandler<NormalizeCumList, DatabaseContext>
{
    #region private
    private readonly ICumListQueryFactory _queryFactory;
    private readonly IGraphQLClient _client;
    private readonly INormalizeDocLoaderFactory _loaderFactory;
    private readonly INormalizeDocTypeFactory _docTypeFactory;
    private readonly IMapperFactory _mapperFactory;

    private async Task<Database.Models.CumList?> LoadCumListAsync(
        long cumId,
        CancellationToken cancellationToken
    )
    {
        using var queryVariables = new QueryVariables();
        queryVariables.AddOrUpdate(JsonPropertyName.ImpСumList.CumId, cumId);

        var impCumList = await _client.GetItemsAsync<ImpCumList>(
            _queryFactory.GetImpСumListByCumIdFromList,
            queryVariables,
            cancellationToken
        ).FirstOrDefaultAsync();

        if (impCumList == null)
            return null;

        var cumList = _mapperFactory.CreateCumList(impCumList);

        if (cumList.StationId != null)
            return cumList;

        queryVariables.Clear();
        cumList.StationId = await GetStationIdFromOrganizationAsync(
            impCumList.CumRzdOrgId,
            impCumList.CumRzdOrgCode,
            impCumList.CumRzdOrgName,
            impCumList.CumDateCreate,
            queryVariables,
            cancellationToken
        );

        return cumList;
    }

    private async Task<long?> GetStationIdFromOrganizationAsync(
        long? orgId,
        string? orgCode,
        string? orgName,
        DateTime? requestDate,
        QueryVariables queryVariables,
        CancellationToken cancellationToken
    )
    {
        if (orgId == null)
            return null;

        if (string.IsNullOrWhiteSpace(orgCode) || string.IsNullOrWhiteSpace(orgName))
            return null;

        queryVariables.Clear();
        queryVariables.AddOrUpdate(JsonPropertyName.ImpOrgPassport.Id, orgId);
        queryVariables.AddOrUpdate(JsonPropertyName.Request.RequestDate, requestDate);

        var impOrgPassport = await _client.GetItemsAsync<ImpOrgPassport>(
            _queryFactory.GetImpOrgPassportByIdOnRequestDateFromList,
            queryVariables,
            cancellationToken
        ).FirstOrDefaultAsync();

        if (impOrgPassport == null)
            return null;

        if (!string.Equals(impOrgPassport.TypeName, "Станция", StringComparison.OrdinalIgnoreCase))
            return null;

        queryVariables.Clear();
        queryVariables.AddOrUpdate(JsonPropertyName.NsiStation.CodeOsjd, orgCode);
        queryVariables.AddOrUpdate(JsonPropertyName.NsiStation.Name, orgName);
        queryVariables.AddOrUpdate(JsonPropertyName.Request.RequestDate, requestDate);

        var nsiStation = await _client.GetItemsAsync<NsiStation>(
            _queryFactory.GetNsiStationByIdOnRequestDateFromList,
            queryVariables,
            cancellationToken
        ).FirstOrDefaultAsync();

        return nsiStation?.Id;
    }

    private async Task<List<CumListDue>> LoadCumListDuesAsync(
        long cumId,
        CancellationToken cancellationToken
    )
    {
        var cumListDues = new List<CumListDue>();

        using var queryVariables = new QueryVariables();
        queryVariables.AddOrUpdate(JsonPropertyName.ImpСumListDue.CumId, cumId);
        queryVariables.AddOrUpdate(JsonPropertyNamePagination.Skip, 0);

        var asyncEnumerable = _client.GetItemsAsync<ImpCumListDue>(
            _queryFactory.GetImpСumListDuesByCumId,
            queryVariables,
            cancellationToken
        );

        await foreach (ReadOnlySpan<ImpCumListDue> rawDues in asyncEnumerable)
        {
            foreach (var rawDue in rawDues)
            {
                cumListDues.Add(_mapperFactory.CreateCumListDue(rawDue));
            }
        }

        return cumListDues;
    }
    #endregion
    #region protected
    protected override async Task DeleteModelAsync(NormalizeCumList model, long docId, DatabaseContext context, CancellationToken cancellationToken)
    {
        await context.CumLists.Where(x => x.DocId == docId).ExecuteDeleteAsync(cancellationToken);
        await context.CumListDues.Where(x => x.DocId == docId).ExecuteDeleteAsync(cancellationToken);
    }

    protected override async Task AddModelAsync(NormalizeCumList model, DatabaseContext context, CancellationToken cancellationToken)
    {
        await context.CumLists.AddAsync(model.CumList, cancellationToken);

        if (model.CumListDues.Count != 0)
            await context.CumListDues.AddRangeAsync(model.CumListDues, cancellationToken);
    }
    #endregion
    public NormalizeDocHandler(
        INormalizeDocIdConverter idConverter,
        ICumListQueryFactory queryFactory,
        IGraphQLClient client,
        INormalizeDocLoaderFactory loaderFactory,
        INormalizeDocTypeFactory docTypeFactory,
        IMapperFactory mapperFactory
    ) : base(idConverter)
    {
        ArgumentNullException.ThrowIfNull(queryFactory);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(loaderFactory);
        ArgumentNullException.ThrowIfNull(docTypeFactory);
        ArgumentNullException.ThrowIfNull(mapperFactory);

        _queryFactory = queryFactory;
        _client = client;
        _loaderFactory = loaderFactory;
        _docTypeFactory = docTypeFactory;
        _mapperFactory = mapperFactory;
    }

    public override ValueTask<bool> IsHandleAvailable(
        NormalizeDocModel model,
        CancellationToken cancellationToken = default
    )
    {
        return ValueTask.FromResult(_docTypeFactory.IsDocTypeAvailable(model));
    }

    public override async Task<NormalizeModel<NormalizeCumList>?> CreateNormalizeModelAsync(
        NormalizeDocModel model,
        CancellationToken cancellationToken = default
    )
    {
        var normalizeDoc = await _loaderFactory.LoadNormalizeDocAsync(model, cancellationToken);
        if (normalizeDoc == null)
            return null;

        var cumId = IdConverter.GetWithoutPrefixFrom(model.DocId);

        var loadCumListTask = LoadCumListAsync(cumId, cancellationToken);
        var loadCumListDuesTask = LoadCumListDuesAsync(cumId, cancellationToken);

        await Task.WhenAll(loadCumListTask, loadCumListDuesTask);

        var cumList = loadCumListTask.Result;
        if (cumList == null)
            return null;

        var cumListDues = loadCumListDuesTask.Result;

        return new NormalizeModel<NormalizeCumList>
        {
            Model = new NormalizeCumList
            {
                CumList = cumList,
                CumListDues = cumListDues
            },
            NormalizeDoc = normalizeDoc.Value
        };
    }
}
