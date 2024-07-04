using Grpc.Core;

using ICEBG.AppConfig;
using ICEBG.DataTier.BusinessLogic;
using ICEBG.DataTier.DataDefinitions;
using ICEBG.DataTier.gRPCClient;
using ICEBG.SystemFramework;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Reflection;
using System.Threading.Tasks;

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

                var reply = new ConfigurationSelectReply
                {
                    SuccessIndicator = true,
                    ErrorMessage = "",
                    ServerVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.Split('+')[0]
                };

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

                var reply = new ConfigurationSelectAllReply
                {
                    SuccessIndicator = true,
                    ServerVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.Split('+')[0],
                    ErrorMessage = ""
                };

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
                "",
                request.Configuration.Configuration);
            pConfigurationBL.Upsert(Configuration);
            pLogger.LogInformation("   pConfigurationBL.Upsert succeeded");

            var goodReply = new ConfigurationUpsertReply
            {
                ServerVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.Split('+')[0],
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

    #region SelectStatistics

    public override Task<StatisticsReportReply> SelectStatisticsReport(StatisticsReportRequest request, ServerCallContext context)
    {
        try
        {
            pLogger.LogInformation("Configuration.StatisticsReport initiated.");

            var reply = new StatisticsReportReply
            {
                SuccessIndicator = true,
                ErrorMessage = "",
                ServerVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion.Split('+')[0]
            };

            var StatisticsReportDD = new StatisticsReportDD
            {
                PiAverageSpan = WorkerServiceReference.pConfigurationWorkerServiceReference.pPiAverageSpan.ToString(),
                PiHeartbeat = WorkerServiceReference.pConfigurationWorkerServiceReference.pPiHeartbeat.ToString(),
                PiIterations = WorkerServiceReference.pConfigurationWorkerServiceReference.pPiIterations.ToString(),
                PiStartTime = WorkerServiceReference.pConfigurationWorkerServiceReference.pPiStartTime.ToString(),
                SqlAverageSpan = WorkerServiceReference.pConfigurationWorkerServiceReference.pSqlAverageSpan.ToString(),
                SqlHeartbeat = WorkerServiceReference.pConfigurationWorkerServiceReference.pSqlHeartbeat.ToString(),
                SqlIterations = WorkerServiceReference.pConfigurationWorkerServiceReference.pSqlIterations.ToString(),
                SqlStartTime = WorkerServiceReference.pConfigurationWorkerServiceReference.pSqlStartTime.ToString()
            };
            reply.StatisticsReport = StatisticsReportDD;

            return Task.FromResult(reply);
        }
        catch (Exception ex)
        {
            var error = "Exception in Configuration.StatisticsReport of " + ex.ToString();
            pLogger.LogError(error);
            var badReply = new StatisticsReportReply
            {
                SuccessIndicator = false,
                ErrorMessage = ex.ToString()
            };
            return Task.FromResult(badReply);
        }
    }

    #endregion

}

