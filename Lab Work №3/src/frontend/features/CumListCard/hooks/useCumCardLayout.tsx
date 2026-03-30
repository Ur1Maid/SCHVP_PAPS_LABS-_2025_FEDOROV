import { useCumCardLayoutQuery } from 'graphql/generated';
import { useParams } from 'react-router-dom';

import { DocIdType } from 'shared/types';

export const useCumCardLayout = () => {
  const { docId } = useParams<DocIdType>();
  const { data, refetch } = useCumCardLayoutQuery({
    variables: { docId: Number(docId) },
  });
  return {
    docId,
    state: data?.cumlistCumListByDocId?.document?.state?.name ?? '',
    isWaiting: data?.cumlistCumListByDocId?.operationsNts.at(-1)?.isWaiting,
    refetch,
  };
};
