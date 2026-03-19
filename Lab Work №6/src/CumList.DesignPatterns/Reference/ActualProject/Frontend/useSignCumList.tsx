import { useSignCumListMutation } from 'graphql/generated';
import { useMemo } from 'react';

import { UUID } from 'shared_ui/utils';

import { useAllCumListOperationsSubscription } from './useAllCumListOperationsSubscription';
import { useCumListSubscription } from './useCumListSubscription';

type signParams = {
  onCompleted?: VoidFunction;
  onError?: (err?: unknown) => void;
};

export const useSignCumList = ({ onCompleted, onError }: signParams) => {
  const correlationId = useMemo(() => UUID(), []);

  const [signCumLists, { loading: mutLoading }] = useSignCumListMutation({
    fetchPolicy: 'no-cache',
  });

  const { loading } = useCumListSubscription({
    correlationId,
    onCompleted,
    onError,
    mutLoading,
  });
  useAllCumListOperationsSubscription({ onError });

  const handleCumListSign = (docId: number) => {
    // TODO: Обраблтать ошибку
    void signCumLists({
      variables: {
        input: {
          correlationId,
          docId,
        },
      },
    });
  };
  return {
    loading,
    handleCumListSign,
  };
};
