using LunchRoulette.Web.Models;
using Microsoft.Extensions.Logging;

namespace LunchRoulette.Web.Utils.Logger
{
    public static class LoggerHelpers
    {
        public static void LogErrorModel(this ILogger logger, ErrorModel errorModel)
        {
            errorModel.ErrorMessages.ForEach(x => logger.LogWarning(x));
        }

        public static void LogOk(this ILogger logger, object response)
        {
            logger.LogTrace($"Returning Ok({response})");
        }

        public static void LogBadRequest(this ILogger logger, object response)
        {
            logger.LogTrace($"Returning BadRequest({response})");
        }

        public static void LogNotFound(this ILogger logger, object response)
        {
            logger.LogTrace($"Returning NotFound({response})");
        }
    }
}