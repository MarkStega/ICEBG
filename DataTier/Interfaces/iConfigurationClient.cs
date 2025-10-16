using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;

using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;

//
//  2022-05-23  Mark Stega
//              Created
//
//  2024-07-04  Mark Stega
//              Added StatisticsReportAsync
//

namespace ICEBG.DataTier.Interfaces;

public interface iConfigurationClient
{
    Task<ServiceResult<Configuration_DD>> SelectAsync(
        string id);
    Task<ServiceResult<List<Configuration_DD>>> SelectAllAsync(
        Metadata header);
    Task<ServiceResult<string>> UpsertAsync(
        Configuration_DD Configuration,
        Metadata header);
    Task<ServiceResult<StatisticsReport_DD>> StatisticsReportAsync(
        );
}

