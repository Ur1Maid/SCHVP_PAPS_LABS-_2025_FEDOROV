using CumList.DataService.Context;
using CumList.DataService.Models;
using GreenDonut.Data;
using HotChocolate.Data;
using HotChocolate.Fusion.SourceSchema.Types;
using HotChocolate.Types;
using NTS.GraphQL.Extensions;

namespace CumList.DataService.Services.GraphQL;

public partial class Query
{
    /// <summary>
    /// Получить накопительные ведомости
    /// </summary>
    /// <param name="dbContext">Контекст БД</param>
    /// <param name="queryContext">Контекст запроса</param>
    /// <returns>Список накопительных ведомостей</returns>
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<CumListView> GetCumLists(
        DatabaseContext dbContext,
        QueryContext<CumListView> queryContext
    )
    {
        return dbContext.CumListViews.WhereWith(queryContext);
    }

    /// <summary>
    /// Получить накопительные ведомости по идентификаторам документов
    /// </summary>
    /// <param name="docId">Идентификаторы документов</param>
    /// <param name="dataLoader">Загрузчик данных</param>
    /// <param name="queryContext">Контекст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Накопительные ведомости</returns>
    [Internal]
    public Task<IReadOnlyList<CumListView?>> GetCumListsByDocIdAsync(
        long?[]? docId,
        ICumListViewByDocIdDataLoader dataLoader,
        QueryContext<CumListView?> queryContext,
        CancellationToken cancellationToken
    )
    {
        return dataLoader.LoadAsync(docId, queryContext, cancellationToken);
    }

    /// <summary>
    /// Получить накопительную ведомость по идентификатору документа
    /// </summary>
    /// <param name="docId">Идентификатор документа</param>
    /// <param name="dataLoader">Загрузчик данных</param>
    /// <param name="queryContext">Контекст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Накопительная ведомость</returns>
    public Task<CumListView?> GetCumListByDocIdAsync(
        long? docId,
        ICumListViewByDocIdDataLoader dataLoader,
        QueryContext<CumListView?> queryContext,
        CancellationToken cancellationToken
    )
    {
        return dataLoader.LoadAsync(docId, queryContext, cancellationToken);
    }
}
