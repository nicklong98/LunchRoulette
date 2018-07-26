using System.Linq;

using LunchRoulette.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LunchRoulette.Web.Utils.ModelState
{
    public static class ModelStateHelpers
    {
        public static ErrorModel GenerateErrorModel(this ModelStateDictionary modelState)
        {
            var errorResponse = new ErrorModel();
            errorResponse.Message = "Invalid creation request";
            modelState.Values.Where(x => x.Errors.Any()).ToList()
                .ForEach(value => value.Errors.ToList()
                .ForEach(err => errorResponse.ErrorMessages.Add(err.ErrorMessage)));
            return errorResponse;
        }
    }
}