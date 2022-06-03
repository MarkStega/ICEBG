using System;
using System.IO;
using System.Text;
using ICEBG.GeneratorUtilities;

//
//  2009-04-02  Mark Stega
//              (1) Lost in the mists of time is the origin of this code. I believe it was SharpCore
//                  which is only visible on the Codeplex archive site and dates from 2007.
//
//  2019-04-02  Mark Stega
//              (1) Added Upsert method generation
//              (2) Simplified from the original by allowing only one primary key and no foreign keys
//
//  2019-09-27  Mark Stega
//              (1) Changed simple POCO fields to properties with setter and getter to accomodate
//                  the System.Text.Json serializer/deserializer in ASP.Net 3.0.0
//

namespace ICEBG.GenerateDataTier
{
    /// <summary>
    /// Generates C# data access and data transfer classes.
    /// </summary>
    public static class CsGenerator
    {
        private static void CreateSeparator(StreamWriter writer, string creationTime)
        {
            // Create the separator
            writer.WriteLine();
            writer.WriteLine("/******************************************************************************");
            writer.WriteLine("Generated file - Created on " + creationTime + "; Do not edit!");
            writer.WriteLine("******************************************************************************/");
            writer.WriteLine("");
        }

        public static void CreateDataTransferClass(Table table, string path, string timeOfGeneration)
        {
            string className = SqlUtility.FormatClassName(table.Name) + "_DD";

            using (var streamWriter = new StreamWriter(Path.Combine(path, className + ".cs")))
            {
                // Create the header for the class
                CreateSeparator(streamWriter, timeOfGeneration);
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace ICEBG.DataTier.DataDefinitions");
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("\tpublic class " + className);
                streamWriter.WriteLine("\t{");

                // Append the private members
                streamWriter.WriteLine("\t\t#region Properties");
                streamWriter.WriteLine();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];
                    string parameter = SqlUtility.CreateMethodParameter(column);
                    string type = parameter.Split(' ')[0];
                    string name = parameter.Split(' ')[1];

                    streamWriter.WriteLine("\t\tpublic " + type + " " + SqlUtility.FormatPascal(name) + " { get; set; }");
                }

                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                // Create an explicit public constructor
                streamWriter.WriteLine("\t\t#region Constructors");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Initializes a new instance of the " + className + " class.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic " + className + "()");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();

                // Create the "partial" constructor (Skips identity & RowGUID columns)
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Initializes a new instance of the " + className + " class.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.Write("\t\tpublic " + className + "(");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];
                    if (column.IsIdentity == false && column.IsRowGuidCol == false)
                    {
                        streamWriter.Write(SqlUtility.CreateMethodParameter(column));
                        if (i < (table.Columns.Count - 1))
                        {
                            streamWriter.Write(", ");
                        }
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");
                foreach (Column column in table.Columns)
                {
                    if (column.IsIdentity == false && column.IsRowGuidCol == false)
                    {
                        streamWriter.WriteLine("\t\t\tthis." + column.Name + " = " + SqlUtility.FormatCamel(column.Name) +
                                               ";");
                    }
                }
                streamWriter.WriteLine("\t\t}");

                // Create the "copy" constructor
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Initializes a new instance of the " + className + " class.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic " + className + "(" + className + " classInstance)");
                streamWriter.WriteLine("\t\t{");
                foreach (Column column in table.Columns)
                {
                    if (column.IsIdentity == false && column.IsRowGuidCol == false)
                    {
                        streamWriter.WriteLine(
                            "\t\t\tthis." + column.Name + " = classInstance." + column.Name + ";");
                    }
                }
                streamWriter.WriteLine("\t\t}");

                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();
/*
                // Append the public properties
                streamWriter.WriteLine("\t\t#region Properties");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];
                    string parameter = Utility.CreateMethodParameter(column);
                    string type = parameter.Split(' ')[0];
                    string name = parameter.Split(' ')[1];

                    streamWriter.WriteLine("\t\t/// <summary>");
                    streamWriter.WriteLine("\t\t/// Gets or sets the " + Utility.FormatPascal(name) + " value.");
                    streamWriter.WriteLine("\t\t/// </summary>");
                    streamWriter.WriteLine("\t\tpublic virtual " + type + " " + Utility.FormatPascal(name));
                    streamWriter.WriteLine("\t\t{");
                    streamWriter.WriteLine("\t\t\tget { return " + name + "; }");
                    streamWriter.WriteLine("\t\t\tset { " + name + " = value; }");
                    streamWriter.WriteLine("\t\t}");

                    if (i < (table.Columns.Count - 1))
                    {
                        streamWriter.WriteLine();
                    }
                }
                
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
*/

                // Close out the class and namespace
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
        }

        public static void CreateDataAccessClass(Table table, string path, string timeOfGeneration)
        {
            string className = SqlUtility.FormatClassName(table.Name) + "_BL";

            using (var streamWriter = new StreamWriter(Path.Combine(path, className + ".cs")))
            {
                // Create the header for the class
                CreateSeparator(streamWriter, timeOfGeneration);
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Collections.Generic;");
                streamWriter.WriteLine("using System.Data;");
                streamWriter.WriteLine("using System.Data.SqlClient;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("using ICEBG.DataTier.DataDefinitions;");
                streamWriter.WriteLine("using ICEBG.DataTier.HelperClasses;");
                streamWriter.WriteLine();
                streamWriter.WriteLine("namespace ICEBG.DataTier.BusinessLogic");
                streamWriter.WriteLine("{");

                streamWriter.WriteLine("\tpublic partial class " + className);
                streamWriter.WriteLine("\t{");

                // Append the fields
                streamWriter.WriteLine("\t\t#region Properties");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\tprotected SqlConnection m_SqlConnection { get; set; }");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                // Append the constructors
                streamWriter.WriteLine("\t\t#region Constructors");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\tpublic " + className + "(string connectionStringName)");
                streamWriter.WriteLine("\t\t{");
                streamWriter.WriteLine(
                    "\t\t\tValidationUtility.ValidateArgument(\"connectionStringName\", connectionStringName);");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\tm_SqlConnection = new SqlConnection(connectionStringName);");
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");
                streamWriter.WriteLine();

                // Append the access methods
                streamWriter.WriteLine("\t\t#region Methods");
                streamWriter.WriteLine();

                CreateInsertMethod(table, streamWriter);
                CreateUpdateMethod(table, streamWriter);
                CreateUpsertMethod(table, streamWriter);
                CreateDeleteMethod(table, streamWriter);
                CreateSelectMethod(table, streamWriter);
                CreateSelectAllMethod(table, streamWriter);
                CreateSelectAllByMethods(table, streamWriter);
                CreateMakeMethod(table, streamWriter);

                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t#endregion");

                // Close out the class and namespace
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
        }

        private static void CreateInsertMethod(Table table, StreamWriter streamWriter)
        {
            string className = SqlUtility.FormatClassName(table.Name);
            string variableName = SqlUtility.FormatCamel(className);

            // Append the method header
            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// Saves a record to the " + table.Name + " table.");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tpublic virtual void Insert(" + className + "_DD " + variableName + ")");
            streamWriter.WriteLine("\t\t{");

            // Append validation for the parameter
            streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"" + variableName + "\", " + variableName +
                                   ");");
            streamWriter.WriteLine();

            // Append the parameter declarations
            streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
            streamWriter.WriteLine("\t\t\t{");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column column = table.Columns[i];
                if (column.IsIdentity == false && column.IsRowGuidCol == false)
                {
                    streamWriter.Write("\t\t\t\t" + SqlUtility.CreateSqlParameter(table, column));
                    if (i < (table.Columns.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }
            }

            streamWriter.WriteLine("\t\t\t};");
            streamWriter.WriteLine();

            bool hasReturnValue = false;
            foreach (Column column in table.Columns)
            {
                if (column.IsIdentity || column.IsRowGuidCol)
                {
                    if (column.IsIdentity && Convert.ToInt32(column.Length) == 4)
                    {
                        // changed from "(int)..." to ConvertToInt32(...)" after conversion failure from decimal to int

                        streamWriter.WriteLine("\t\t\t" + variableName + "." + SqlUtility.FormatPascal(column.Name) +
                                               " = Convert.ToInt32(SqlClientUtility.ExecuteScalar(m_SqlConnection, CommandType.StoredProcedure, \"" +
                                               table.Name + "_Insert\", parameters));");
                        hasReturnValue = true;
                    }
                    else if (column.IsIdentity && Convert.ToInt32(column.Length) == 8)
                    {
                        // changed from "(int)..." to ConvertToInt64(...)" after above error

                        streamWriter.WriteLine("\t\t\t" + variableName + "." + SqlUtility.FormatPascal(column.Name) +
                                               " = Convert.ToInt64(SqlClientUtility.ExecuteScalar(m_SqlConnection, CommandType.StoredProcedure, \"" +
                                               table.Name + "_Insert\", parameters));");
                        hasReturnValue = true;
                    }
                    else if (column.IsRowGuidCol)
                    {
                        streamWriter.WriteLine("\t\t\t" + variableName + "." + SqlUtility.FormatPascal(column.Name) +
                                               " = (Guid) SqlClientUtility.ExecuteScalar(m_SqlConnection, CommandType.StoredProcedure, \"" +
                                               table.Name + "_Insert\", parameters);");
                        hasReturnValue = true;
                    }
                }
            }

            if (hasReturnValue == false)
            {
                streamWriter.WriteLine(
                    "\t\t\tSqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, \"" +
                    table.Name + "_Insert\", parameters);");
            }

            // Append the method footer
            streamWriter.WriteLine("\t\t}");
            streamWriter.WriteLine();
        }

        private static void CreateUpdateMethod(Table table, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count == 1 && table.Columns.Count > 1)
            {
                string className = SqlUtility.FormatClassName(table.Name);
                string variableName = SqlUtility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Updates a record in the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic virtual void Update(" + className + "_DD " + variableName + ")");
                streamWriter.WriteLine("\t\t{");

                // Append validation for the parameter
                streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"" + variableName + "\", " +
                                       variableName + ");");
                streamWriter.WriteLine();

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Column column = table.Columns[i];
                    streamWriter.Write("\t\t\t\t" + SqlUtility.CreateSqlParameter(table, column));
                    if (i < (table.Columns.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                streamWriter.WriteLine(
                    "\t\t\tSqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, \"" +
                    table.Name + "_Update\", parameters);");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        private static void CreateUpsertMethod(Table table, StreamWriter streamWriter)
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

            if (table.PrimaryKeys.Count == 1 && table.Columns.Count > 1)
            {
                string className = SqlUtility.FormatClassName(table.Name);
                string variableName = SqlUtility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Updates or inserts a record in the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic virtual void Upsert(" + className + "_DD " + variableName + ")");
                streamWriter.WriteLine("\t\t{");

                // Append validation for the parameter
                streamWriter.WriteLine("\t\t\tValidationUtility.ValidateArgument(\"" + variableName + "\", " +
                                       variableName + ");");
                streamWriter.WriteLine();

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    column = table.Columns[i];
                    streamWriter.Write("\t\t\t\t" + SqlUtility.CreateSqlParameter(table, column));
                    if (i < (table.Columns.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                streamWriter.WriteLine(
                    "\t\t\tSqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, \"" +
                    table.Name + "_Upsert\", parameters);");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        private static void CreateDeleteMethod(Table table, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count == 1)
            {
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Deletes a record from the " + table.Name + " table by its primary key.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.Write("\t\tpublic virtual void Delete(");

                Column column = table.PrimaryKeys[0];
                streamWriter.Write(SqlUtility.CreateMethodParameter(column));

                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");

                column = table.Columns[0];
                streamWriter.Write("\t\t\t\tnew SqlParameter(\"@" + column.Name + "\", " +
                                    SqlUtility.FormatCamel(column.Name) + ")");
                streamWriter.WriteLine();

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                // Append the stored procedure execution
                streamWriter.WriteLine(
                    "\t\t\tSqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, \"" +
                    table.Name + "_Delete\", parameters);");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        private static void CreateSelectMethod(Table table, StreamWriter streamWriter)
        {
            if (table.PrimaryKeys.Count == 1 )
            {
                string className = SqlUtility.FormatClassName(table.Name);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects a single record from the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic virtual " + className + "_DD Select(");
                Column column = table.PrimaryKeys[0];
                streamWriter.Write(SqlUtility.CreateMethodParameter(column));
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                column = table.Columns[0];
                streamWriter.Write("\t\t\t\tnew SqlParameter(\"@" + column.Name + "\", " +
                                    SqlUtility.FormatCamel(column.Name) + ")");

                streamWriter.WriteLine();

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                // Append the stored procedure execution
                streamWriter.WriteLine(
                    "\t\t\tusing (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(m_SqlConnection, CommandType.StoredProcedure, \"" +
                    table.Name + "_Select\", parameters))");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tif (dataReader.Read())");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\treturn Make" + className + "(dataReader);");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine("\t\t\t\telse");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\treturn null;");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine("\t\t\t}");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        private static void CreateSelectAllMethod(Table table, StreamWriter streamWriter)
        {
            if (table.Columns.Count > 0 && table.Columns.Count != table.ForeignKeys.Count)
            {
                string className = SqlUtility.FormatClassName(table.Name);
                string dtoVariableName = SqlUtility.FormatCamel(className);

                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects all records from the " + table.Name + " table.");
                streamWriter.WriteLine("\t\t/// </summary>");
                streamWriter.WriteLine("\t\tpublic virtual List<" + className + "_DD> SelectAll()");
                streamWriter.WriteLine("\t\t{");

                // Append the stored procedure execution
                streamWriter.WriteLine(
                    "\t\t\tusing (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(m_SqlConnection, CommandType.StoredProcedure, \"" +
                    table.Name + "_SelectAll\"))");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tList<" + className + "_DD> " + dtoVariableName + "List = new List<" +
                                       className + "_DD>();");
                streamWriter.WriteLine("\t\t\t\twhile (dataReader.Read())");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\t" + className + "_DD " + dtoVariableName + " = Make" + className +
                                       "(dataReader);");
                streamWriter.WriteLine("\t\t\t\t\t" + dtoVariableName + "List.Add(" + dtoVariableName + ");");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\treturn " + dtoVariableName + "List;");
                streamWriter.WriteLine("\t\t\t}");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        private static void CreateSelectAllByMethods(Table table, StreamWriter streamWriter)
        {
            string className = SqlUtility.FormatClassName(table.Name);
            string dtoVariableName = SqlUtility.FormatCamel(className);

            // Create a stored procedure for each foreign key
            foreach (var compositeKeyList in table.ForeignKeys.Values)
            {
                // Create the stored procedure name
                var stringBuilder = new StringBuilder(255);
                stringBuilder.Append("SelectAllBy");
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
                string methodName = stringBuilder.ToString();
                string procedureName = table.Name + "_" + methodName;

                // Create the select function based on keys
                // Append the method header
                streamWriter.WriteLine("\t\t/// <summary>");
                streamWriter.WriteLine("\t\t/// Selects all records from the " + table.Name + " table by a foreign key.");
                streamWriter.WriteLine("\t\t/// </summary>");

                streamWriter.Write("\t\tpublic virtual List<" + className + "_DD> " + methodName + "(");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write(SqlUtility.CreateMethodParameter(column));
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(", ");
                    }
                }
                streamWriter.WriteLine(")");
                streamWriter.WriteLine("\t\t{");

                // Append the parameter declarations
                streamWriter.WriteLine("\t\t\tSqlParameter[] parameters = new SqlParameter[]");
                streamWriter.WriteLine("\t\t\t{");
                for (int i = 0; i < compositeKeyList.Count; i++)
                {
                    Column column = compositeKeyList[i];
                    streamWriter.Write("\t\t\t\tnew SqlParameter(\"@" + column.Name + "\", " +
                                       SqlUtility.FormatCamel(column.Name) + ")");
                    if (i < (compositeKeyList.Count - 1))
                    {
                        streamWriter.Write(",");
                    }

                    streamWriter.WriteLine();
                }

                streamWriter.WriteLine("\t\t\t};");
                streamWriter.WriteLine();

                // Append the stored procedure execution
                streamWriter.WriteLine(
                    "\t\t\tusing (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(m_SqlConnection, CommandType.StoredProcedure, \"" +
                    procedureName + "\", parameters))");
                streamWriter.WriteLine("\t\t\t{");
                streamWriter.WriteLine("\t\t\t\tList<" + className + "_DD> " + dtoVariableName + "List = new List<" +
                                       className + "_DD>();");
                streamWriter.WriteLine("\t\t\t\twhile (dataReader.Read())");
                streamWriter.WriteLine("\t\t\t\t{");
                streamWriter.WriteLine("\t\t\t\t\t" + className + "_DD " + dtoVariableName + " = Make" + className +
                                       "(dataReader);");
                streamWriter.WriteLine("\t\t\t\t\t" + dtoVariableName + "List.Add(" + dtoVariableName + ");");
                streamWriter.WriteLine("\t\t\t\t}");
                streamWriter.WriteLine();
                streamWriter.WriteLine("\t\t\t\treturn " + dtoVariableName + "List;");
                streamWriter.WriteLine("\t\t\t}");

                // Append the method footer
                streamWriter.WriteLine("\t\t}");
                streamWriter.WriteLine();
            }
        }

        private static void CreateMakeMethod(Table table, StreamWriter streamWriter)
        {
            string className = SqlUtility.FormatClassName(table.Name);
            string dtoVariableName = SqlUtility.FormatCamel(className);

            streamWriter.WriteLine("\t\t/// <summary>");
            streamWriter.WriteLine("\t\t/// Creates a new instance of the " + table.Name +
                                   " class and populates it with data from the specified SqlDataReader.");
            streamWriter.WriteLine("\t\t/// </summary>");
            streamWriter.WriteLine("\t\tprotected virtual " + className + "_DD Make" + className +
                                   "(SqlDataReader dataReader)");
            streamWriter.WriteLine("\t\t{");
            streamWriter.WriteLine("\t\t\t" + className + "_DD " + dtoVariableName + " = new " + className + "_DD();");

            foreach (Column column in table.Columns)
            {
                string columnNamePascal = SqlUtility.FormatPascal(column.Name);
                streamWriter.WriteLine("\t\t\t" + dtoVariableName + "." + columnNamePascal + " = SqlClientUtility." +
                                       SqlUtility.GetXxxMethod(column) + "(dataReader, \"" + column.Name + "\", " +
                                       SqlUtility.GetDefaultValue(column) + ");");
            }

            streamWriter.WriteLine();
            streamWriter.WriteLine("\t\t\treturn " + dtoVariableName + ";");
            streamWriter.WriteLine("\t\t}");
        }
    }
}
