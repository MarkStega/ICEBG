using ICEBG.GeneratorUtilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ICEBG.GenerateDataTier
{
    internal delegate void CountUpdate(object sender, CountEventArgs e);

    /// <summary>
    /// Generates C# and SQL code for accessing a database.
    /// </summary>
    internal static class GenerateDataTier
    {
        public static event CountUpdate databaseCounted;
        public static event CountUpdate tableCounted;

        public static void GenerateDataTierElements(
            string connectionString,
            string usingDatabase,
            string outputDirectory,
            bool deleteOutputDALDirectory)
        {
            var tableList = new List<Table>();
            string databaseName;
            string sqlPath;
            string dalPath;
            string csLogicPath;
            string csDataPath;

            using (var connection = new SqlConnection(connectionString))
            {
                databaseName = SqlUtility.FormatPascal(connection.Database);
                dalPath = Path.Combine(outputDirectory, "DataTier");
                sqlPath = Path.Combine(dalPath, "SQL");
                csLogicPath = Path.Combine(dalPath, "BusinessLogic");
                csDataPath = Path.Combine(dalPath, "DataDefinitions");

                connection.Open();

                // Get a list of the entities in the database
                var dataTable = new DataTable();
                var dataAdapter = new SqlDataAdapter(SqlUtility.GetTableQuery(connection.Database), connection);
                dataAdapter.Fill(dataTable);

                // Process each table
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var table = new Table();
                    table.Name = (string) dataRow["TABLE_NAME"];
                    QueryTable(connection, table);
                    tableList.Add(table);
                }
            }

            databaseCounted(null, new CountEventArgs(tableList.Count));

            // Generate the necessary SQL and C# code for each table
            int count = 0;
            if (tableList.Count > 0)
            {
                // Create the necessary directories
                FileUtility.CreateSubDirectory(dalPath, deleteOutputDALDirectory);
                FileUtility.CreateSubDirectory(sqlPath, false);
                FileUtility.CreateSubDirectory(csDataPath, false);
                FileUtility.CreateSubDirectory(csLogicPath, false);

                string timeOfGeneration = DateTime.Now.ToShortDateString();

                // Create the CRUD stored procedures and data access code for each table
                foreach (Table table in tableList)
                {
                    SqlGenerator.CreateUse(table, usingDatabase, sqlPath, timeOfGeneration);
                    SqlGenerator.CreateInsertStoredProcedure(table, sqlPath, timeOfGeneration);
                    SqlGenerator.CreateUpdateStoredProcedure(table, sqlPath, timeOfGeneration);
                    SqlGenerator.CreateUpsertStoredProcedure(table, sqlPath, timeOfGeneration);
                    SqlGenerator.CreateDeleteStoredProcedure(table, sqlPath, timeOfGeneration);
                    SqlGenerator.CreateSelectStoredProcedure(table, sqlPath, timeOfGeneration);
                    SqlGenerator.CreateSelectAllStoredProcedure(table, sqlPath, timeOfGeneration);
                    SqlGenerator.CreateSelectAllByStoredProcedures(table, sqlPath, timeOfGeneration);

                    CsGenerator.CreateDataTransferClass(table, csDataPath, timeOfGeneration);
                    CsGenerator.CreateDataAccessClass(table, csLogicPath, timeOfGeneration);

                    count++;
                    tableCounted(null, new CountEventArgs(count));
                }
            }
        }

        /// <summary>
        /// Retrieves the column, primary key, and foreign key information for the specified table.
        /// </summary>
        /// <param name="connection">The SqlConnection to be used when querying for the table information.</param>
        /// <param name="table">The table instance that information should be retrieved for.</param>
        private static void QueryTable(SqlConnection connection, Table table)
        {
            // Get a list of the entities in the database
            var dataTable = new DataTable();
            var dataAdapter = new SqlDataAdapter(SqlUtility.GetColumnQuery(table.Name), connection);
            dataAdapter.Fill(dataTable);

            foreach (DataRow columnRow in dataTable.Rows)
            {
                var column = new Column();
                column.Name = columnRow["COLUMN_NAME"].ToString();
                column.Type = columnRow["DATA_TYPE"].ToString();
                column.Precision = columnRow["NUMERIC_PRECISION"].ToString();
                column.Scale = columnRow["NUMERIC_SCALE"].ToString();

                // Determine the column's length
                if (columnRow["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value)
                {
                    column.Length = columnRow["CHARACTER_MAXIMUM_LENGTH"].ToString();
                }
                else
                {
                    column.Length = columnRow["COLUMN_LENGTH"].ToString();
                }

                // Is the column a RowGuidCol column?
                if (columnRow["IS_ROWGUIDCOL"].ToString() == "1")
                {
                    column.IsRowGuidCol = true;
                }

                // Is the column an Identity column?
                if (columnRow["IS_IDENTITY"].ToString() == "1")
                {
                    column.IsIdentity = true;
                }

                // Is columnRow column a computed column?
                if (columnRow["IS_COMPUTED"].ToString() == "1")
                {
                    column.IsComputed = true;
                }

                table.Columns.Add(column);
            }

            // Get the list of primary keys
            DataTable primaryKeyTable = SqlUtility.GetPrimaryKeyList(connection, table.Name);
            foreach (DataRow primaryKeyRow in primaryKeyTable.Rows)
            {
                string primaryKeyName = primaryKeyRow["COLUMN_NAME"].ToString();

                foreach (Column column in table.Columns)
                {
                    if (column.Name == primaryKeyName)
                    {
                        table.PrimaryKeys.Add(column);
                        break;
                    }
                }
            }

            // Get the list of foreign keys
            DataTable foreignKeyTable = SqlUtility.GetForeignKeyList(connection, table.Name);
            foreach (DataRow foreignKeyRow in foreignKeyTable.Rows)
            {
                string name = foreignKeyRow["FK_NAME"].ToString();
                string columnName = foreignKeyRow["FKCOLUMN_NAME"].ToString();

                if (table.ForeignKeys.ContainsKey(name) == false)
                {
                    table.ForeignKeys.Add(name, new List<Column>());
                }

                List<Column> foreignKeys = table.ForeignKeys[name];

                foreach (Column column in table.Columns)
                {
                    if (column.Name == columnName)
                    {
                        foreignKeys.Add(column);
                        break;
                    }
                }
            }
        }
    }
}
