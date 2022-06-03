using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

//
//  2016-03-28  Mark Stega
//              Added command timeout of 600 seconds
//

namespace ICEBG.DataTier.HelperClasses;

public static class SqlClientUtility
{
    #region ExecuteXXX methods

    public static void ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText,
                                       params SqlParameter[] parameters)
    {
        if (connection != null && connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
        {
            command.ExecuteNonQuery();
        }
    }

    public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText,
                                              params SqlParameter[] parameters)
    {
        if (connection != null && connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
        {
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }

    public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText,
                                       params SqlParameter[] parameters)
    {
        if (connection != null && connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
        {
            return command.ExecuteScalar();
        }
    }

    public static DataSet ExecuteDataSet(SqlConnection connection, CommandType commandType, string commandText,
                                         params SqlParameter[] parameters)
    {
        if (connection != null && connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        using (SqlCommand command = CreateCommand(connection, commandType, commandText, parameters))
        {
            return CreateDataSet(command);
        }
    }

    public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText,
                                             params SqlParameter[] values)
    {
        if (connection != null && connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        using (SqlCommand command = CreateCommand(connection, commandType, commandText, values))
        {
            return CreateDataTable(command);
        }
    }

    #endregion

    #region Utility functions

    /// <summary>
    /// Converts the specified value to <code>DBNull.Value</code> if it is <code>null</code>.
    /// </summary>
    /// <param name="value">The value that should be checked for <code>null</code>.</param>
    /// <returns>The original value if it is not null, otherwise <code>DBNull.Value</code>.</returns>
    private static object CheckValue(object value)
    {
        if (value == null)
        {
            return DBNull.Value;
        }
        else
        {
            return value;
        }
    }

    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities"),
     SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    private static SqlCommand CreateCommand(SqlConnection connection, CommandType commandType, string commandText)
    {
        if (connection != null && connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = commandText;
        command.CommandTimeout = 600;
        command.CommandType = commandType;
        return command;
    }

    [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities"),
     SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    private static SqlCommand CreateCommand(
        SqlConnection connection,
        CommandType commandType,
        string commandText,
        params SqlParameter[] parameters)
    {
        if (connection != null && connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }

        var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = commandText;
        command.CommandTimeout = 600;
        command.CommandType = commandType;

        SqlParameterCollection sqlParams = command.Parameters;

        // Append each parameter to the command
        if (parameters != null && parameters.Count() > 0)
        {
            for (int i = 0; i < parameters.Count(); i++)
            {
                sqlParams.Add(new SqlParameter(parameters[i].ParameterName, parameters[i].DbType));
                sqlParams[parameters[i].ParameterName].Value = parameters[i].Value;
            }
        }

        return command;
    }

    private static DataSet CreateDataSet(SqlCommand command)
    {
        using (var dataAdapter = new SqlDataAdapter(command))
        {
            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            return dataSet;
        }
    }

    private static DataTable CreateDataTable(SqlCommand command)
    {
        using (var dataAdapter = new SqlDataAdapter(command))
        {
            var dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            return dataTable;
        }
    }

    #endregion

    #region Exception functions

    /// <summary>
    /// Determines if the specified exception is the result of a foreign key violation.
    /// </summary>
    /// <param name="e">The exception to check.</param>
    /// <returns><code>true</code> if the exception is a foreign key violation, otherwise <code>false</code>.</returns>
    public static bool IsForeignKeyContraintException(Exception e)
    {
        var sqlex = e as SqlException;
        if (sqlex != null && sqlex.Number == 547)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the specified exception is the result of a unique constraint violation.
    /// </summary>
    /// <param name="e">The exception to check.</param>
    /// <returns><code>true</code> if the exception is a unique constraint violation, otherwise <code>false</code>.</returns>
    public static bool IsUniqueConstraintException(Exception e)
    {
        var sqlex = e as SqlException;
        if (sqlex != null && (sqlex.Number == 2627 || sqlex.Number == 2601))
        {
            return true;
        }

        return false;
    }

    #endregion

    #region DataSet and DataTable functions

    /// <summary>
    /// Returns the first (and only) DataTable found in the DataSet.
    /// </summary>
    /// <param name="dataSet">The DataSet to retrieve the DataTable from.</param>
    /// <returns>The first (and only) DataTable found in the DataSet, otherwise null.</returns>
    public static DataTable GetDataTable(DataSet dataSet)
    {
        if (dataSet != null && dataSet.Tables.Count == 1)
        {
            return dataSet.Tables[0];
        }

        return null;
    }

    /// <summary>
    /// Returns the first (and only) DataRow found in the DataTable.
    /// </summary>
    /// <param name="dataTable">The DataTable to retrieve the DataRow from.</param>
    /// <returns>The first (and only) DataRow found in the DataTable, otherwise null.</returns>
    public static DataRow GetDataRow(DataTable dataTable)
    {
        if (dataTable != null && dataTable.Rows.Count == 1)
        {
            return dataTable.Rows[0];
        }

        return null;
    }

    /// <summary>
    /// Returns the first (and only) DataRow found in the DataSet.
    /// </summary>
    /// <param name="dataSet">The DataSet to retrieve the DataRow from.</param>
    /// <returns>The first (and only) DataRow found in the DataSet, otherwise null.</returns>
    public static DataRow GetDataRow(DataSet dataSet)
    {
        DataTable dataTable = GetDataTable(dataSet);
        return GetDataRow(dataTable);
    }

    #endregion

    #region Field functions

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static bool GetBoolean(DataRow dataRow, string columnName, bool valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is bool)
            {
                return (bool) value;
            }
            else if (value is byte)
            {
                var byteValue = (byte) value;
                if (byteValue == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return Boolean.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static bool GetBoolean(SqlDataReader dataReader, string columnName, bool valueIfNull)
    {
        object value = GetObject(dataReader, columnName, valueIfNull);
        if (value != null)
        {
            if (value is bool)
            {
                return (bool) value;
            }
            else if (value is byte)
            {
                var byteValue = (byte) value;
                if (byteValue == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return Boolean.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
    public static byte GetByte(DataRow dataRow, string columnName, byte valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is byte)
            {
                return (byte) value;
            }
            else
            {
                return Byte.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
    public static byte GetByte(SqlDataReader dataReader, string columnName, byte valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is byte)
            {
                return (byte) value;
            }
            else
            {
                return Byte.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
    public static byte[] GetBytes(DataRow dataRow, string columnName, byte[] valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null && value is byte[])
        {
            return (byte[]) value;
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise the value specified by <code>valueIfNull</code>.</returns>
    public static byte[] GetBytes(SqlDataReader dataReader, string columnName, byte[] valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null && value is byte[])
        {
            return (byte[]) value;
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static DateTime GetDateTime(DataRow dataRow, string columnName, DateTime valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is DateTime)
            {
                return (DateTime) value;
            }
            else
            {
                return DateTime.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static DateTime GetDateTime(SqlDataReader dataReader, string columnName, DateTime valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is DateTime)
            {
                return (DateTime) value;
            }
            else
            {
                return DateTime.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static decimal GetDecimal(DataRow dataRow, string columnName, decimal valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is decimal)
            {
                return (decimal) value;
            }
            else
            {
                return Decimal.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static decimal GetDecimal(SqlDataReader dataReader, string columnName, decimal valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is decimal)
            {
                return (decimal) value;
            }
            else
            {
                return Decimal.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static double GetDouble(DataRow dataRow, string columnName, double valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is double)
            {
                return (double) value;
            }
            else
            {
                return Double.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static double GetDouble(SqlDataReader dataReader, string columnName, double valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is double)
            {
                return (double) value;
            }
            else
            {
                return Double.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static Guid GetGuid(DataRow dataRow, string columnName, Guid valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is Guid)
            {
                return (Guid) value;
            }
            else
            {
                return new Guid(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static Guid GetGuid(SqlDataReader dataReader, string columnName, Guid valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is Guid)
            {
                return (Guid) value;
            }
            else
            {
                return new Guid(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static float GetSingle(DataRow dataRow, string columnName, float valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is float)
            {
                return (float) value;
            }
            else
            {
                return Single.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static float GetSingle(SqlDataReader dataReader, string columnName, float valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is float)
            {
                return (float) value;
            }
            else
            {
                return Single.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static int GetInt32(DataRow dataRow, string columnName, int valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is int)
            {
                return (int) value;
            }
            else
            {
                return Int32.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static int GetInt32(SqlDataReader dataReader, string columnName, int valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is int)
            {
                return (int) value;
            }
            else
            {
                return Int32.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static long GetInt64(DataRow dataRow, string columnName, long valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is long)
            {
                return (long) value;
            }
            else
            {
                return Int64.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static long GetInt64(SqlDataReader dataReader, string columnName, long valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is long)
            {
                return (long) value;
            }
            else
            {
                return Int64.Parse(value.ToString());
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static object GetObject(DataRow dataRow, string columnName, object valueIfNull)
    {
        object value = dataRow[columnName];
        if (value != null && value != DBNull.Value)
        {
            return value;
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static object GetObject(SqlDataReader dataReader, string columnName, object valueIfNull)
    {
        object value = dataReader[columnName];
        if (value != null && value != DBNull.Value)
        {
            return value;
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a DataRow.
    /// </summary>
    /// <param name="dataRow">The DataRow to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static string GetString(DataRow dataRow, string columnName, string valueIfNull)
    {
        object value = GetObject(dataRow, columnName, null);
        if (value != null)
        {
            if (value is string)
            {
                return (string) value;
            }
            else
            {
                return value.ToString();
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    /// <summary>
    /// Attempts to extract the requested column value from a SqlDataReader.
    /// </summary>
    /// <param name="dataReader">The SqlDataReader to extract the column value from.</param>
    /// <param name="columnName">The name of the column to extract the value from.</param>
    /// <param name="valueIfNull">The value to return if the requested column value is null or DBNull.Value.</param>
    /// <returns>The value contained in the requested column if the value is not null or DBNull.Value, otherwise null.</returns>
    public static string GetString(SqlDataReader dataReader, string columnName, string valueIfNull)
    {
        object value = GetObject(dataReader, columnName, null);
        if (value != null)
        {
            if (value is string)
            {
                return (string) value;
            }
            else
            {
                return value.ToString();
            }
        }
        else
        {
            return valueIfNull;
        }
    }

    #endregion
}
