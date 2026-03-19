using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Structural;

public interface IIntegrationReplyAdapter
{
    InternalOperationResult Adapt(ExternalOperationReply reply, string previousState);
}

public sealed class EtranReplyAdapter : IIntegrationReplyAdapter
{
    public InternalOperationResult Adapt(ExternalOperationReply reply, string previousState)
    {
        if (reply.Status == "Accepted")
        {
            return new InternalOperationResult(true, reply.ExternalState ?? "Подписан", null, null);
        }

        if (reply.Status == "Rejected")
        {
            return new InternalOperationResult(false, previousState, "ETRAN_REJECTED", reply.ErrorText);
        }

        return new InternalOperationResult(false, previousState, "ETRAN_UNKNOWN", "Неизвестный ответ от внешней системы.");
    }
}
