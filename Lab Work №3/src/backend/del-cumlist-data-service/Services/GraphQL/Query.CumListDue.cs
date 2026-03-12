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
    /// Получить сборы накопительной ведомости
    /// </summary>
    /// <param name="context">Контекст БД</param>
    /// <param name="queryContext">Контекст запроса</param>
    /// <returns>Список сборов накопительной ведомости</returns>
    [UseOffsetPaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<CumListDue> GetCumListDues(
        DatabaseContext context,
        QueryContext<CumListDue> queryContext
    )
    {
        return context.CumListDues.WhereWith(queryContext);
    }

    /// <summary>
    /// Получить сборы накопительных ведомостей по идентификаторам сборов
    /// </summary>
    /// <param name="id">Идентификаторы сборов</param>
    /// <param name="dataLoader">Загрузчик данных</param>
    /// <param name="queryContext">Контекст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Cборы накопительных ведомостей</returns>
    [Internal]
    public Task<IReadOnlyList<CumListDue?>> GetCumListDuesByIdAsync(
        Guid?[]? id,
        ICumListDueByIdDataLoader dataLoader,
        QueryContext<CumListDue?> queryContext,
        CancellationToken cancellationToken
    )
    {
        return dataLoader.LoadAsync(id, queryContext, cancellationToken);
    }

    /// <summary>
    /// Получить сбор накопительной ведомости по идентификатору сбора
    /// </summary>
    /// <param name="id">Идентификатор сбора</param>
    /// <param name="dataLoader">Загрузчик данных</param>
    /// <param name="queryContext">Контекст запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Cбор накопительной ведомости</returns>
    public Task<CumListDue?> GetCumListDueByIdAsync(
        Guid? id,
        ICumListDueByIdDataLoader dataLoader,
        QueryContext<CumListDue?> queryContext,
        CancellationToken cancellationToken
    )
    {
        return dataLoader.LoadAsync(id, queryContext, cancellationToken);
    }
}
