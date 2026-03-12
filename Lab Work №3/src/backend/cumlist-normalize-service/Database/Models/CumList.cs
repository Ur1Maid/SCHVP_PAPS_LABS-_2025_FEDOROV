namespace CumList.NormalizeService.Database.Models;

internal sealed class CumList
{
    public long DocId { get; set; }

    public long? MainId { get; set; }

    public string? Number { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? FinishDate { get; set; }

    public string? Person { get; set; }

    public string? Contractor { get; set; }

    public string? Discord { get; set; }

    public bool? ArbSign { get; set; }

    public string? ArbNum { get; set; }

    public long? ClientId { get; set; }

    public long? PayerId { get; set; }

    public long? OrgId { get; set; }

    public long? TypeId { get; set; }

    public long? PayFormId { get; set; }

    public long? PayPlaceId { get; set; }

    public long? StationId { get; set; }
}
