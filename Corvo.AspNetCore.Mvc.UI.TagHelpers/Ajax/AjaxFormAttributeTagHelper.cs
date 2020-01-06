﻿//using Microsoft.AspNetCore.Mvc.TagHelpers;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Ardalis.GuardClauses;
using Corvo.AspNetCore.Mvc.UI.TagHelpers.Extensions;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;

namespace Corvo.AspNetCore.Mvc.UI.TagHelpers.Ajax
{
    /// <summary>
    /// Extends <see cref="FormTagHelper"/> element with ajax data-* attributes
    /// Mimics BeginForm from <see cref="https://github.com/mono/aspnetwebstack/blob/master/src/System.Web.Mvc/Ajax/AjaxExtensions.cs"/>
    /// In sync with <see cref="https://github.com/aspnet/jquery-ajax-unobtrusive/blob/master/src/jquery.unobtrusive-ajax.js"/>
    /// </summary>
    /// <remarks>
    /// Extend with additional functionalities like redirect on success, search form resubmit etc.
    /// </remarks>
    [HtmlTargetElement("form", Attributes = AjaxAttributeName)]
    public class AjaxFormAttributeTagHelper : FormTagHelper
    {
        #region Constants

        /// <summary>
        /// Attribute names cannot start with "data-*" prefix so we use "asp-*" instead.
        /// </summary>
        public const string AjaxAttributeName = "asp-ajax";

        public const string AjaxConfirmAttributeName = "asp-ajax-confirm";
        public const string AjaxMethodAttributeName = "asp-ajax-method";
        public const string AjaxUpdateElementAttributeName = "asp-ajax-update";
        public const string AjaxLoadingElementAttributeName = "asp-ajax-loading";
        public const string AjaxModeAttributeName = "asp-ajax-mode";
        public const string AjaxLoadingElemenDurationtAttributeName = "asp-ajax-loading-duration";
        public const string AjaxSucessAttributeName = "asp-ajax-success";
        public const string AjaxFailureAttributeName = "asp-ajax-failure";
        public const string AjaxBeginAttributeName = "asp-ajax-begin";
        public const string AjaxCompleteAttributeName = "asp-ajax-complete";
        public const string AjaxUrlAttributeName = "asp-ajax-url";

        #endregion Constants

        public AjaxFormAttributeTagHelper(IHtmlGenerator generator)
            : base(generator)
        {
        }

        /// <summary>
        /// Must be set to true to activate unobtrusive Ajax on the target element.
        /// Has to be present because we are only extending existing form tag helper ...
        /// </summary>
        [HtmlAttributeName(AjaxAttributeName)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the message to display in a confirmation window before a request is submitted.
        /// </summary>
        [HtmlAttributeName(AjaxConfirmAttributeName)]
        public string ConfirmMessage { get; set; }

        /// <summary>
        /// Gets or sets the HTTP request method ("Get" or "Post").
        /// </summary>
        [HtmlAttributeName(AjaxMethodAttributeName)]
        public string FormMethod { get; set; }

        /// <summary>
        /// Gets or sets the ID of the DOM element to update by using the response from the server.
        /// </summary>
        [HtmlAttributeName(AjaxUpdateElementAttributeName)]
        public string UpdateElementId { get; set; }

        /// <summary>
        /// Gets or sets the id attribute of an HTML element that is displayed while the Ajax function is loading.
        /// </summary>
        [HtmlAttributeName(AjaxLoadingElementAttributeName)]
        public string LoadingElementId { get; set; }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that controls the duration of the animation when showing or hiding the loading element.
        /// </summary>
        [HtmlAttributeName(AjaxLoadingElemenDurationtAttributeName)]
        public int LoadingElementDuration { get; set; }

        /// <summary>
        /// Gets or sets the mode that specifies how to insert the response into the target DOM element. Valid values are before, after and replace. Default is replace
        /// </summary>
        [HtmlAttributeName(AjaxModeAttributeName)]
        public InsertionMode InsertionMode { get; set; } = InsertionMode.Replace;

        /// <summary>
        /// Gets or sets the JavaScript function to call after the page is successfully updated.
        /// </summary>
        [HtmlAttributeName(AjaxSucessAttributeName)]
        public string OnSuccessMethod { get; set; }

        /// <summary>
        /// Gets or sets the JavaScript function to call if the page update fails.
        /// </summary>
        [HtmlAttributeName(AjaxFailureAttributeName)]
        public string OnFailureMethod { get; set; }

        /// <summary>
        /// Gets or sets the JavaScript function to call when response data has been instantiated but before the page is updated.
        /// </summary>
        [HtmlAttributeName(AjaxCompleteAttributeName)]
        public string OnCompleteMethod { get; set; }

        /// <summary>
        /// Gets or sets the name of the JavaScript function to call immediately before the page is updated.
        /// </summary>
        [HtmlAttributeName(AjaxBeginAttributeName)]
        public string OnBeginMethod { get; set; }

        /// <summary>
        /// Gets or sets the URL to make the request to.
        /// </summary>
        [HtmlAttributeName(AjaxBeginAttributeName)]
        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            Guard.Against.Null(context, nameof(context));
            Guard.Against.Null(output, nameof(output));

            // Ignore all attributes if data-ajax is set to false
            if (Enabled)
            {
                // Convert to string manually so we guarantee "true/false" values instead of "True/False".
                output.Attributes.Add("data-ajax", true.ToString().ToLowerInvariant());
                output.Attributes.AddIf(!string.IsNullOrEmpty(ConfirmMessage), "data-ajax-confirm", ConfirmMessage);
                output.Attributes.AddIf(!string.IsNullOrEmpty(FormMethod), "data-ajax-method", FormMethod);
                output.Attributes.AddIf(!string.IsNullOrEmpty(OnSuccessMethod), "data-ajax-success", OnSuccessMethod);
                output.Attributes.AddIf(!string.IsNullOrEmpty(OnFailureMethod), "data-ajax-failure", OnFailureMethod);
                output.Attributes.AddIf(!string.IsNullOrEmpty(OnBeginMethod), "data-ajax-begin", OnBeginMethod);
                output.Attributes.AddIf(!string.IsNullOrEmpty(OnCompleteMethod), "data-ajax-complete", OnCompleteMethod);
                output.Attributes.AddIf(!string.IsNullOrEmpty(Url), "data-ajax-url", Url);

                if (!string.IsNullOrEmpty(UpdateElementId))
                {
                    output.Attributes.Add(
                        "data-ajax-update",
                        UpdateElementId.StartsWith("#") ? UpdateElementId : "#" + UpdateElementId);

                    // Append insertion mode only if update element id is present
                    output.Attributes.Add("data-ajax-mode", InsertionMode.ToInsertionModeUnobtrusive());
                }

                if (!string.IsNullOrEmpty(LoadingElementId))
                {
                    output.Attributes.Add(
                        "data-ajax-loading",
                        LoadingElementId.StartsWith("#") ? LoadingElementId : "#" + LoadingElementId);

                    output.Attributes.AddIf(LoadingElementDuration > 0, "data-ajax-loading-duration", LoadingElementDuration);
                }
            }
        }
    }

    /// <summary>
    /// Used to determine where html content should be rendered
    /// </summary>
    public enum InsertionMode
    {
        Replace = 0,
        InsertBefore = 1,
        InsertAfter = 2
    }

    /// <summary>
    /// Extension methods for <see cref="InsertionMode"/> used only inside <see cref="AjaxFormAttributeTagHelper"/>
    /// </summary>
    internal static class InsertionModeExtensions
    {
        /// <summary>
        /// Returns proper insertion value based on enum <paramref name="value"/>
        /// </summary>
        /// <param name="value">Insertion mode</param>
        /// <returns></returns>
        public static string ToInsertionModeUnobtrusive(this InsertionMode value)
        {
            return value switch
            {
                InsertionMode.Replace => "replace",
                InsertionMode.InsertBefore => "before",
                InsertionMode.InsertAfter => "after",
                _ => ((int)value).ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}