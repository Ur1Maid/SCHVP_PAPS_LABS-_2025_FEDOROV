using CumList.AppService.Constants;
using CumList.AppService.Database;
using CumList.AppService.Kafka.Models;
using CumList.AppService.Models;
using CumList.AppService.Models.Inputs;
using CumList.AppService.Types;
using Microsoft.EntityFrameworkCore;
using Normalize.DocOperNtsCore;
using Normalize.DocOperNtsCore.Database.Models;
using NTS.Entity.Exceptions;
using NTS.Entity.Notifications;
using NTS.Entity.Operations;
using NTS.EtranGateway.Library.KafkaModel.Models;
using NTS.Kafka.Consumers.ModelHandlers;

namespace CumList.AppService.Handlers.Core;

internal abstract class BaseOperationCumListHandler<TInput> : IModelTargetHandler<
    EntityIdOperationWith<TInput, long, CumListOperationType>,
    DatabaseContext,
    DocumentOperationTargetModel,
    Exception?
> where TInput : BaseDocumentInput
{
    #region protected
    protected INormalizedDocTypeFactory DocTypeFactory { get; }
    protected INormalizeDocOperNtsFactory DocOperNtsFactory { get; }

    protected virtual IntegrationModuleRequestModel? CreateIntergrationModuleRequest(
        EntityIdOperationWith<TInput, long, CumListOperationType> model
    )
    {
        return null;
    }

    protected static async Task IsAvailableOperationAsync(
        DatabaseContext context,
        long docId,
        HashSet<long>? availableStateIds,
        CancellationToken cancellationToken
    )
    {
        var document = await context
            .Documents
            .AsNoTracking()
            .Where(x => x.Id == docId)
            .Select(x => new Document { StateId = x.StateId })
            .FirstOrDefaultAsync(cancellationToken);

        if (document == null)
        {
            EntityException.Throw(DocumentOperationErrorCode.NotFound,
                $"The cumlist with id '{docId}' is not found."
            );
        }

        if (availableStateIds != null)
        {
            var documentStateId = document.StateId;
            if (documentStateId == null || !availableStateIds.Contains(documentStateId.Value))
            {
                EntityException.Throw(DocumentOperationErrorCode.NotAvilable,
                    $"The cumlist with id '{docId}' in state '{documentStateId}' is not available to perform the operation."
                );
            }
        }
    }

    protected async Task LastOperationWithoutErrorAsync(
        DatabaseContext context,
        long docId,
        CancellationToken cancellationToken
    )
    {
        var lastDocOperNts = await DocOperNtsFactory.GetLastDocOperNtsForDocumentAsync(
            context,
            docId,
            cancellationToken
        );

        var resultId = lastDocOperNts?.ResultId;
        if (resultId == null)
            return;

        var isErrorAvailable = AvailableDocOperResultNts.Errors.Contains(resultId.Value);
        if (!isErrorAvailable)
        {
            EntityException.Throw(DocumentOperationErrorCode.NotAvilable,
                $"Operation with cumlist '{docId}' is not available."
            );
        }
    }
    #endregion
    protected BaseOperationCumListHandler(
        INormalizedDocTypeFactory docTypeFactory,
        INormalizeDocOperNtsFactory docOperNtsFactory
    )
    {
        ArgumentNullException.ThrowIfNull(docTypeFactory);
        ArgumentNullException.ThrowIfNull(docOperNtsFactory);

        DocTypeFactory = docTypeFactory;
        DocOperNtsFactory = docOperNtsFactory;
    }

    public abstract Task HandleAsync(
        EntityIdOperationWith<TInput, long, CumListOperationType> model,
        DatabaseContext dbContext,
        CancellationToken cancellationToken = default
    );

    public DocumentOperationTargetModel CreateTarget(
        EntityIdOperationWith<TInput, long, CumListOperationType> model,
        Exception? exception
    )
    {
        var entityType = DocTypeFactory.GetName(AvailableDocType.CumList);

        var targetModel = new DocumentOperationTargetModel
        {
            Notification = new EntityIdOperationNotifications<DocumentOperationErrorCode, long, CumListOperationType>(
                model.CorrelationId,
                resultNotification: new EntityIdOperationResultNotification<DocumentOperationErrorCode, long, CumListOperationType>(
                    entityType: entityType,
                    model,
                    DocumentOperationErrorCode.Exception,
                    exception
                ),
                successNotification: exception == null
                    ? new EntityIdOperationSuccessNotification<long, CumListOperationType>(
                        entityType: entityType,
                        model.EntityId,
                        CumListOperationType.Update)
                    : null
            )
        };

        if (exception == null)
            targetModel.IntegrationModuleRequest = CreateIntergrationModuleRequest(model);

        return targetModel;
    }
}
