
/******************************************************************************
Generated file - Created on 8/1/2023; Do not edit!
******************************************************************************/

using System;

namespace ICEBG.DataTier.DataDefinitions
{
	public class Configuration_DD
	{
		#region Properties

		public string Id { get; set; }
		public string ServerVersion { get; set; }
		public string Configuration { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Configuration_DD class.
		/// </summary>
		public Configuration_DD()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Configuration_DD class.
		/// </summary>
		public Configuration_DD(string id, string serverVersion, string configuration)
		{
			this.Id = id;
			this.ServerVersion = serverVersion;
			this.Configuration = configuration;
		}

		/// <summary>
		/// Initializes a new instance of the Configuration_DD class.
		/// </summary>
		public Configuration_DD(Configuration_DD classInstance)
		{
			this.Id = classInstance.Id;
			this.ServerVersion = classInstance.ServerVersion;
			this.Configuration = classInstance.Configuration;
		}

		#endregion

	}
}
