using CumList.NormalizeService.Database.Constants;
using Microsoft.EntityFrameworkCore.Migrations;
using Normalize.DocCore.Database.Constants;

namespace CumList.NormalizeService.Database;

internal static class ViewFactory
{
    #region private

    private const string CreateCumListView =
        $"""
         CREATE OR REPLACE VIEW {NormalizeDocSchemaName.Default}."{NormalizeCumListViewName.CumListView}"
         AS
         SELECT cl."{NormalizeCumListColumnName.CumList.DocId}" as "{NormalizeCumListColumnName.CumListView.DocId}",
         cl."{NormalizeCumListColumnName.CumList.MainId}" as "{NormalizeCumListColumnName.CumListView.MainId}",
         cl."{NormalizeCumListColumnName.CumList.Number}" as "{NormalizeCumListColumnName.CumListView.Number}",
         cl."{NormalizeCumListColumnName.CumList.CreateDate}" as "{NormalizeCumListColumnName.CumListView.CreateDate}",
         cl."{NormalizeCumListColumnName.CumList.StartDate}" as "{NormalizeCumListColumnName.CumListView.StartDate}",
         cl."{NormalizeCumListColumnName.CumList.FinishDate}" as "{NormalizeCumListColumnName.CumListView.FinishDate}",
         cl."{NormalizeCumListColumnName.CumList.Person}" as "{NormalizeCumListColumnName.CumListView.Person}",
         cl."{NormalizeCumListColumnName.CumList.Contractor}" as "{NormalizeCumListColumnName.CumListView.Contractor}",
         cl."{NormalizeCumListColumnName.CumList.Discord}" as "{NormalizeCumListColumnName.CumListView.Discord}",
         cl."{NormalizeCumListColumnName.CumList.ArbSign}" AS "{NormalizeCumListColumnName.CumListView.ArbSign}",
         cl."{NormalizeCumListColumnName.CumList.ArbNum}" AS "{NormalizeCumListColumnName.CumListView.ArbNum}",
         cl."{NormalizeCumListColumnName.CumList.ClientId}" as "{NormalizeCumListColumnName.CumListView.ClientId}",
         cl."{NormalizeCumListColumnName.CumList.PayerId}" as "{NormalizeCumListColumnName.CumListView.PayerId}",
         cl."{NormalizeCumListColumnName.CumList.OrgId}" as "{NormalizeCumListColumnName.CumListView.OrgId}",
         cl."{NormalizeCumListColumnName.CumList.TypeId}" as "{NormalizeCumListColumnName.CumListView.TypeId}",
         cl."{NormalizeCumListColumnName.CumList.StationId}" as "{NormalizeCumListColumnName.CumListView.StationId}",
         cl."{NormalizeCumListColumnName.CumList.PayFormId}" as "{NormalizeCumListColumnName.CumListView.PayFormId}",
         cl."{NormalizeCumListColumnName.CumList.PayPlaceId}" as "{NormalizeCumListColumnName.CumListView.PayPlaceId}",

         min(due."{NormalizeCumListColumnName.CumListDue.Id}"::text)::uuid as "{NormalizeCumListColumnName.CumListView.MinDueId}",
         sum(due."{NormalizeCumListColumnName.CumListDue.Amount}") as "{NormalizeCumListColumnName.CumListView.AmountSum}",
         sum(due."{NormalizeCumListColumnName.CumListDue.Amount}") + sum(due."{NormalizeCumListColumnName.CumListDue.TaxValue}") as "{NormalizeCumListColumnName.CumListView.AmountTotal}",
         sum(due."{NormalizeCumListColumnName.CumListDue.TaxValue}") as "{NormalizeCumListColumnName.CumListView.TaxValueSum}",
         sum(due."{NormalizeCumListColumnName.CumListDue.KzAmount}") as "{NormalizeCumListColumnName.CumListView.KzAmountSum}",
         sum(due."{NormalizeCumListColumnName.CumListDue.KzAmount}") + sum(due."{NormalizeCumListColumnName.CumListDue.KzTaxValue}") as "{NormalizeCumListColumnName.CumListView.KzAmountTotal}",
         sum(due."{NormalizeCumListColumnName.CumListDue.KzTaxValue}") as "{NormalizeCumListColumnName.CumListView.KzTaxValueSum}",
         count(due."{NormalizeCumListColumnName.CumListDue.DocId}") AS "{NormalizeCumListColumnName.CumListView.DueCount}",

         max(doc."{NormalizeDocColumnName.Document.DocTypeId}") AS "{NormalizeCumListColumnName.CumListView.DocTypeId}",
         max(doc."{NormalizeDocColumnName.Document.StateId}") AS "{NormalizeCumListColumnName.CumListView.StateId}"

         FROM {NormalizeDocSchemaName.Default}."{NormalizeCumListTableName.CumList}" cl
         LEFT JOIN {NormalizeDocSchemaName.Default}."{NormalizeCumListTableName.CumListDue}" due on cl."{NormalizeCumListColumnName.CumList.DocId}" = due."{NormalizeCumListColumnName.CumListDue.DocId}"
         JOIN {NormalizeDocSchemaName.Default}."{NormalizeDocTableName.Document}" doc on cl."{NormalizeCumListColumnName.CumList.DocId}" = doc."{NormalizeDocColumnName.Document.Id}"
         GROUP BY cl."{NormalizeCumListColumnName.CumList.DocId}";
         """;

    #endregion

    public static void CreateOrReplace(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(CreateCumListView);
    }
}
