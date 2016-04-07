using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    public class Options: DialogPage
    {
        [Category(Vsix.Name)]
        [DisplayName("Enabled")]
        [Description("Enables the accessibility checker.")]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;
    }
}
