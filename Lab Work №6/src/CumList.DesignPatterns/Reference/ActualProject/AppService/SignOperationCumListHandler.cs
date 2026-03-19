using CumList.AppService.Constants;
using CumList.AppService.Database;
using CumList.AppService.Handlers.Core;
using CumList.AppService.Models;
using CumList.AppService.Models.Inputs;
using CumList.AppService.Types;
using Normalize.DocOperNtsCore;
using NTS.Entity.Operations;
using NTS.EtranGateway.Library.KafkaModel.Models;

namespace CumList.AppService.Handlers;

internal sealed class SignOperationCumListHandler(
    INormalizedDocTypeFactory docTypeFactory,
    INormalizeDocOperNtsFactory docOperNtsFactory
) : BaseOperationCumListHandler<SignDocumentInput>(docTypeFactory, docOperNtsFactory)
{
    #region private
    private static readonly HashSet<long> AvailableStateIds = [
        302 //На подписи
    ];
    #endregion
    #region protected
    protected override IntegrationModuleRequestModel CreateIntergrationModuleRequest(
        EntityIdOperationWith<SignDocumentInput, long, CumListOperationType> model
    )
    {
        return new IntegrationModuleRequestModel
        {
            DocId = model.EntityId.ToString(),
            CorrelationId = model.CorrelationId.ToString(),
            MessageType = IntergrationModuleMessageType.SetCumListAgreement,
            Message = "{\"action\":1 }"
        };
    }
    #endregion
    public override async Task HandleAsync(
        EntityIdOperationWith<SignDocumentInput, long, CumListOperationType> model,
        DatabaseContext dbContext,
        CancellationToken cancellationToken = default
    )
    {
        var docId = model.EntityId;

        await IsAvailableOperationAsync(dbContext, docId, AvailableStateIds, cancellationToken);

        await LastOperationWithoutErrorAsync(dbContext, docId, cancellationToken);

        await DocOperNtsFactory.AddAndSaveOperNtsForDocumentAsync(
            dbContext,
            docId,
            AvailableDocOperResultNts.Sign.Wait,
            model.UserId,
            model.CorrelationId.ToString(),
            cancellationToken
        );
    }
}
