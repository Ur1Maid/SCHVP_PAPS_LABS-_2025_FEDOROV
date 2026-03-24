using CumList.NormalizeService.Constants;
using Normalize.DocCore.GraphQL.Requests;
using NTS.GraphQL.Client;

namespace CumList.NormalizeService.GraphQL.Requests;

internal sealed class CumListQueryFactory : NormalizeDocQueryFactory, ICumListQueryFactory
{
    public string GetImpСumListByCumIdFromList { get; } =
        new(
            $$"""
              query impCumLists (
                ${{JsonPropertyName.ImpСumList.CumId}}: Long) {
                impCumLists (
                  where: { 
                    {{JsonPropertyName.ImpСumList.CumId}}: { eq: ${{JsonPropertyName.ImpСumList.CumId}} }
                  }
                  {{JsonPropertyNamePagination.Take}}: 1
                ) {
                  items {
                    {{JsonPropertyName.ImpСumList.CumId}}
                    {{JsonPropertyName.ImpСumList.CumTypeId}}
                    {{JsonPropertyName.ImpСumList.CumParentId}}
                    {{JsonPropertyName.ImpСumList.CumMainId}}
                    {{JsonPropertyName.ImpСumList.CumNumber}}
                    {{JsonPropertyName.ImpСumList.CumRzdOrgId}}
                    {{JsonPropertyName.ImpСumList.CumRzdOrgName}}
                    {{JsonPropertyName.ImpСumList.CumRzdOrgCode}}
                    {{JsonPropertyName.ImpСumList.CumStationId}}
                    {{JsonPropertyName.ImpСumList.CumDateCreate}}
                    {{JsonPropertyName.ImpСumList.CumStartDate}}
                    {{JsonPropertyName.ImpСumList.CumFinishDate}}
                    {{JsonPropertyName.ImpСumList.CumPayerId}}
                    {{JsonPropertyName.ImpСumList.CumPayPlaceId}}
                    {{JsonPropertyName.ImpСumList.CumPayFormId}}
                    {{JsonPropertyName.ImpСumList.CumPersonRzd}}
                    {{JsonPropertyName.ImpСumList.CumContractor}}
                    {{JsonPropertyName.ImpСumList.CumClientId}}
                    {{JsonPropertyName.ImpСumList.CumDiscord}}
                    {{JsonPropertyName.ImpСumList.CumArbSign}}
                    {{JsonPropertyName.ImpСumList.CumArbNum}}
                  }
                }
              }
              """
        );

    public string GetImpСumListForCheckFromList { get; } =
        new(
            $$"""
              query impCumLists (
                ${{JsonPropertyName.ImpСumList.CumId}}: Long) {
                impCumLists (
                  where: { 
                    {{JsonPropertyName.ImpСumList.CumId}}: { eq: ${{JsonPropertyName.ImpСumList.CumId}} }
                  }
                  {{JsonPropertyNamePagination.Take}}: 1
                ) {
                  items {
                    {{JsonPropertyName.ImpСumList.CumId}}
                    {{JsonPropertyName.ImpСumList.CumTypeId}}
                  }
                }
              }
              """
        );

    public string GetImpCumListIdsByTypes { get; } =
        new(
            $$"""
              query impCumLists (
                ${{JsonPropertyName.ImpСumList.CumTypeId}}: [Long]!
                ${{JsonPropertyNamePagination.Skip}}: Int) {
                impCumLists (
                  where : {
                    {{JsonPropertyName.ImpСumList.CumTypeId}}: { in: ${{JsonPropertyName.ImpСumList.CumTypeId}} }
                  }
                  {{JsonPropertyNamePagination.Skip}}: ${{JsonPropertyNamePagination.Skip}}
                ) {
                  items {
                    {{JsonPropertyName.ImpСumList.CumId}}
                    {{JsonPropertyName.ImpСumList.CumTypeId}}
                  }
                }
              }
              """
        );

    public string GetImpСumListDuesByCumId { get; } =
        new(
            $$"""
              query impCumListDues (
                ${{JsonPropertyName.ImpСumListDue.CumId}}: Long
                ${{JsonPropertyNamePagination.Skip}}: Int) {
                impCumListDues (
                  where : {
                    {{JsonPropertyName.ImpСumListDue.CumId}}: { eq: ${{JsonPropertyName.ImpСumListDue.CumId}} } 
                  }
                  {{JsonPropertyNamePagination.Skip}}: ${{JsonPropertyNamePagination.Skip}}
                 ) {
                  items {
                    {{JsonPropertyName.ImpСumListDue.Id}}
                    {{JsonPropertyName.ImpСumListDue.CumId}}
                    {{JsonPropertyName.ImpСumListDue.DueDate}}
                    {{JsonPropertyName.ImpСumListDue.DueParentDocId}}
                    {{JsonPropertyName.ImpСumListDue.DueParentDocNameId}}
                    {{JsonPropertyName.ImpСumListDue.DueParentDocNum}}
                    {{JsonPropertyName.ImpСumListDue.DueId}}
                    {{JsonPropertyName.ImpСumListDue.DueSum}}
                    {{JsonPropertyName.ImpСumListDue.DueTaxAble}}
                    {{JsonPropertyName.ImpСumListDue.DueSumNds}}
                    {{JsonPropertyName.ImpСumListDue.DueSumKz}}
                    {{JsonPropertyName.ImpСumListDue.DueTaxAbleKz}}
                    {{JsonPropertyName.ImpСumListDue.DueSumNdsKz}}
                    {{JsonPropertyName.ImpСumListDue.DueInfo}}
                    {{JsonPropertyName.ImpСumListDue.DueNote}}
                    {{JsonPropertyName.ImpСumListDue.DueExistSign}}
                    {{JsonPropertyName.ImpСumListDue.AgMpsOrgId}}
                  }
                }
              }
              """
        );

    public string GetImpOrgPassportByIdOnRequestDateFromList { get; } =
        new(
            $$"""
              query impOrgPassports (
                ${{JsonPropertyName.ImpOrgPassport.Id}}: Long
                ${{JsonPropertyName.Request.RequestDate}}: DateTime) {
                impOrgPassports (
                  where: {
                    {{JsonPropertyName.ImpOrgPassport.Id}}: { eq: ${{JsonPropertyName.ImpOrgPassport.Id}} }
                  }
                  {{JsonPropertyName.Request.RequestDate}}: ${{JsonPropertyName.Request.RequestDate}}
                  {{JsonPropertyNamePagination.Take}}: 1
                ) {
                  items
                  {
                    {{JsonPropertyName.ImpOrgPassport.TypeName}}
                  }
                }
              }
              """
        );

    public string GetNsiStationByIdOnRequestDateFromList { get; } =
        new(
            $$"""
              query nsiStations (
                ${{JsonPropertyName.NsiStation.CodeOsjd}}: String
                ${{JsonPropertyName.NsiStation.Name}}: String 
                ${{JsonPropertyName.Request.RequestDate}}: DateTime) {
                nsiStations(
                  where: {
                    and: [
                      { {{JsonPropertyName.NsiStation.CodeOsjd}}: { eq: ${{JsonPropertyName.NsiStation.CodeOsjd}} } }
                      { {{JsonPropertyName.NsiStation.Name}}: { eq: ${{JsonPropertyName.NsiStation.Name}} } }
                    ]
                  }
                  {{JsonPropertyName.Request.RequestDate}}: ${{JsonPropertyName.Request.RequestDate}}
                  {{JsonPropertyNamePagination.Take}}: 1
                ) {
                  items
                  {
                    {{JsonPropertyName.NsiStation.Id}}
                  }
                }
              }
              """
        );
}
