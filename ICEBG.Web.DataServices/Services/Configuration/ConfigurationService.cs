using System;
using System.Threading.Tasks;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ICEBG.DataTier.BusinessLogic;
using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.gRPCClient;
using ICEBG.SystemFramework;
using ICEBG.AppConfig;

//
//  2022-05-24  Mark Stega
//              Created
//

namespace ICEBG.Web.DataServices;

public class ConfigurationService : ConfigurationProto.ConfigurationProtoBase
{
    #region Members
    protected IConfiguration pConfiguration { get; private set; }
    protected Configuration_BL pConfigurationBL { get; private set; }
    protected ILogger<LoggingFramework> pLogger { get; private set; }

    #endregion

    #region ctor
    public ConfigurationService(
        IConfiguration configuration,
        ILogger<LoggingFramework> logger)
    {
        pConfiguration = configuration;
        pConfigurationBL = new Configuration_BL(ApplicationConfiguration.pSqlConnectionString);
        pLogger = logger;
        pLogger.LogInformation("ConfigurationService ctor");
    }

    #endregion

    #region Select

    public override Task<ConfigurationSelectReply> Select(ConfigurationSelectRequest request, ServerCallContext context)
    {
        try
        {
            pLogger.LogInformation("Configuration.Select initiated.");
            var configuration = pConfigurationBL.Select(request.Id);
            if (configuration == null)
            {
                pLogger.LogInformation("   pConfigurationBL.Select failed with a null Configuration.");
                var badReply = new ConfigurationSelectReply
                {
                    SuccessIndicator = false,
                    ErrorMessage = "NULL Configuration returned."
                };
                return Task.FromResult(badReply);
            }
            else
            {
                pLogger.LogInformation("   pConfigurationBL.Select succeeded");

                var reply = new ConfigurationSelectReply();

                reply.SuccessIndicator = true;
                reply.ErrorMessage = "";

                var ConfigurationDD = new ConfigurationDD
                {
                    Id = configuration.Id,
                    Configuration = configuration.Configuration
                };
                reply.ReturnedConfiguration = ConfigurationDD;

                return Task.FromResult(reply);
            }
        }
        catch (Exception ex)
        {
            var error = "Exception in Configuration.SelectAll of " + ex.ToString();
            pLogger.LogError(error);
            var badReply = new ConfigurationSelectReply
            {
                SuccessIndicator = false,
                ErrorMessage = ex.ToString()
            };
            return Task.FromResult(badReply);
        }
    }

    #endregion

    #region SelectAll

    [Authorize]
    public override Task<ConfigurationSelectAllReply> SelectAll(ConfigurationSelectAllRequest request, ServerCallContext context)
    {
        try
        {
            pLogger.LogInformation("Configuration.SelectAll initiated.");
            var Configurations = pConfigurationBL.SelectAll();
            if (Configurations == null)
            {
                pLogger.LogInformation("   pConfigurationBL.SelectAll failed with a null Configuration list.");
                var badReply = new ConfigurationSelectAllReply
                {
                    SuccessIndicator = false,
                    ErrorMessage = "NULL Configuration array returned."
                };
                return Task.FromResult(badReply);
            }
            else
            {
                pLogger.LogInformation(
                    "   pConfigurationBL.SelectAll succeeded, returning " +
                    Configurations.Count.ToString() + " Configuration record(s)");

                var reply = new ConfigurationSelectAllReply();

                reply.SuccessIndicator = true;
                reply.ErrorMessage = "";

                foreach (var Configuration in Configurations)
                {
                    var ConfigurationDD = new ConfigurationDD
                    {
                        Id = Configuration.Id,
                        Configuration = Configuration.Configuration
                    };
                    reply.ConfigurationList.Add(ConfigurationDD);
                }

                return Task.FromResult(reply);
            }
        }
        catch (Exception ex)
        {
            var error = "Exception in Configuration.SelectAll of " + ex.ToString();
            pLogger.LogError(error);
            var badReply = new ConfigurationSelectAllReply
            {
                SuccessIndicator = false,
                ErrorMessage = ex.ToString()
            };
            return Task.FromResult(badReply);
        }
    }

    #endregion

    #region Upsert

    [Authorize]
    public override Task<ConfigurationUpsertReply> Upsert(ConfigurationUpsertRequest request, ServerCallContext context)
    {
        try
        {
            pLogger.LogInformation("Configuration.Upsert initiated.");

            var Configuration = new Configuration_DD(
                request.Configuration.Id,
                request.Configuration.Configuration);
            pConfigurationBL.Upsert(Configuration);
            pLogger.LogInformation("   pConfigurationBL.Upsert succeeded");

            var goodReply = new ConfigurationUpsertReply
            {
                SuccessIndicator = true
            };
            return Task.FromResult(goodReply);
        }
        catch (Exception ex)
        {
            var error = "Exception in Configuration.Upsert of " + ex.ToString();
            pLogger.LogError(error);
            var badReply = new ConfigurationUpsertReply
            {
                SuccessIndicator = false,
                ErrorMessage = ex.ToString()
            };
            return Task.FromResult(badReply);
        }
    }

    #endregion

}

