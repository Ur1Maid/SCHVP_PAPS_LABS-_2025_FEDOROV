using System;
using System.Collections.Generic;
using System.Linq;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Structural;

public abstract class FilterNode
{
    public abstract object ToGraphQl();
    public abstract FilterNode DeepCopy();
}

public sealed class FilterLeaf(string field, string operation, object? value) : FilterNode
{
    public string Field { get; } = field;
    public string Operation { get; } = operation;
    public object? Value { get; } = value;

    public override object ToGraphQl() => new Dictionary<string, object?>
    {
        [Field] = new Dictionary<string, object?> { [Operation] = Value }
    };

    public override FilterNode DeepCopy() => new FilterLeaf(Field, Operation, Value);
}

public sealed class FilterGroup(LogicalOperator @operator) : FilterNode
{
    private readonly List<FilterNode> _children = [];

    public LogicalOperator Operator { get; } = @operator;
    public IReadOnlyCollection<FilterNode> Children => _children;

    public void Add(FilterNode child) => _children.Add(child);

    public override object ToGraphQl() => new Dictionary<string, object?>
    {
        [Operator == LogicalOperator.And ? "and" : "or"] = _children.Select(x => x.ToGraphQl()).ToArray()
    };

    public override FilterNode DeepCopy()
    {
        var copy = new FilterGroup(Operator);
        foreach (var child in _children)
        {
            copy.Add(child.DeepCopy());
        }

        return copy;
    }
}
