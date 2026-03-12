import { CumlistCumList } from 'graphql/generated';
import { useState } from 'react';

import { Toast } from 'shared_ui/primereact';

import {
  FiltersPlaceholder,
  ModalRejectionReason,
  TableFilterProvider,
  TableTemplate,
} from 'shared/components';
import { useCumList, useOperationCallbacks } from 'shared/hooks';

import { cumListsColumnsMetaConfig } from '../constants';
import { useCumListPage, useCumListPageFilters } from '../hooks';
import { CumListsTableHeader } from './CumListsTableHeader';

type CumListsTableData = CumlistCumList;

interface IProps {
  onOpenFilters: VoidFunction;
}

const CumListsPageComponent = ({ onOpenFilters }: IProps) => {
  const { onGlobalFilterChange, filters, onFilterChange } =
    useCumListPageFilters();

  const {
    transformedData,
    cumListsColumnsMetaData,
    loading,
    rows,
    totalCount,
    hasFilters,
    tableColumnsGroups,
  } = useCumListPage();

  const { toastRef, onCompleted, onError } = useOperationCallbacks();

  const {
    selectedRows,
    operButtonsDisabled,
    rejectModalVisible,
    onSelectionChange,
    signCumLists,
    onModalClose,
    onModalOpen,
    onRowDoubleClick,
    signLoading,
  } = useCumList({
    onCompleted,
    onError,
  });

  const docId = Number(selectedRows[0]?.docId ?? 0);

  if (signLoading) return <div>Загрузка...</div>;
  return (
    <>
      <Toast ref={toastRef} />
      {hasFilters ? (
        <div className="flex flex-column gap-2">
          <CumListsTableHeader
            signCumLists={() => signCumLists(docId)}
            rejectCumLists={onModalOpen}
            operButtonsDisabled={operButtonsDisabled}
            onSearch={onGlobalFilterChange}
            tableMetaData={cumListsColumnsMetaData}
            value={transformedData}
          />
          <TableTemplate<CumListsTableData>
            dataKey="docId"
            value={transformedData}
            totalCount={totalCount}
            tableMetaData={cumListsColumnsMetaData}
            columnGroup={
              typeof tableColumnsGroups === 'function'
                ? tableColumnsGroups?.({ tableData: transformedData })
                : tableColumnsGroups
            }
            filters={filters}
            onFilter={onFilterChange}
            loading={loading}
            selection={selectedRows}
            onSelectionChange={onSelectionChange}
            rows={rows}
            onRowDoubleClick={onRowDoubleClick}
            hasCheckBox
          />
        </div>
      ) : (
        <FiltersPlaceholder onOpenFilters={onOpenFilters} />
      )}
      <ModalRejectionReason
        visible={rejectModalVisible}
        onClose={onModalClose}
        docIds={selectedRows.flatMap(item =>
          typeof item.docId === 'number' ? [item.docId] : [],
        )} //TODO: change to multi operation
        onCompleted={() => {
          onCompleted();
          onModalClose();
        }}
        onError={onError}
      />
    </>
  );
};

export const CumListsPage = () => {
  const [activeTab, setActiveTab] = useState<number | undefined>();
  const openFilters = () => {
    setActiveTab(undefined);
    queueMicrotask(() => setActiveTab(0));
  };
  return (
    <TableFilterProvider
      scope="cumlist"
      config={cumListsColumnsMetaConfig}
      active={activeTab}
    >
      <CumListsPageComponent onOpenFilters={openFilters} />
    </TableFilterProvider>
  );
};
