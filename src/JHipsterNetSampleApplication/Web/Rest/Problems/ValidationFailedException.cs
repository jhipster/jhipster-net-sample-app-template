using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JHipsterNetSampleApplication.Web.Rest.Problems {
    //https://www.jerriepelser.com/blog/validation-response-aspnet-core-webapi/
    public class ValidationFailedProblem : ProblemDetailsException {
        public ValidationFailedProblem(ModelStateDictionary modelState) : base(new ValidationProblemDetails(modelState))
        {
        }
    }
}
