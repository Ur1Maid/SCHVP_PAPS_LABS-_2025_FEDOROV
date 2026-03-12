import { CumListsQuery, useCumListsLazyQuery } from 'graphql/generated';
import { useContext, useEffect, useMemo, useState } from 'react';

import { useApolloLazyQueryPage } from 'core_client/apollo';

import { DataService } from 'shared_ui/components';

import { mapToRow, TransformedRow } from '../model/columnSelectors';

const initialParams = {
  rows: 100,
  requestDate: new Date().toISOString(),
};
export const useCumListPage = () => {
  const [tableParams] = useState(initialParams);
  const {
    tableColumns: cumListsColumnsMetaData,
    filtersWhereGraphQLStatement,
    tableColumnsGroups,
  } = useContext(DataService.DataServiceContext);
  const hasFilters =
    filtersWhereGraphQLStatement &&
    Object.keys(filtersWhereGraphQLStatement).length > 0;

  const extractData = (
    data: CumListsQuery,
  ): CumListsQuery['cumlistCumLists'] => {
    return data.cumlistCumLists;
  };

  const [getCumLists] = useCumListsLazyQuery();

  const { loading, load, items } = useApolloLazyQueryPage({
    pageSize: 100,
    total: 1000,
    extractData,
    lazyQuery: getCumLists,
  });

  const transformedData = useMemo<TransformedRow[]>(
    () => (items ?? []).map(mapToRow),
    [items],
  );

  useEffect(() => {
    if (!hasFilters) return;
    load({
      variables: {
        where: filtersWhereGraphQLStatement,
        requestDate: new Date().toISOString(),
      },
    });
  }, [filtersWhereGraphQLStatement]);

  const totalCount = items.length;
  return {
    cumListsColumnsMetaData,
    transformedData,
    loading,
    rows: tableParams.rows,
    totalCount,
    hasFilters,
    tableColumnsGroups,
  };
};
