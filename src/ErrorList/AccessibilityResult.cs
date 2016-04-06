using System.Collections.Generic;

namespace WebAccessibilityChecker
{
    class AccessibilityResult
    {
        public string Url { get; set; }

        public List<Rule> Violations { get; set; }
        public List<Rule> Passes { get; set; }
    }

    class Rule
    {
        public string Description { get; set; }
        public string Help { get; set; }
        public string HelpUrl { get; set; }
        public string Id { get; set; }
        public string Impact { get; set; }

        public string FileName { get; set; }
    }
}
