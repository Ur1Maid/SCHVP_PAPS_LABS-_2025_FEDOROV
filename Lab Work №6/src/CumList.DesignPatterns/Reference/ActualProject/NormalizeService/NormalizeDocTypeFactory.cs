using CumList.NormalizeService.Configurations;
using Normalize.DocCore.Types;

namespace CumList.NormalizeService.Types;

internal sealed class NormalizeDocTypeFactory(KafkaConfiguration kafkaConfiguration) :
    NormalizeDocTypeBaseFactory,
    INormalizeDocTypeFactory
{
    public INormalizeDocType GetCumListType { get; } = new NormalizeDocType(
        docTypeId: 27, //Тип документа нормализации
        normalizeSystem: kafkaConfiguration.GroupId,
        subTypeIds:
        [
            2 //Накопительная ведомость
        ]
    );
}
