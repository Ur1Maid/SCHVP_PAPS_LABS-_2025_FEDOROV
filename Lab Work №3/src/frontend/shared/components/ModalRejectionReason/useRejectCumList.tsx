import { useRejectCumListMutation } from 'graphql/generated';
import { useMemo } from 'react';

import { UUID } from 'shared_ui/utils';

import {
  useAllCumListOperationsSubscription,
  useCumListSubscription,
} from 'shared/hooks';

type rejectParams = {
  docIds: number[];
  discordId: number;
  discordText: string;
  onCompleted?: VoidFunction;
  onError?: (err?: unknown) => void;
};

export const useRejectCumList = ({
  docIds,
  discordId,
  discordText,
  onCompleted,
  onError,
}: rejectParams) => {
  const correlationId = useMemo(() => UUID(), []);

  const [rejectCumList, { loading: mutLoading }] = useRejectCumListMutation({
    fetchPolicy: 'no-cache',
  });

  const { loading } = useCumListSubscription({
    correlationId,
    onCompleted,
    onError,
    mutLoading,
  });

  useAllCumListOperationsSubscription({ onError });

  const handleReject = () => {
    // TODO: Обраблтать ошибку
    rejectCumList({
      variables: {
        input: {
          correlationId,
          docId: docIds[0],
          discordId,
          discordText,
        },
      },
    });
  };
  return {
    loading,
    handleReject,
  };
};
