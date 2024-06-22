using System.Web.UI;

namespace System.Web.ContentSecurityPolicy.Adapater
{
    internal class RenderMethodWrapper
    {
        private readonly RenderMethod _originalCaller;
        private readonly HtmlTextWriter _writer;

        public RenderMethodWrapper(RenderMethod originalCaller, HtmlTextWriter writer)
        {
            _originalCaller = originalCaller;
            _writer = writer;
        }

        internal void Render(HtmlTextWriter output, Control container)
        {
            _originalCaller.Invoke(_writer, container);
        }
    }
}
