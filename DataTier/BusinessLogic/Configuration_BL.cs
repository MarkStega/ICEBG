
/******************************************************************************
Generated file - Created on 7/1/2022; Do not edit!
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;

namespace ICEBG.DataTier.BusinessLogic
{
	public partial class Configuration_BL
	{
		#region Properties

		protected SqlConnection m_SqlConnection { get; set; }

		#endregion

		#region Constructors

		public Configuration_BL(string connectionStringName)
		{
			ValidationUtility.ValidateArgument("connectionStringName", connectionStringName);

			m_SqlConnection = new SqlConnection(connectionStringName);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Saves a record to the Configuration table.
		/// </summary>
		public virtual void Insert(Configuration_DD configuration)
		{
			ValidationUtility.ValidateArgument("configuration", configuration);

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Id", configuration.Id),
				new SqlParameter("@Configuration", configuration.Configuration)
			};

			SqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, "Configuration_Insert", parameters);
		}

		/// <summary>
		/// Updates a record in the Configuration table.
		/// </summary>
		public virtual void Update(Configuration_DD configuration)
		{
			ValidationUtility.ValidateArgument("configuration", configuration);

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Id", configuration.Id),
				new SqlParameter("@Configuration", configuration.Configuration)
			};

			SqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, "Configuration_Update", parameters);
		}

		/// <summary>
		/// Updates or inserts a record in the Configuration table.
		/// </summary>
		public virtual void Upsert(Configuration_DD configuration)
		{
			ValidationUtility.ValidateArgument("configuration", configuration);

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Id", configuration.Id),
				new SqlParameter("@Configuration", configuration.Configuration)
			};

			SqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, "Configuration_Upsert", parameters);
		}

		/// <summary>
		/// Deletes a record from the Configuration table by its primary key.
		/// </summary>
		public virtual void Delete(string id)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Id", id)
			};

			SqlClientUtility.ExecuteNonQuery(m_SqlConnection, CommandType.StoredProcedure, "Configuration_Delete", parameters);
		}

		/// <summary>
		/// Selects a single record from the Configuration table.
		/// </summary>
		public virtual Configuration_DD Select(string id)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@Id", id)
			};

			using (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(m_SqlConnection, CommandType.StoredProcedure, "Configuration_Select", parameters))
			{
				if (dataReader.Read())
				{
					return MakeConfiguration(dataReader);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Selects all records from the Configuration table.
		/// </summary>
		public virtual List<Configuration_DD> SelectAll()
		{
			using (SqlDataReader dataReader = SqlClientUtility.ExecuteReader(m_SqlConnection, CommandType.StoredProcedure, "Configuration_SelectAll"))
			{
				List<Configuration_DD> configurationList = new List<Configuration_DD>();
				while (dataReader.Read())
				{
					Configuration_DD configuration = MakeConfiguration(dataReader);
					configurationList.Add(configuration);
				}

				return configurationList;
			}
		}

		/// <summary>
		/// Creates a new instance of the Configuration class and populates it with data from the specified SqlDataReader.
		/// </summary>
		protected virtual Configuration_DD MakeConfiguration(SqlDataReader dataReader)
		{
			Configuration_DD configuration = new Configuration_DD();
			configuration.Id = SqlClientUtility.GetString(dataReader, "Id", String.Empty);
			configuration.Configuration = SqlClientUtility.GetString(dataReader, "Configuration", String.Empty);

			return configuration;
		}

		#endregion
	}
}
