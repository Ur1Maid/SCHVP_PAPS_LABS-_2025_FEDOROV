using System.Collections.Generic;
using System.Linq;
using CumList.DesignPatterns.Structural;

namespace CumList.DesignPatterns.Creational;

public interface IPrototype<T>
{
    T DeepCopy(string? newName = null);
}

public sealed record ColumnSetting(string FieldName, bool IsVisible, int Order);

public sealed class CumListFilterTemplate : IPrototype<CumListFilterTemplate>
{
    public CumListFilterTemplate(string name, FilterGroup rootFilter, IReadOnlyCollection<ColumnSetting> columns)
    {
        Name = name;
        RootFilter = rootFilter;
        Columns = columns.ToList();
    }

    public string Name { get; }
    public FilterGroup RootFilter { get; }
    public IReadOnlyCollection<ColumnSetting> Columns { get; }

    public CumListFilterTemplate DeepCopy(string? newName = null)
    {
        return new CumListFilterTemplate(
            newName ?? $"{Name} (copy)",
            (FilterGroup)RootFilter.DeepCopy(),
            Columns.Select(column => column with { }).ToArray());
    }
}
