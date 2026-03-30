using CumList.NormalizeService.Database.Models;
using CumList.NormalizeService.GraphQL.Responses;
using Normalize.DocCore.Converters;
using Normalize.DocCore.Mappers;

namespace CumList.NormalizeService.Mappers;

internal sealed class MapperFactory(INormalizeDocIdConverter idConverter) :
    NormalizeDocMapperFactory(idConverter),
    IMapperFactory
{
    #region private
    private static bool? AsBool(long? value)
    {
        if (value.HasValue)
            return value == 1;

        return null;
    }
    #endregion

    public Database.Models.CumList CreateCumList(ImpCumList impCumList)
    {
        return new Database.Models.CumList
        {
            DocId = IdConverter.AddPrefixTo(impCumList.CumId),
            MainId = IdConverter.AddPrefixTo(impCumList.CumMainId),
            Number = impCumList.CumNumber,
            CreateDate = impCumList.CumDateCreate,
            StartDate = impCumList.CumStartDate,
            FinishDate = impCumList.CumFinishDate,
            Person = impCumList.CumPersonRzd,
            Contractor = impCumList.CumContractor,
            Discord = impCumList.CumDiscord,
            ArbSign = AsBool(impCumList.CumArbSign),
            ArbNum = impCumList.CumArbNum,
            ClientId = impCumList.CumClientId,
            PayerId = impCumList.CumPayerId,
            OrgId = impCumList.CumRzdOrgId,
            TypeId = impCumList.CumTypeId,
            PayFormId = impCumList.CumPayFormId,
            PayPlaceId = impCumList.CumPayPlaceId,
            StationId = impCumList.CumStationId
        };
    }

    public CumListDue CreateCumListDue(ImpCumListDue due)
    {
        return new CumListDue
        {
            Id = due.Id,
            DueDate = due.DueDate,
            ParentDocNum = due.DueParentDocId == null ? due.DueParentDocNum : null,
            Amount = due.DueSum,
            TaxableId = due.DueTaxAble,
            TaxValue = due.DueSumNds,
            KzAmount = due.DueSumKz,
            KzTaxableId = due.DueTaxAbleKz,
            KzTaxValue = due.DueSumNdsKz,
            Info = due.DueInfo,
            Note = due.DueNote,
            ExistSign = AsBool(due.DueExistSign),
            AgMpsOrgId = due.AgMpsOrgId,
            ParentDocId = IdConverter.AddPrefixTo(due.DueParentDocId),
            DocId = IdConverter.AddPrefixTo(due.CumId),
            DueTypeId = due.DueId,
            ParentDocTypeId = due.DueParentDocNameId
        };
    }

}
