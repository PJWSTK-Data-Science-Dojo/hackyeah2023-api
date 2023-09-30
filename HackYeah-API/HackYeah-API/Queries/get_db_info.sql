WITH DatabaseSize AS (
    SELECT page_count * page_size AS database_size_bytes
    FROM pragma_page_count(), pragma_page_size()
),
DatabaseVersion AS (
    SELECT sqlite_version() AS version
),
AttachedDatabases AS (
    SELECT name AS database_name, file AS file_path
    FROM pragma_database_list()
),
TableInfo AS (
    SELECT
        name AS table_name,
        sql AS create_statement
    FROM sqlite_master
    WHERE type = 'table'
)
SELECT
    (SELECT database_size_bytes FROM DatabaseSize) AS database_size_bytes,
    (SELECT version FROM DatabaseVersion) AS sqlite_version,
    (SELECT COUNT(*) FROM AttachedDatabases) AS attached_database_count,
    (SELECT group_concat(database_name, ', ') FROM AttachedDatabases) AS attached_databases,
    table_name,
    create_statement
FROM TableInfo;
