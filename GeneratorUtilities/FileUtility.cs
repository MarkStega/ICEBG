using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace ICEBG.GeneratorUtilities
{
    /// <summary>
    /// Provides utility functions for the data tier generator.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Creates the specified sub-directory, if it doesn't exist.
        /// </summary>
        /// <param name="name">The name of the sub-directory to be created.</param>
        public static void CreateSubDirectory(string name)
        {
            if (Directory.Exists(name) == false)
            {
                Directory.CreateDirectory(name);
            }
        }

        /// <summary>
        /// Creates the specified sub-directory, if it doesn't exist.
        /// </summary>
        /// <param name="name">The name of the sub-directory to be created.</param>
        /// <param name="deleteIfExists">Indicates if the directory should be deleted if it exists.</param>
        public static void CreateSubDirectory(string name, bool deleteIfExists)
        {
            if (deleteIfExists && Directory.Exists(name))
            {
                Directory.Delete(name, true);
            }

            Directory.CreateDirectory(name);
        }

    }
}
