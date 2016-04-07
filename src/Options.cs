using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    public class Options: DialogPage
    {
        [Category("General")]
        [DisplayName("Enabled")]
        [Description("Enables the accessibility checker.")]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Category("Severity")]
        [DisplayName("Show warnings")]
        [Description("Shows accessibility errors classified as warnings.")]
        [DefaultValue(true)]
        public bool ShowWarnings { get; set; } = true;

        [Category("Severity")]
        [DisplayName("Show messages")]
        [Description("Shows accessibility errors classified as informational messages.")]
        [DefaultValue(false)]
        public bool ShowMessages { get; set; }


    }
}
