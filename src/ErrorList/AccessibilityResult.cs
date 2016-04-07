using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;

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
        public string Html { get; set; }
        public string Id { get; set; }
        public string Impact { get; set; }
        public List<string> Tags { get; set; }

        public __VSERRORCATEGORY GetSeverity()
        {
            switch (Impact)
            {
                case "critical":
                case "serious":
                    return __VSERRORCATEGORY.EC_ERROR;

                case "moderate":
                    return __VSERRORCATEGORY.EC_WARNING;
            }

            return __VSERRORCATEGORY.EC_MESSAGE;
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                SetLineAndColumn();
            }
        }

        private int _position = -1;
        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                SetLineAndColumn();
            }
        }

        [JsonIgnore]
        public int Line { get; set; }
        [JsonIgnore]
        public int Column { get; set; }

        private void SetLineAndColumn()
        {
            if (Line != 0 || Column != 0 || Position == -1 || string.IsNullOrEmpty(FileName))
                return;
            
            int lineCount = 0;
            int columnCount = 0;
            int bufferPos = 0;
            bool hasBackslashN = false;

            using (var reader = new StreamReader(FileName))
            {
                char[] buffer = new char[Position];
                reader.ReadBlock(buffer, 0, Position);

                while (bufferPos < Position) 
                {
                    if (buffer[bufferPos] == '\r')
                    {
                        lineCount++;
                        columnCount = 0;
                    }
                    else if (buffer[bufferPos] == '\n')
                    {
                        hasBackslashN = true;
                    }

                    columnCount++;
                    bufferPos++;
                }
            }

            Line = lineCount;
            Column = columnCount - (hasBackslashN ? 1 : 0);
        }
    }
}
