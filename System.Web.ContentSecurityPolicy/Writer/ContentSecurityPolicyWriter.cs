using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace System.Web.ContentSecurityPolicy.Writer
{
    internal class ContentSecurityPolicyWriter : XhtmlTextWriter
    {
        private static string SCRIPT_TAG = "script";
        private static string STYLE_TAG = "style";
        private static int NONCE_LENGTH = 42;
        public IList<string> ScriptNonces { get; private set; }
        public IList<string> StyleNonces { get; private set; }

        public ContentSecurityPolicyWriter(TextWriter writer) : this(writer, "\t")
        {
        }

        public ContentSecurityPolicyWriter(TextWriter writer, string tabString) : base(writer, tabString)
        {
            ScriptNonces = new List<string>();
            StyleNonces = new List<string>();
            AddRecognizedAttribute(STYLE_TAG, "nonce");
            AddRecognizedAttribute(SCRIPT_TAG, "nonce");
        }

        public override void RenderBeginTag(string tagName)
        {
            if (tagName.ToLower() == SCRIPT_TAG || tagName.ToLower() == STYLE_TAG)
            {
                AddAttribute("nonce", CreateNonce((tagName.ToLower() == SCRIPT_TAG ? NonceType.Script : NonceType.Style)));
            }
            base.RenderBeginTag(tagName);
        }

        public override void RenderBeginTag(HtmlTextWriterTag tagKey)
        {
            if (tagKey == HtmlTextWriterTag.Script || tagKey == HtmlTextWriterTag.Style)
            {
                AddAttribute("nonce", CreateNonce((tagKey == HtmlTextWriterTag.Script ? NonceType.Script : NonceType.Style)));
            }
            base.RenderBeginTag(tagKey);
        }

        public override void Write(string value)
        {
            value = FindInsertNonce(value, "<script", NonceType.Script);
            value = FindInsertNonce(value, "<style", NonceType.Style);
            base.Write(value);
        }

        private string FindInsertNonce(string value, string pattern, NonceType nonceType)
        {
            if (value.Contains(pattern))
            {
                var matches = Regex.Matches(value, pattern);
                var offset = 0;
                foreach (Match match in matches)
                {
                    value = AddNonce(value, offset, match, nonceType);
                    offset += NONCE_LENGTH;
                }
            }

            return value;
        }

        private string AddNonce(string value, int offset, Match match, NonceType nonceType)
        {
            var currentOffset = match.Index + match.Length + offset;
            var nonce = String.Format(" nonce=\"{0}\" ", CreateNonce(nonceType));
            return String.Format(
                "{0}{1}{2}",
                value.Substring(0, currentOffset),
                nonce,
                value.Substring(currentOffset)
            );
        }

        private string CreateNonce(NonceType nonceType)
        {
            var nonce = Guid.NewGuid().ToString().Replace("-", "");
            if (nonceType == NonceType.Script)
            {
                ScriptNonces.Add(nonce);
            }
            else
            {
                StyleNonces.Add(nonce);
            }

            return nonce;
        }
    }
}

