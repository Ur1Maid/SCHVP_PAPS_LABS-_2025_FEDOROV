import {
  CumCardLayoutDocument,
  CumListsDocument,
  HistoryByCumListIdDocument,
  HistoryNtsByCumListIdDocument,
  useEntityOperationsSubscription,
} from 'graphql/generated';

import { useApolloClient } from '@apollo/client';

type subscriptionParams = {
  onError?: (err?: unknown) => void;
};

export const useAllCumListOperationsSubscription = ({
  onError,
}: subscriptionParams) => {
  const client = useApolloClient();

  useEntityOperationsSubscription({
    onData: () => {
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
};
