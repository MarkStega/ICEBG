
using System;

namespace ICEBG.DataTier.DataDefinitions
{
	public class StatisticsReport_DD
	{
        #region Properties

        public string pPiAverageSpan { get; set; }
        public string pPiHeartbeat { get; set; }
        public string pPiIterations { get; set; }
        public string pPiStartTime { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the StatisticsReport_DD class.
        /// </summary>
        public StatisticsReport_DD()
		{
		}

        /// <summary>
        /// Initializes a new instance of the StatisticsReport_DD class.
        /// </summary>
        public StatisticsReport_DD(string piAverage, string piHeart, string piIter, string piStart)
		{
            pPiAverageSpan = piAverage;
            pPiHeartbeat = piHeart;
            pPiIterations = piIter;
            pPiStartTime = piStart;
        }

        #endregion

    }
}
