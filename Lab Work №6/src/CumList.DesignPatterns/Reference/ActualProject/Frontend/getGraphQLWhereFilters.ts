import {
  FilterDataType,
  IFilter,
  IFilterNestedLookup,
  IGroup,
} from '../contexts/DataServiceFilterContext';
import { LogicalCondition } from '../types';
import { ConditionOperand } from '../hooks/useCondition';

interface IOptions {
  groupsConnectonCondition: LogicalCondition;
  groups: IGroup[];
  filters: IFilter[];
}

interface IFilterValueMapperByType {
  (filter: IFilter): IFilter['value'];
}

const mapLogicalConditionToGQL: Record<LogicalCondition, string> = {
  [LogicalCondition.AND]: 'and',
  [LogicalCondition.OR]: 'or',
};

const getFilterValueString: IFilterValueMapperByType = function ({
  value,
  useInputFromBuffer,
}) {
  if (useInputFromBuffer === true) {
    return Array.isArray(value) ? value : [String(value)];
  }
  return String(value);
};

const getFilterValueNumber: IFilterValueMapperByType = function ({
  value,
  useInputFromBuffer,
}) {
  if (useInputFromBuffer === true) {
    return Array.isArray(value) ? value : [Number(value)];
  }
  return Number(value);
};

const getFilterValueBoolean: IFilterValueMapperByType = function ({ value }) {
  return String(value === true ? 1 : 0);
};

const FilterValueByType: Partial<
  Record<FilterDataType, IFilterValueMapperByType>
> = {
  [FilterDataType.STRING]: getFilterValueString,
  [FilterDataType.NUMBER]: getFilterValueNumber,
  [FilterDataType.BOOLEAN]: getFilterValueBoolean,
};

const getFilterValue: IFilterValueMapperByType = function (filter) {
  const { value, dataType, lookup } = filter;
  const dataTypeSource = (
    lookup?.dataType || dataType
  ).toUpperCase() as FilterDataType;
  const filterValueByTypeFunction = FilterValueByType[dataTypeSource];

  if (filterValueByTypeFunction) {
    return filterValueByTypeFunction(filter);
  }

  return value;
};

function getFilterCondition(filter: IFilter): IFilter['condition'] {
  if (filter.useInputFromBuffer) {
    if (filter.dataType === FilterDataType.NUMBER) {
      return ConditionOperand.IN;
    }
  }

  // отключено в рамках задачи TZAD-916
  // if (filter.lookup) {
  //   return ConditionOperand.EQ;
  // }

  return filter.condition;
}

function getFilterNestedLookup(filter: IFilter): object {
  const condition = getFilterCondition(filter);
  const value = getFilterValue(filter);
  const useNull = filter.nullable && filter.condition === ConditionOperand.NULL;
  const endFilter = useNull
    ? { [ConditionOperand.EQ]: null }
    : { [condition]: value };

  const key = filter?.lookup?.fieldName ?? filter.fieldName;

  if (!filter.lookup?.nested) {
    if (filter.lookup) {
      return { [key]: endFilter };
    }
    return {
      [filter.fieldName]: endFilter,
    };
  }

  const mapNested = function (
    accumulator: Pick<IFilterNestedLookup, 'condition' | 'fieldName'>[],
    nested?: IFilterNestedLookup,
  ): any {
    if (!nested) {
      return accumulator.reverse().reduce((acc, curr, index) => {
        const data = { [curr.fieldName]: index ? acc : endFilter };

        if (!curr.condition) {
          return data;
        }

        return {
          [curr.condition]: data,
        };
      }, {});
    }

    return mapNested(
      [
        ...accumulator,
        { condition: nested.condition, fieldName: nested.fieldName },
      ],
      nested.nested,
    );
  };

  return {
    [filter.lookup.fieldName || filter.fieldName]: mapNested(
      [],
      filter.lookup.nested,
    ),
  };
}

function getGraphQLGroupStatement(group: IGroup, filters: IFilter[]): object {
  return {
    [mapLogicalConditionToGQL[group.connectonCondition]]: filters
      .filter(filter => filter.groupId === group.id)
      .map(getFilterNestedLookup),
  };
}

export function getGraphQLWhereFilters({
  groupsConnectonCondition,
  groups,
  filters,
}: IOptions): object {
  if (!filters.length) return {};

  if (groups.length === 1) {
    return getGraphQLGroupStatement(groups[0], filters);
  }

  return {
    [mapLogicalConditionToGQL[groupsConnectonCondition]]: groups.map(group =>
      getGraphQLGroupStatement(group, filters),
    ),
  };
}
