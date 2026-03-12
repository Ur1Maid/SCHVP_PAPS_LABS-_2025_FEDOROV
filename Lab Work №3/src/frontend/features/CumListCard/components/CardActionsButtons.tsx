import { Button } from 'shared_ui/components';

interface IProps {
  className?: string;
  onBack: VoidFunction;
  rejectCumList: VoidFunction;
  signCumList: VoidFunction;
  operButtonsDisabled?: boolean;
}

export const CardActionsButtons = ({
  onBack,
  rejectCumList,
  signCumList,
  operButtonsDisabled,
}: IProps) => {
  return (
    <div className="flex gap-2">
      <Button size="medium" variant="secondary" onClick={onBack}>
        Закрыть
      </Button>
      <Button
        size="medium"
        variant="danger"
        onClick={rejectCumList}
        disabled={operButtonsDisabled}
      >
        Отклонить
      </Button>
      <Button
        size="medium"
        onClick={signCumList}
        disabled={operButtonsDisabled}
      >
        Подписать
      </Button>
    </div>
  );
};
