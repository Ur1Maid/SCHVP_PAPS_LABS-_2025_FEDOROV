import {
  CumCardLayoutDocument,
  CumListsDocument,
  HistoryByCumListIdDocument,
  HistoryNtsByCumListIdDocument,
  useEntityOperationResultSubscription,
} from 'graphql/generated';

import { useApolloClient } from '@apollo/client';

type subscriptionParams = {
  correlationId: string;
  onCompleted?: VoidFunction;
  onError?: (err?: unknown) => void;
  mutLoading: boolean;
};

export const useCumListSubscription = ({
  correlationId,
  onCompleted,
  onError,
  mutLoading,
}: subscriptionParams) => {
  const client = useApolloClient();

  const { data, error } = useEntityOperationResultSubscription({
    variables: { correlationId },
    onData: () => {
      onCompleted?.();
      (async () => {
        client.refetchQueries({
          include: [
            HistoryByCumListIdDocument,
            HistoryNtsByCumListIdDocument,
            CumListsDocument,
            CumCardLayoutDocument,
          ],
        });
      })();
    },
    onError: error => {
      onError?.(error);
    },
  });
  return { loading: mutLoading && !!data && !!error };
};
