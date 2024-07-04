using System.Collections.Generic;

//
//  2009-04-02  Mark Stega
//              (1) Lost in the mists of time is the origin of this code. I believe it was SharpCore
//                  which is only visible on the Codeplex archive site and dates from 2007
//

namespace ICEBG.GenerateDataTier
{
    /// <summary>
    /// Class that stores information for tables in a database.
    /// </summary>
    public class Table
    {
        private readonly List<Column> columns;
        private readonly Dictionary<string, List<Column>> foreignKeys;
        private readonly List<Column> primaryKeys;

        /// <summary>
        /// Default constructor.  All collections are initialized.
        /// </summary>
        public Table()
        {
            columns = new List<Column>();
            primaryKeys = new List<Column>();
            foreignKeys = new Dictionary<string, List<Column>>();
        }

        /// <summary>
        /// Contains the list of Column instances that define the table.
        /// </summary>
        public List<Column> Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// Contains the list of Column instances that define the table.  The Dictionary returned 
        /// is keyed on the foreign key name, and the value associated with the key is an 
        /// List of Column instances that compose the foreign key.
        /// </summary>
        public Dictionary<string, List<Column>> ForeignKeys
        {
            get { return foreignKeys; }
        }

        /// <summary>
        /// Name of the table.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contains the list of primary key Column instances that define the table.
        /// </summary>
        public List<Column> PrimaryKeys
        {
            get { return primaryKeys; }
        }
    }
}
