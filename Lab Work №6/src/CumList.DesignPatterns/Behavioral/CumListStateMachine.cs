using System;
using CumList.DesignPatterns.Domain;

namespace CumList.DesignPatterns.Behavioral;

public interface ICumListState
{
    string Name { get; }
    bool CanSign { get; }
    bool CanReject { get; }
    ICumListState Submit();
    ICumListState OnSuccess();
    ICumListState OnError();
}

public sealed class AwaitingSignatureState : ICumListState
{
    public string Name => "На подписи";
    public bool CanSign => true;
    public bool CanReject => true;
    public ICumListState Submit() => new OperationInProgressState(this);
    public ICumListState OnSuccess() => this;
    public ICumListState OnError() => this;
}

public sealed class OperationInProgressState(ICumListState previousState) : ICumListState
{
    public string Name => "Выполнение операции";
    public bool CanSign => false;
    public bool CanReject => false;
    public ICumListState Submit() => this;
    public ICumListState OnSuccess() => new SignedState();
    public ICumListState OnError() => previousState;
}

public sealed class SignedState : ICumListState
{
    public string Name => "Подписан";
    public bool CanSign => false;
    public bool CanReject => false;
    public ICumListState Submit() => this;
    public ICumListState OnSuccess() => this;
    public ICumListState OnError() => this;
}

public sealed class RejectedState : ICumListState
{
    public string Name => "Отклонён";
    public bool CanSign => false;
    public bool CanReject => false;
    public ICumListState Submit() => this;
    public ICumListState OnSuccess() => this;
    public ICumListState OnError() => this;
}

public sealed class CumListStateContext(ICumListState state)
{
    public ICumListState State { get; private set; } = state;

    public void Submit() => State = State.Submit();
    public void CompleteSuccessfully() => State = State.OnSuccess();
    public void CompleteWithError() => State = State.OnError();
}
