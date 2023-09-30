SELECT
    m.name AS table_name,
    p.name AS column_name,
    p.[type] AS data_type,
    f.[table] AS referenced_table,
    f.[to] AS referenced_column
FROM
    sqlite_master AS m
JOIN
    pragma_table_info(m.name) AS p
LEFT JOIN
    pragma_foreign_key_list(m.name) AS f
ON
    p.name = f.'from'
WHERE
    m.type = 'table'
ORDER BY
    m.name, p.cid;