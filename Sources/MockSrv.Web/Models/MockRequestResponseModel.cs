using MockSrv.Web.Extensions;
using MockSrv.Web.Resources;
using MockSrv.Web.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MockSrv.Web.Models;

public class MockRequestResponseModel : ICloneable
{
    public int Id { get; set; }

    [Display(Name = nameof(Messages.ApiNameDisplay), ResourceType = typeof(Messages))]
    public string ApiName { get; set; }

    [Display(Name = nameof(Messages.ApiRouteDisplay), ResourceType = typeof(Messages))]
    public string Route { get; set; }

    [Required]
    [RegularExpression(@"^\/[\w\.]+(\/[\w\.]+)+", ErrorMessageResourceName = "RequestPathInvalid", ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Messages.RequestPathDisplay), ResourceType = typeof(Messages))]
    public string RequestPath { get; set; }

    [Required]
    [Display(Name = nameof(Messages.RequestMethodDisplay), ResourceType = typeof(Messages))]
    public string RequestMethod { get; set; }

    [CustomValidation(typeof(MockRequestResponseModel), nameof(ValidateHeaders), ErrorMessageResourceName = "RequestHeadersInvalid", ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Messages.RequestHeadersDisplay), ResourceType = typeof(Messages))]
    public string RequestHeaders { get; set; }

    [RegularExpression(@"^\?[\w]+=[\w]+(&[\w]+=[\w]+)*$", ErrorMessageResourceName = "RequestQueryStringInvalid", ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Messages.RequestQueryStringDisplay), ResourceType = typeof(Messages))]
    public string RequestQueryString { get; set; }

    [Display(Name = nameof(Messages.RequestBodyDisplay), ResourceType = typeof(Messages))]
    public string RequestBody { get; set; }

    [CustomValidation(typeof(MockRequestResponseModel), nameof(ValidateBody), ErrorMessageResourceName = "ResponseBodyInvalid", ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Messages.ResponseBodyDisplay), ResourceType = typeof(Messages))]
    public string ResponseBody { get; set; }

    [Required]
    [Display(Name = nameof(Messages.ResponseStatusCodeDisplay), ResourceType = typeof(Messages))]
    [Range(100, 599)]
    public int ResponseStatusCode { get; set; }

    [Display(Name = nameof(Messages.ResponseContentTypeDisplay), ResourceType = typeof(Messages))]
    public string ResponseContentType { get; set; }

    [CustomValidation(typeof(MockRequestResponseModel), nameof(ValidateHeaders), ErrorMessageResourceName = "ResponseHeadersInvalid", ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Messages.ResponseHeadersDisplay), ResourceType = typeof(Messages))]
    public string ResponseHeaders { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public static ValidationResult ValidateBody(string value, ValidationContext context)
    {
        if (context.ObjectInstance is MockRequestResponseModel model) 
        {
            if (model.ResponseContentType != null)
            {
                if (model.ResponseContentType.Contains("json", StringComparison.CurrentCultureIgnoreCase) && !model.ResponseBody.IsValidJson())
                    return new ValidationResult(null, [nameof(ResponseBody), nameof(ResponseContentType)]);
                if (model.ResponseContentType.Contains("xml", StringComparison.CurrentCultureIgnoreCase) && !model.ResponseBody.IsValidXml())
                    return new ValidationResult(null, [nameof(ResponseBody), nameof(ResponseContentType)]);
            }
        }

        return ValidationResult.Success;
    }

    public static ValidationResult ValidateHeaders(string value, ValidationContext context)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var localizer = context.GetService(typeof(LocalisationService)) as LocalisationService;

            string pattern = @"^[\w-]+=[\w_]*(&[\w-]+=[\w]*)*$";
            bool isMatch = Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase);

            if (!isMatch)
                return new ValidationResult(null, [context.MemberName]);
            else
            {
                var keys = new List<string>();
                var kvs = value!.Split('&', StringSplitOptions.TrimEntries);
                foreach (var kv in kvs)
                {
                    var kvSepareted = kv.Split('=', StringSplitOptions.RemoveEmptyEntries);
                    keys.Add(kvSepareted[0]);
                }

                if (keys.Distinct().Count() != keys.Count)
                {
                    return new ValidationResult(localizer["MsgErrorHeadersKeysDuplicated"], [context.MemberName]);
                }
            }
        }

        return ValidationResult.Success;
    }
}