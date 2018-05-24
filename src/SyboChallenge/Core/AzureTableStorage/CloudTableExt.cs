using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyboChallenge.Core.AzureTableStorage
{
    public static class CloudTableExtensions
    {
        public static Task<IList<TEntity>> All<TEntity>(this CloudTable table, IList<string> selectColumns = null)
            where TEntity : TableEntity, new()
        {
            return Query(table, new TableQuery<TEntity>().Select(selectColumns));
        }

        public static async Task<TEntity> Find<TEntity>(this CloudTable table, string partitionKey, string rowKey, List<string> selectColumns = null)
            where TEntity : ITableEntity
        {
            if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentNullException(nameof(partitionKey));
            if (rowKey == null) throw new ArgumentNullException(nameof(rowKey));

            var result = await table.ExecuteAsync(TableOperation.Retrieve<TEntity>(partitionKey, rowKey, selectColumns));
            return (TEntity)result.Result;
        }

        public static Task<IList<TEntity>> QueryByPartitionKey<TEntity>(this CloudTable table, string partitionKey, IList<string> selectColumns = null)
            where TEntity : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentNullException(nameof(partitionKey));

            return QueryByField<TEntity>(table, nameof(ITableEntity.PartitionKey), partitionKey, selectColumns);
        }

        public static async Task<TEntity> QueryByPartitionKeySingleOrDefault<TEntity>(this CloudTable table, string partitionKey, IList<string> selectColumns = null)
            where TEntity : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentNullException(nameof(partitionKey));

            var result = await QueryByField<TEntity>(table, nameof(ITableEntity.PartitionKey), partitionKey, selectColumns);
            return result.SingleOrDefault();
        }

        public static Task<IList<TEntity>> QueryByRowKey<TEntity>(this CloudTable table, string rowKey, IList<string> selectColumns = null)
            where TEntity : ITableEntity, new()
        {
            return QueryByField<TEntity>(table, nameof(ITableEntity.RowKey), rowKey, selectColumns);
        }

        public static async Task<TEntity> QueryByRowKeySingleOrDefault<TEntity>(this CloudTable table, string rowKey, IList<string> selectColumns = null)
            where TEntity : ITableEntity, new()
        {
            var result = await QueryByField<TEntity>(table, nameof(ITableEntity.RowKey), rowKey, selectColumns);
            return result.SingleOrDefault();
        }

        public static Task<IList<TEntity>> QueryByPartitionKeyAndField<TEntity>(this CloudTable table, string partitionKey, string field, Guid value)
            where TEntity : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException(nameof(field));
            }

            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(ITableEntity.PartitionKey), QueryComparisons.Equal, partitionKey),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForGuid(field, QueryComparisons.Equal, value)
            );
            return Query(table, new TableQuery<TEntity>().Where(filter));
        }

        public static Task<IList<TEntity>> QueryByPartitionKeyAndField<TEntity>(this CloudTable table, string partitionKey, string field, string value)
            where TEntity : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException(nameof(field));
            }

            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(ITableEntity.PartitionKey), QueryComparisons.Equal, partitionKey),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(field, QueryComparisons.Equal, value)
            );
            return Query(table, new TableQuery<TEntity>().Where(filter));
        }

        public static Task<IList<TEntity>> QueryByField<TEntity>(this CloudTable table, string field, string value, IList<string> selectColumns = null)
            where TEntity : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException(nameof(field));
            }

            var filter = TableQuery.GenerateFilterCondition(field, QueryComparisons.Equal, value);
            return Query(table, new TableQuery<TEntity>().Where(filter).Select(selectColumns));
        }

        public static Task<IList<TEntity>> QueryByField<TEntity>(this CloudTable table, string field, Guid value)
            where TEntity : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException(nameof(field));
            }

            var filter = TableQuery.GenerateFilterConditionForGuid(field, QueryComparisons.Equal, value);
            return Query(table, new TableQuery<TEntity>().Where(filter));
        }

        public static Task<IList<TEntity>> Where<TEntity>(this CloudTable table, string filter)
            where TEntity : ITableEntity, new()
        {
            if (string.IsNullOrWhiteSpace(filter)) throw new ArgumentNullException(nameof(filter));
            return Query(table, new TableQuery<TEntity>().Where(filter));
        }

        public static async Task<IList<TEntity>> Query<TEntity>(this CloudTable table, TableQuery<TEntity> query)
            where TEntity : ITableEntity, new()
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            TableQuerySegment<TEntity> query_segment = null;
            var return_list = new List<TEntity>();
            while (query_segment == null || query_segment.ContinuationToken != null)
            {
                query_segment = await table.ExecuteQuerySegmentedAsync(query, query_segment?.ContinuationToken);
                return_list.AddRange(query_segment);
            }
            return return_list;
        }

        public static Task RemoveWithKeys(this CloudTable table, string partitionKey, string rowKey)
        {
            // A valid ETag is a precodnition of TableOperation.Delete.
            return table.ExecuteAsync(TableOperation.Delete(new DynamicTableEntity(partitionKey, rowKey) { ETag = "*" }));
        }

        public static Task RemoveWithPartitionKey(this CloudTable table, string partitionKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey)) throw new ArgumentNullException(nameof(partitionKey));
            return Remove(table, TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, partitionKey));
        }

        public static Task RemoveWithRowKey(this CloudTable table, string rowKey)
        {
            return Remove(table, TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, rowKey));
        }

        public static Task RemoveWithRowKeys(this CloudTable table, List<string> rowKeys)
        {
            if (rowKeys == null || !rowKeys.Any()) throw new ArgumentNullException(nameof(rowKeys));

            var filter = TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, rowKeys[0]);
            filter = rowKeys
                .Skip(1)
                .Aggregate(filter, (current, key) =>
                    TableQuery.CombineFilters(
                        current,
                        TableOperators.Or,
                        TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, key)));
            return Remove(table, filter);
        }

        public static Task Remove(this CloudTable table, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) throw new ArgumentNullException(nameof(filter));
            return Remove(table, new TableQuery().Where(filter));
        }

        public static async Task Remove(this CloudTable table, TableQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            TableQuerySegment query_segment = null;
            while (query_segment == null || query_segment.ContinuationToken != null)
            {
                query_segment = await table.ExecuteQuerySegmentedAsync(query, query_segment?.ContinuationToken);
                await Remove(table, query_segment.Results);
            }
        }

        public static async Task Remove<TEntity>(this CloudTable table, IList<TEntity> entities)
            where TEntity : ITableEntity, new()
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (!entities.Any())
            {
                return;
            }

            var batch = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batch.Add(TableOperation.Delete(entity));

                // Table batch can have up to 100 operations.
                if (batch.Count == 100)
                {
                    await table.ExecuteBatchAsync(batch);
                    batch.Clear();
                }
            }
            if (batch.Any())
            {
                await table.ExecuteBatchAsync(batch);
            }
        }
    }
}
