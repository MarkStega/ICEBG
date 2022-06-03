using System.IO;
using System.Text;

//
//  2009-04-02  Mark Stega
//              (1) Lost in the mists of time is the origin of this code. I believe it was SharpCore
//                  which is only visible on the Codeplex archive site and dates from 2007.
//
//  2019-04-02  Mark Stega
//              (1) Added Upsert stored procedure per http://michaeljswart.com/2011/09/mythbusting-concurrent-updateinsert-solutions/
//              (2) Simplified from the original by allowing only one primary key and no foreign keys
//

namespace ICEBG.GenerateDataTier
{
    /// <summary>
    /// Generates SQL Server stored procedures for a database.
    /// </summary>
    public static class SqlGenerator
    {
        private static void CreateSeparator(StreamWriter writer, string timeOfGeneration)
        {
            // Create the separator
            writer.WriteLine();
            writer.WriteLine("/******************************************************************************");
            writer.WriteLine("Generated file - Created on " + timeOfGeneration + "; Do not edit!");
            writer.WriteLine("******************************************************************************/");
        }

        public static void CreateUse(Table table, string usingDatabase, string path, string timeOfGeneration)
        {
            string fileName;
            fileName = Path.Combine(path, table.Name + ".sql");

            using (var writer = new StreamWriter(fileName, true))
            {
                CreateSeparator(writer, timeOfGeneration);

                // Create the using statement
                writer.WriteLine("use [" + usingDatabase + "]");
                writer.WriteLine("go");
            }
        }

        public static void CreateInsertStoredProcedure(Table table, string path, string timeOfGeneration)
        {
            // Create the stored procedure name
            string procedureName = table.Name + "_Insert";
            string fileName;

            fileName = Path.Combine(path, table.Name + ".sql");

            using (var writer = new StreamWriter(fileName, true))
            {
                CreateSeparator(writer, timeOfGeneration);

                // Create the drop statment
                writer.WriteLine("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                                 procedureName + "]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                writer.WriteLine("\tdrop procedure [dbo].[" + procedureName + "]");
                writer.WriteLine("GO");
                writer.WriteLine();

                // Create the SQL for the stored procedure
                writer.WriteLine("CREATE PROCEDURE [dbo].[" + procedureName + "]");
                writer.WriteLine("(");

                // Create the parameter list
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];
                    if (column.IsIdentity == false && column.IsRowGuidCol == false)
                    {
                        writer.Write("\t" + SqlUtility.CreateParameterString(column, true));
                        if (i < (table.Columns.Count - 1))
                        {
                            writer.Write(",");
                        }
                        writer.WriteLine();
                    }
                }
                writer.WriteLine(")");

                writer.WriteLine();
                writer.WriteLine("AS");
                writer.WriteLine();
                writer.WriteLine("SET NOCOUNT ON");
                writer.WriteLine();

                // Initialize all RowGuidCol columns
                foreach (Column column in table.Columns)
                {
                    if (column.IsRowGuidCol)
                    {
                        writer.WriteLine("SET @" + column.Name + " = NEWID()");
                        writer.WriteLine();
                        break;
                    }
                }

                writer.WriteLine("INSERT INTO [" + table.Name + "]");
                writer.WriteLine("(");

                // Create the parameter list
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];

                    // Ignore any identity columns
                    if (column.IsIdentity == false)
                    {
                        // Append the column name as a parameter of the insert statement
                        if (i < (table.Columns.Count - 1))
                        {
                            writer.WriteLine("\t[" + column.Name + "],");
                        }
                        else
                        {
                            writer.WriteLine("\t[" + column.Name + "]");
                        }
                    }
                }

                writer.WriteLine(")");
                writer.WriteLine("VALUES");
                writer.WriteLine("(");

                // Create the values list
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];

                    // Is the current column an identity column?
                    if (column.IsIdentity == false)
                    {
                        // Append the necessary line breaks and commas
                        if (i < (table.Columns.Count - 1))
                        {
                            writer.WriteLine("\t@" + column.Name + ",");
                        }
                        else
                        {
                            writer.WriteLine("\t@" + column.Name);
                        }
                    }
                }

                writer.WriteLine(")");
                writer.WriteLine();
                writer.WriteLine("GO");
            }
        }

        /// <summary>
        /// Creates an update stored procedure SQL script for the specified table
        /// </summary>
        /// <param name="table">Instance of the Table class that represents the table this stored procedure will be created for.</param>
        /// <param name="grantLoginName">Name of the SQL Server user that should have execute rights on the stored procedure.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="path">Path where the stored procedure script should be created.</param>
        /// <param name="createMultipleFiles">Indicates the procedure(s) generated should be created in its own file.</param>
        public static void CreateUpdateStoredProcedure(Table table, string path, string timeOfGeneration)
        {
            if (table.PrimaryKeys.Count == 1 && table.Columns.Count > 1)
            {
                // Create the stored procedure name
                string procedureName = table.Name + "_Update";
                string fileName;

                fileName = Path.Combine(path, table.Name + ".sql");

                using (var writer = new StreamWriter(fileName, true))
                {
                    CreateSeparator(writer, timeOfGeneration);

                    // Create the drop statment
                    writer.WriteLine("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                                     procedureName + "]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                    writer.WriteLine("\tdrop procedure [dbo].[" + procedureName + "]");
                    writer.WriteLine("GO");
                    writer.WriteLine();

                    // Create the SQL for the stored procedure
                    writer.WriteLine("CREATE PROCEDURE [dbo].[" + procedureName + "]");
                    writer.WriteLine("(");

                    // Create the parameter list
                    Column column;
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        column = table.Columns[i];

                        if (i < (table.Columns.Count - 1))
                        {
                            writer.WriteLine("\t" + SqlUtility.CreateParameterString(column, false) + ",");
                        }
                        else
                        {
                            writer.WriteLine("\t" + SqlUtility.CreateParameterString(column, false));
                        }
                    }
                    writer.WriteLine(")");

                    writer.WriteLine();
                    writer.WriteLine("AS");
                    writer.WriteLine();
                    writer.WriteLine("SET NOCOUNT ON");
                    writer.WriteLine();
                    writer.WriteLine("UPDATE [" + table.Name + "]");
                    writer.Write("SET");

                    // Create the set statement
                    bool firstLine = true;
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        column = table.Columns[i];

                        // Ignore Identity and RowGuidCol columns
                        if ((table.PrimaryKeys.Contains(column) == false) && (column.IsIdentity == false))
                        {
                            if (firstLine)
                            {
                                writer.Write(" ");
                                firstLine = false;
                            }
                            else
                            {
                                writer.Write("\t");
                            }

                            writer.Write("[" + column.Name + "] = @" + column.Name);

                            if (i < (table.Columns.Count - 1))
                            {
                                writer.Write(",");
                            }

                            writer.WriteLine();
                        }
                    }

                    writer.Write("WHERE");

                    // Create the where clause
                    column = table.PrimaryKeys[0];
                    writer.Write(" [" + column.Name + "] = @" + column.Name);

                    writer.WriteLine();

                    writer.WriteLine("GO");
                }
            }
        }

        public static void CreateUpsertStoredProcedure(Table table, string path, string timeOfGeneration)
        {
            //
            // See if we have an identity column or a RowGuid column
            // If so, do not create an Upsert
            //
            Column column;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                column = table.Columns[i];
                if (column.IsIdentity || column.IsRowGuidCol)
                {
                    return;
                }
            }

            // Create the stored procedure name
            string procedureName = table.Name + "_Upsert";
            string fileName;

            fileName = Path.Combine(path, table.Name + ".sql");

            using (var writer = new StreamWriter(fileName, true))
            {
                CreateSeparator(writer, timeOfGeneration);

                // Create the drop statment
                writer.WriteLine("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                                 procedureName + "]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                writer.WriteLine("\tdrop procedure [dbo].[" + procedureName + "]");
                writer.WriteLine("GO");
                writer.WriteLine();

                // Create the SQL for the stored procedure
                writer.WriteLine("CREATE PROCEDURE [dbo].[" + procedureName + "]");
                writer.WriteLine("(");

                // Create the parameter list
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    column = table.Columns[i];
                    if (column.IsIdentity == false && column.IsRowGuidCol == false)
                    {
                        writer.Write("\t" + SqlUtility.CreateParameterString(column, true));
                        if (i < (table.Columns.Count - 1))
                        {
                            writer.Write(",");
                        }
                        writer.WriteLine();
                    }
                }
                writer.WriteLine(")");

                writer.WriteLine();
                writer.WriteLine("AS");
                writer.WriteLine();
                writer.WriteLine("SET NOCOUNT ON");
                writer.WriteLine();
                writer.WriteLine("SET TRANSACTION ISOLATION LEVEL READ COMMITTED");
                writer.WriteLine();
                writer.WriteLine("BEGIN TRANSACTION");
                writer.WriteLine();
                writer.WriteLine("UPDATE [" + table.Name + "]  WITH (UPDLOCK, HOLDLOCK)");
                writer.Write("SET");

                // Create the set statement
                bool firstLine = true;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    column = table.Columns[i];

                    // Ignore Identity and RowGuidCol columns
                    if ((table.PrimaryKeys.Contains(column) == false) && (column.IsIdentity == false))
                    {
                        if (firstLine)
                        {
                            writer.Write(" ");
                            firstLine = false;
                        }
                        else
                        {
                            writer.Write("\t");
                        }

                        writer.Write("[" + column.Name + "] = @" + column.Name);

                        if (i < (table.Columns.Count - 1))
                        {
                            writer.Write(",");
                        }

                        writer.WriteLine();
                    }
                }

                writer.Write("WHERE");

                // Create the where clause
                column = table.PrimaryKeys[0];
                writer.Write(" [" + column.Name + "] = @" + column.Name);

                writer.WriteLine();

                writer.WriteLine("IF(@@ROWCOUNT = 0)");
                writer.WriteLine("BEGIN");

                writer.WriteLine("\tINSERT INTO [" + table.Name + "]");
                writer.WriteLine("\t(");

                // Create the parameter list
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    column = table.Columns[i];

                    // Ignore any identity columns
                    if (column.IsIdentity == false)
                    {
                        // Append the column name as a parameter of the insert statement
                        if (i < (table.Columns.Count - 1))
                        {
                            writer.WriteLine("\t\t[" + column.Name + "],");
                        }
                        else
                        {
                            writer.WriteLine("\t\t[" + column.Name + "]");
                        }
                    }
                }

                writer.WriteLine("\t)");
                writer.WriteLine("\tVALUES");
                writer.WriteLine("\t(");

                // Create the values list
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    column = table.Columns[i];

                    // Is the current column an identity column?
                    if (column.IsIdentity == false)
                    {
                        // Append the necessary line breaks and commas
                        if (i < (table.Columns.Count - 1))
                        {
                            writer.WriteLine("\t\t@" + column.Name + ",");
                        }
                        else
                        {
                            writer.WriteLine("\t\t@" + column.Name);
                        }
                    }
                }

                writer.WriteLine("\t)");
                writer.WriteLine();
                writer.WriteLine("END");

                writer.WriteLine();
                writer.WriteLine("COMMIT");
                writer.WriteLine();
                writer.WriteLine("GO");
            }
        }

        /// <summary>
        /// Creates an delete stored procedure SQL script for the specified table
        /// </summary>
        /// <param name="table">Instance of the Table class that represents the table this stored procedure will be created for.</param>
        /// <param name="grantLoginName">Name of the SQL Server user that should have execute rights on the stored procedure.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="path">Path where the stored procedure script should be created.</param>
        /// <param name="createMultipleFiles">Indicates the procedure(s) generated should be created in its own file.</param>
        public static void CreateDeleteStoredProcedure(Table table, string path, string timeOfGeneration)
        {
            if (table.PrimaryKeys.Count == 1)
            {
                // Create the stored procedure name
                string procedureName = table.Name + "_Delete";
                string fileName;

                fileName = Path.Combine(path, table.Name + ".sql");

                using (var writer = new StreamWriter(fileName, true))
                {
                    CreateSeparator(writer, timeOfGeneration);

                    // Create the drop statment
                    writer.WriteLine("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                                     procedureName + "]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                    writer.WriteLine("\tdrop procedure [dbo].[" + procedureName + "]");
                    writer.WriteLine("GO");
                    writer.WriteLine();

                    // Create the SQL for the stored procedure
                    writer.WriteLine("CREATE PROCEDURE [dbo].[" + procedureName + "]");
                    writer.WriteLine("(");

                    // Create the parameter list
                    Column column = table.PrimaryKeys[0];
                    writer.WriteLine("\t" + SqlUtility.CreateParameterString(column, false));

                    writer.WriteLine(")");

                    writer.WriteLine();
                    writer.WriteLine("AS");
                    writer.WriteLine();
                    writer.WriteLine("SET NOCOUNT ON");
                    writer.WriteLine();
                    writer.WriteLine("DELETE FROM [" + table.Name + "]");
                    writer.Write("WHERE");

                    // Create the where clause
                    column = table.PrimaryKeys[0];
                    writer.WriteLine(" [" + column.Name + "] = @" + column.Name);

                    writer.WriteLine("GO");
                }
            }
        }

        /// <summary>
        /// Creates an select stored procedure SQL script for the specified table
        /// </summary>
        /// <param name="table">Instance of the Table class that represents the table this stored procedure will be created for.</param>
        /// <param name="grantLoginName">Name of the SQL Server user that should have execute rights on the stored procedure.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="path">Path where the stored procedure script should be created.</param>
        /// <param name="createMultipleFiles">Indicates the procedure(s) generated should be created in its own file.</param>
        public static void CreateSelectStoredProcedure(Table table, string path, string timeOfGeneration)
        {
            if (table.PrimaryKeys.Count == 1)
            {
                // Create the stored procedure name
                string procedureName = table.Name + "_Select";
                string fileName;

                fileName = Path.Combine(path, table.Name + ".sql");

                using (var writer = new StreamWriter(fileName, true))
                {
                    CreateSeparator(writer, timeOfGeneration);

                    // Create the drop statment
                    writer.WriteLine("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                                     procedureName + "]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                    writer.WriteLine("\tdrop procedure [dbo].[" + procedureName + "]");
                    writer.WriteLine("GO");
                    writer.WriteLine();

                    // Create the SQL for the stored procedure
                    writer.WriteLine("CREATE PROCEDURE [dbo].[" + procedureName + "]");
                    writer.WriteLine("(");

                    // Create the parameter list
                    Column column = table.PrimaryKeys[0];
                    writer.WriteLine("\t" + SqlUtility.CreateParameterString(column, false));

                    writer.WriteLine(")");

                    writer.WriteLine();
                    writer.WriteLine("AS");
                    writer.WriteLine();
                    writer.WriteLine("SET NOCOUNT ON");
                    writer.WriteLine();
                    writer.Write("SELECT");

                    // Create the list of columns
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        column = table.Columns[i];

                        if (i == 0)
                        {
                            writer.Write(" ");
                        }
                        else
                        {
                            writer.Write("\t");
                        }

                        writer.Write("[" + column.Name + "]");

                        if (i < (table.Columns.Count - 1))
                        {
                            writer.Write(",");
                        }

                        writer.WriteLine();
                    }

                    writer.WriteLine("FROM [" + table.Name + "]");
                    writer.Write("WHERE");

                    // Create the where clause
                    column = table.PrimaryKeys[0];
                    writer.WriteLine(" [" + column.Name + "] = @" + column.Name);

                    writer.WriteLine("GO");
                }
            }
        }

        /// <summary>
        /// Creates an select all stored procedure SQL script for the specified table
        /// </summary>
        /// <param name="table">Instance of the Table class that represents the table this stored procedure will be created for.</param>
        /// <param name="grantLoginName">Name of the SQL Server user that should have execute rights on the stored procedure.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="path">Path where the stored procedure script should be created.</param>
        /// <param name="createMultipleFiles">Indicates the procedure(s) generated should be created in its own file.</param>
        public static void CreateSelectAllStoredProcedure(Table table, string path, string timeOfGeneration)
        {
            if (table.PrimaryKeys.Count == 1)
            {
                // Create the stored procedure name
                string procedureName = table.Name + "_SelectAll";
                string fileName;

                fileName = Path.Combine(path, table.Name + ".sql");

                using (var writer = new StreamWriter(fileName, true))
                {
                    CreateSeparator(writer, timeOfGeneration);

                    // Create the drop statment
                    writer.WriteLine("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                                     procedureName + "]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                    writer.WriteLine("\tdrop procedure [dbo].[" + procedureName + "]");
                    writer.WriteLine("GO");
                    writer.WriteLine();

                    // Create the SQL for the stored procedure
                    writer.WriteLine("CREATE PROCEDURE [dbo].[" + procedureName + "]");
                    writer.WriteLine();
                    writer.WriteLine("AS");
                    writer.WriteLine();
                    writer.WriteLine("SET NOCOUNT ON");
                    writer.WriteLine();
                    writer.Write("SELECT");

                    // Create the list of columns
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Column column = table.Columns[i];

                        if (i == 0)
                        {
                            writer.Write(" ");
                        }
                        else
                        {
                            writer.Write("\t");
                        }

                        writer.Write("[" + column.Name + "]");

                        if (i < (table.Columns.Count - 1))
                        {
                            writer.Write(",");
                        }

                        writer.WriteLine();
                    }

                    writer.WriteLine("FROM [" + table.Name + "]");

                    writer.WriteLine("GO");
                }
            }
        }

        /// <summary>
        /// Creates one or more select stored procedures SQL script for the specified table and its foreign keys
        /// </summary>
        /// <param name="table">Instance of the Table class that represents the table this stored procedure will be created for.</param>
        /// <param name="grantLoginName">Name of the SQL Server user that should have execute rights on the stored procedure.</param>
        /// <param name="storedProcedurePrefix">Prefix to be appended to the name of the stored procedure.</param>
        /// <param name="path">Path where the stored procedure script should be created.</param>
        /// <param name="createMultipleFiles">Indicates the procedure(s) generated should be created in its own file.</param>
        public static void CreateSelectAllByStoredProcedures(Table table, string path, string timeOfGeneration)
        {
            // Create a stored procedure for each foreign key
            foreach (var compositeKeyList in table.ForeignKeys.Values)
            {
                // Create the stored procedure name
                var stringBuilder = new StringBuilder(255);
                stringBuilder.Append(table.Name + "_SelectAllBy");

                // Create the parameter list
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    if (i > 0)
                    {
                        stringBuilder.Append("_" + SqlUtility.FormatPascal(column.Name));
                    }
                    else
                    {
                        stringBuilder.Append(SqlUtility.FormatPascal(column.Name));
                    }
                }

                string procedureName = stringBuilder.ToString();
                string fileName;

                fileName = Path.Combine(path, table.Name + ".sql");

                using (var writer = new StreamWriter(fileName, true))
                {
                    CreateSeparator(writer, timeOfGeneration);

                    // Create the drop statment
                    writer.WriteLine("if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[" +
                                     procedureName + "]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)");
                    writer.WriteLine("\tdrop procedure [dbo].[" + procedureName + "]");
                    writer.WriteLine("GO");
                    writer.WriteLine();

                    // Create the SQL for the stored procedure
                    writer.WriteLine("CREATE PROCEDURE [dbo].[" + procedureName + "]");
                    writer.WriteLine("(");

                    // Create the parameter list
                    for (int i = 0; i < compositeKeyList.Count; i++)
                    {
                        Column column = compositeKeyList[i];

                        if (i < (compositeKeyList.Count - 1))
                        {
                            writer.WriteLine("\t" + SqlUtility.CreateParameterString(column, false) + ",");
                        }
                        else
                        {
                            writer.WriteLine("\t" + SqlUtility.CreateParameterString(column, false));
                        }
                    }
                    writer.WriteLine(")");

                    writer.WriteLine();
                    writer.WriteLine("AS");
                    writer.WriteLine();
                    writer.WriteLine("SET NOCOUNT ON");
                    writer.WriteLine();
                    writer.Write("SELECT");

                    // Create the list of columns
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Column column = table.Columns[i];

                        if (i == 0)
                        {
                            writer.Write(" ");
                        }
                        else
                        {
                            writer.Write("\t");
                        }

                        writer.Write("[" + column.Name + "]");

                        if (i < (table.Columns.Count - 1))
                        {
                            writer.Write(",");
                        }

                        writer.WriteLine();
                    }

                    writer.WriteLine("FROM [" + table.Name + "]");
                    writer.Write("WHERE");

                    // Create the where clause
                    for (int i = 0; i < compositeKeyList.Count; i++)
                    {
                        Column column = compositeKeyList[i];

                        if (i == 0)
                        {
                            writer.WriteLine(" [" + column.Name + "] = @" + column.Name);
                        }
                        else
                        {
                            writer.WriteLine("\tAND [" + column.Name + "] = @" + column.Name);
                        }
                    }

                    writer.WriteLine("GO");
                }
            }
        }

    }
}

