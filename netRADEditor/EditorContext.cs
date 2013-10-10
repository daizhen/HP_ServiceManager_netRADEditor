using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScintillaNET;

namespace netRADEditor
{
    /// <summary>
    /// Used to store the context of each editor/tab.
    /// </summary>
    public class EditorContext
    {
        /// <summary>
        /// Editor instance
        /// </summary>
        public Scintilla Editor
        {
            get;
            set;
        }

        /// <summary>
        /// The file name related to current editor.
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
    }
}
