import { CumlistCumList } from 'graphql/generated';
import { useState } from 'react';
import { generatePath, useNavigate } from 'react-router-dom';

import {
  DataTableSelectionMultipleChangeEvent,
  DataTableValueArray,
} from 'shared_ui/components';
import { DataTableRowClickEvent } from 'shared_ui/primereact';

import { paths } from 'app/routers/paths';

import { VALID_STATE } from 'shared/constants/common';
import { useSignCumList } from 'shared/hooks';

type Props = {
  onCompleted: VoidFunction;
  onError: (err: unknown) => void;
};

export const useCumList = ({ onCompleted, onError }: Props) => {
  const [selectedRows, setSelectedRows] = useState<CumlistCumList[]>([]);
  const [rejectModalVisible, setRejectModalVisible] = useState(false);
  const navigate = useNavigate();

  const { handleCumListSign, loading } = useSignCumList({
    onCompleted,
    onError,
  });

  const signCumLists = (docIds: number) => {
    handleCumListSign(docIds); //TODO: change to multi oper
    setSelectedRows([]);
  };

  const onSelectionChange = (
    e: DataTableSelectionMultipleChangeEvent<DataTableValueArray>,
  ) => {
    const forSignature = e.value.filter(row => row.state === VALID_STATE);
    setSelectedRows(forSignature as CumlistCumList[]);
  };

  const onRowDoubleClick = (e: DataTableRowClickEvent) => {
    const row = e?.data as CumlistCumList;
    if (row?.docId) {
      navigate(
        generatePath(paths.docId, {
          docId: row.docId,
        }),
      );
    }
  };

  const handleModalClose = () => {
    setRejectModalVisible(false);
    setSelectedRows([]);
  };

  const handleModalOpen = () => {
    setRejectModalVisible(true);
  };

  return {
    selectedRows,
    operButtonsDisabled: selectedRows.length === 0,
    rejectModalVisible,
    setSelectedRows,
    onSelectionChange,
    signCumLists,
    onModalClose: handleModalClose,
    onModalOpen: handleModalOpen,
    onRowDoubleClick,
    signLoading: loading,
  };
};
