using System;
using CumList.DesignPatterns.Domain;
using CumList.DesignPatterns.Structural;

namespace CumList.DesignPatterns.Creational;

public sealed class CumListFilterBuilder
{
    private readonly FilterGroup _root;

    public CumListFilterBuilder(LogicalOperator logicalOperator = LogicalOperator.And)
    {
        _root = new FilterGroup(logicalOperator);
    }

    public CumListFilterBuilder ByState(string state)
    {
        _root.Add(new FilterLeaf("state", "eq", state));
        return this;
    }

    public CumListFilterBuilder ByStationCode(string code)
    {
        _root.Add(new FilterLeaf("stationCode", "eq", code));
        return this;
    }

    public CumListFilterBuilder ByPayer(string payer)
    {
        _root.Add(new FilterLeaf("payerName", "icontains", payer));
        return this;
    }

    public CumListFilterBuilder ChargeCodes(params string[] codes)
    {
        _root.Add(new FilterLeaf("chargeCode", "in", codes));
        return this;
    }

    public CumListFilterBuilder AddGroup(LogicalOperator logicalOperator, Action<CumListFilterBuilder> configure)
    {
        var nestedBuilder = new CumListFilterBuilder(logicalOperator);
        configure(nestedBuilder);
        _root.Add(nestedBuilder.BuildNode());
        return this;
    }

    public object Build() => _root.ToGraphQl();

    internal FilterGroup BuildNode() => _root;
}
