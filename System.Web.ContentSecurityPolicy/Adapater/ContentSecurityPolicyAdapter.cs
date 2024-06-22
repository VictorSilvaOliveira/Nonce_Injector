using System.Reflection;
using System.Web.UI;
using System.Web.UI.Adapters;

namespace System.Web.ContentSecurityPolicy.Adapater
{
    internal class ContentSecurityPolicyAdapter: ControlAdapter
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var rareFieldProp = typeof(Control).GetProperty("RareFiedsEnsured", BindingFlags.NonPublic | BindingFlags.Instance);
            var getterRareFieldProp = rareFieldProp.GetGetMethod(nonPublic:true);
            var rareField = getterRareFieldProp.Invoke(this.Control, null);
            var getRenderMethod = rareField.GetType().GetField("RenderMethod");
            var originalCaller = (RenderMethod)getRenderMethod.GetValue(rareField);
            if (originalCaller != null)
            {
                var rareRenderWrapper = new RenderMethodWrapper(originalCaller, writer);
                Control.SetRenderMethodDelegate(rareRenderWrapper.Render);
            }

            base.Render(writer);
        }
    }
}
