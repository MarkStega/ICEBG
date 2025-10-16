
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

        public string pSqlAverageSpan { get; set; }
        public string pSqlHeartbeat { get; set; }
        public string pSqlIterations { get; set; }
        public string pSqlStartTime { get; set; }

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
        public StatisticsReport_DD(
            string piAverage,
            string piHeart,
            string piIter,
            string piStart,
            string sqlAverage,
            string sqlHeart,
            string sqlIter,
            string sqlStart
            )
        {
            pPiAverageSpan = piAverage;
            pPiHeartbeat = piHeart;
            pPiIterations = piIter;
            pPiStartTime = piStart;

            pSqlAverageSpan = sqlAverage;
            pSqlHeartbeat = sqlHeart;
            pSqlIterations = sqlIter;
            pSqlStartTime = sqlStart;
        }

        #endregion

    }
}
