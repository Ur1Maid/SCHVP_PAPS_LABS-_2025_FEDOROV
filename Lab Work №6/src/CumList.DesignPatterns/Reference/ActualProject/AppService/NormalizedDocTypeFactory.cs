using CumList.AppService.Constants;
using Normalize.DocOperNtsCore.Types;
using NTS.GraphQL.Conventions;

namespace CumList.AppService.Types;

internal sealed class NormalizedDocTypeFactory :
    NormalizedDocTypeBaseFactory,
    INormalizedDocTypeFactory
{
    public NormalizedDocTypeFactory(IGraphQLNamingFactory nameFactory)
    {
        AddDocTypeId(AvailableDocType.CumList, nameFactory.CreateName("CumList"));
    }
}
