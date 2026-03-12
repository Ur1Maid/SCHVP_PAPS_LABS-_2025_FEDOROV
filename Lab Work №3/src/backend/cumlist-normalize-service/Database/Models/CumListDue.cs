namespace CumList.NormalizeService.Database.Models;

internal sealed class CumListDue
{
    public Guid Id { get; set; }

    public DateTime? DueDate { get; set; }

    public string? ParentDocNum { get; set; }

    public decimal? Amount { get; set; }

    public long? TaxableId { get; set; }

    public decimal? TaxValue { get; set; }

    public decimal? KzAmount { get; set; }

    public long? KzTaxableId { get; set; }

    public decimal? KzTaxValue { get; set; }

    public string? Info { get; set; }

    public string? Note { get; set; }

    public bool? ExistSign { get; set; }

    public long? AgMpsOrgId { get; set; }

    public long? ParentDocId { get; set; }

    public long DocId { get; set; }

    public long? DueTypeId { get; set; }

    public long? ParentDocTypeId { get; set; }
}
