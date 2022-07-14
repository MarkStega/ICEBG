using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;

using Microsoft.Extensions.Logging;

using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.HelperClasses;
using ICEBG.DataTier.Interfaces;
using ICEBG.SystemFramework;

//
//  2022-05-23  Mark Stega
//              Created
//

namespace ICEBG.DataTier.gRPCClient
{
    public class ConfigurationClientGRPC : iConfigurationClient
    {
        private ILogger<LoggingFramework> pLogger { get; set; }
        private ConfigurationProto.ConfigurationProtoClient pConfigurationProtoClient { get; set; }
        private ConfigurationClientGRPC() { }
        public ConfigurationClientGRPC(
            ILogger<LoggingFramework> logger,
            ConfigurationProto.ConfigurationProtoClient protoClient)
        {
            pLogger = logger;
            pConfigurationProtoClient = protoClient;
            pLogger.LogDebug("ConfigurationClientGRPC constructor");
        }

        public async Task<ServiceResult<Configuration_DD>> SelectAsync(
            string requestedId)
        {
            try
            {
                var request = new ConfigurationSelectRequest
                {
                    Id = requestedId
                };
                var reply = await pConfigurationProtoClient.SelectAsync(request);

                if (reply.SuccessIndicator)
                {
                    var configuration = new Configuration_DD(
                        reply.ReturnedConfiguration.Id,
                        reply.ReturnedConfiguration.Configuration);

                    return new ServiceResult<Configuration_DD>
                        (result: configuration, success: true);
                }
                else
                {
                    return new ServiceResult<Configuration_DD>
                        (result: null, success: false, error: reply.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                var exception = ex.ToString() + "(InnerException: ";
                if (ex.InnerException == null)
                    exception += "NULL)";
                else
                    exception += ex.InnerException.ToString() + ")";

                return new ServiceResult<Configuration_DD>
                    (result: null, success: false, error: exception);
            }
        }

        public async Task<ServiceResult<List<Configuration_DD>>> SelectAllAsync(
            Metadata header)
        {
            try
            {
                var request = new ConfigurationSelectAllRequest { };
                var reply = await pConfigurationProtoClient.SelectAllAsync(request, header);

                if (reply.SuccessIndicator)
                {
                    var ConfigurationList = new List<Configuration_DD>();
                    foreach (var ConfigurationDD in reply.ConfigurationList)
                    {
                        var Configuration = new Configuration_DD(
                            ConfigurationDD.Id,
                            ConfigurationDD.Configuration);

                        ConfigurationList.Add(Configuration);
                    }
                    return new ServiceResult<List<Configuration_DD>>
                        (result: ConfigurationList, success: true);
                }
                else
                {
                    return new ServiceResult<List<Configuration_DD>>
                        (result: null, success: false, error: reply.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                var exception = ex.ToString() + "(InnerException: ";
                if (ex.InnerException == null)
                    exception += "NULL)";
                else
                    exception += ex.InnerException.ToString() + ")";

                return new ServiceResult<List<Configuration_DD>>
                    (result: null, success: false, error: exception);
            }
        }

        public async Task<ServiceResult<string>> UpsertAsync(
            Configuration_DD Configuration,
            Metadata header)
        {
            try
            {
                var ConfigurationDD = new ConfigurationDD
                {
                    Id = Configuration.Id,
                    Configuration = Configuration.Configuration
                };
                var request = new ConfigurationUpsertRequest { Configuration = ConfigurationDD };
                var reply = await pConfigurationProtoClient.UpsertAsync(request, header);

                if (reply.SuccessIndicator)
                {
                    return new ServiceResult<string>
                        (result: "", success: true);
                }
                else
                {
                    return new ServiceResult<string>
                        (result: null, success: false, error: reply.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                var exception = ex.ToString() + "(InnerException: ";
                if (ex.InnerException == null)
                    exception += "NULL)";
                else
                    exception += ex.InnerException.ToString() + ")";

                return new ServiceResult<string>
                    (result: null, success: false, error: exception);
            }
        }

    }
}

