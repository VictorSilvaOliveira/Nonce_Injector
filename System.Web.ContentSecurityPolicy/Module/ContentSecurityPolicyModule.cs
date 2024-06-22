using System.Text;
using System.Web.ContentSecurityPolicy.Adapater;
using System.Web.ContentSecurityPolicy.Writer;
using System.Web.UI;

namespace System.Web.ContentSecurityPolicy.Module
{
    public class ContentSecurityPolicyModule : IHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var browser = ((HttpApplication)sender).Context.Request.Browser;
            browser.Capabilities["tagWriter"] = typeof(ContentSecurityPolicyWriter).FullName;
            browser.HtmlTextWriter = typeof(ContentSecurityPolicyWriter).FullName;
            browser.Adapters[typeof(UI.Page).AssemblyQualifiedName] = typeof(ContentSecurityPolicyPageAdapter).FullName;
            browser.Adapters[typeof(UI.UserControl).AssemblyQualifiedName] = typeof(ContentSecurityPolicyAdapter).FullName;
            browser.Adapters[typeof(UI.HtmlControls.HtmlGenericControl).AssemblyQualifiedName] = typeof(ContentSecurityPolicyAdapter).FullName;
            browser.Adapters[typeof(UI.WebControls.Literal).AssemblyQualifiedName] = typeof(ContentSecurityPolicyAdapter).FullName;
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var httpApplication = (HttpApplication)sender;
            if (httpApplication != null)
            {
                var page = httpApplication?.Context?.CurrentHandler as Page;
                var pageAdapter = page?.PageAdapter as ContentSecurityPolicyPageAdapter;
                if (pageAdapter != null)
                {
                    FillNonces(httpApplication.Response, pageAdapter);
                }

            }
        }

        private void FillNonces(HttpResponse response, ContentSecurityPolicyPageAdapter pageAdapter)
        {
            var cspUpdateParams = new StringBuilder();


            response.Headers.Remove("Content-Security-Policy");
            response.Headers.Add("Content-Security-Policy", cspUpdateParams.ToString());
        }
    }
}
