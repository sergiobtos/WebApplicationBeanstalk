#pragma checksum "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\Home\AddComment.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9d99553e1c4bbdd84a5e7962edd516cc2045e71f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_AddComment), @"mvc.1.0.view", @"/Views/Home/AddComment.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/AddComment.cshtml", typeof(AspNetCore.Views_Home_AddComment))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\_ViewImports.cshtml"
using WebApplicationBeanstalk;

#line default
#line hidden
#line 2 "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\_ViewImports.cshtml"
using WebApplicationBeanstalk.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9d99553e1c4bbdd84a5e7962edd516cc2045e71f", @"/Views/Home/AddComment.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7d6ccdd6057bc5f42348ac6ce4f0d25e378de8f9", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_AddComment : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MovieXRating>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "AddComment", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\Home\AddComment.cshtml"
  
    ViewData["Title"] = "AddComment";

#line default
#line hidden
            BeginContext(67, 35, true);
            WriteLiteral("\r\n<h2>Add Comment Page</h2>\r\n\r\n\r\n\r\n");
            EndContext();
            BeginContext(102, 826, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b636e4e54b1241b4af075c23a872b584", async() => {
                BeginContext(146, 198, true);
                WriteLiteral("\r\n    <div class=\"input-group\">\r\n        <div class=\"input-group-prepend\">\r\n            <span class=\"input-group-text\">Type your comment here</span>\r\n        </div>\r\n        <textarea name=\"comment\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 344, "\"", 373, 1);
#line 15 "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\Home\AddComment.cshtml"
WriteAttributeValue("", 352, Model.Rating.Comment, 352, 21, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(374, 272, true);
                WriteLiteral(@" class=""form-control"" aria-label=""With textarea""></textarea>
    </div>
    <div class=""input-group"">
        <div class=""input-group-prepend"">
            <span class=""input-group-text"">Type your rate from 1 to 10</span>
        </div>
        <textarea name=""rate""");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 646, "\"", 672, 1);
#line 21 "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\Home\AddComment.cshtml"
WriteAttributeValue("", 654, Model.Rating.Rate, 654, 18, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(673, 112, true);
                WriteLiteral(" class=\"form-control\" aria-label=\"With textarea\"></textarea>\r\n    </div>\r\n     <input type=\"hidden\" name=\"email\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 785, "\"", 810, 1);
#line 23 "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\Home\AddComment.cshtml"
WriteAttributeValue("", 793, Model.User.Email, 793, 17, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(811, 44, true);
                WriteLiteral(" />\r\n    <input type=\"hidden\" name=\"movieId\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 855, "\"", 878, 1);
#line 24 "C:\Comp306_AWSAmazon\Lab3\Code\WebApplicationBeanstalk\Views\Home\AddComment.cshtml"
WriteAttributeValue("", 863, Model.Movie.Id, 863, 15, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(879, 42, true);
                WriteLiteral(" />\r\n    <button type=\"Submit\"></button>\r\n");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(928, 13, true);
            WriteLiteral("\r\n\r\n \r\n\r\n\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MovieXRating> Html { get; private set; }
    }
}
#pragma warning restore 1591
