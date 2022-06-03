using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;

using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;

//
//  2022-05-23  Mark Stega
//              Created
//

namespace ICEBG.DataTier.Interfaces;

public interface iConfigurationClient
{
    Task<ServiceResult<Configuration_DD>> SelectAsync(
        string id,
        Metadata header);
    Task<ServiceResult<List<Configuration_DD>>> SelectAllAsync(
        Metadata header);
    Task<ServiceResult<string>> UpsertAsync(
        Configuration_DD Configuration,
        Metadata header);
}

