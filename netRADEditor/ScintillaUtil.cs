using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ScintillaNET;

namespace netRADEditor
{
    public class ScintillaUtil
    {
        #region Styles

        public static void SetBackgroundColor(Scintilla scintilla, Color backColor)
        {
            INativeScintilla ns = scintilla.NativeInterface;
            ns.StyleSetBack(32, ScintillaNET.Utilities.ColorToRgb(backColor));
        }

        public static void SetForegroundColor(Scintilla scintilla, Color foreColor)
        {
            INativeScintilla ns = scintilla.NativeInterface;
            ns.StyleSetFore(32, ScintillaNET.Utilities.ColorToRgb(foreColor));
        }

        public static void SetFont(Scintilla scintilla, Font font)
        {
            scintilla.Styles[32].Font = font;
            scintilla.NativeInterface.StyleSetFont(32, font.Name);
            scintilla.NativeInterface.StyleClearAll();
        }

        public static void SetKeyWordsStyle(Scintilla scintilla)
        {
            const int SCE_C_COMMENT = 1;
            const int SCE_C_COMMENTLINE = 2;
            const int SCE_C_WORD = 5;
            const int SCE_C_STRING = 6;
            const int SCE_C_CHARACTER = 7;
            string keywordList = "if then else for to do while in and or not true false isin";
            //SCLEX_CPP
            scintilla.NativeInterface.SetLexer(3);
            scintilla.NativeInterface.SetKeywords(0, keywordList);
            scintilla.NativeInterface.StyleSetFore(SCE_C_WORD, 0x00FF0000); //Key
            scintilla.NativeInterface.StyleSetFore(SCE_C_STRING, 0x001515A3); //string
            scintilla.NativeInterface.StyleSetFore(SCE_C_CHARACTER, 0x00008000); //char
            scintilla.NativeInterface.StyleSetFore(SCE_C_COMMENT, 0x00008000); //Comments
            scintilla.NativeInterface.StyleSetFore(SCE_C_COMMENTLINE, 0x00008000); //Comments
        }

        public static void SetLineWrapping(Scintilla scintilla)
        {
            scintilla.LineWrapping.Mode = LineWrappingMode.Word;
            scintilla.LineWrapping.IndentMode = LineWrappingIndentMode.Same;
            scintilla.LineWrapping.IndentSize = 0;
        }

        public static void ClearLineWrapping(Scintilla scintilla)
        {
            scintilla.LineWrapping.Mode = LineWrappingMode.None;
        }

        public static void GetDetaultFont(Scintilla scintilla)
        {
            string fontName="";
            int fontSize = 0;
            FontStyle fontStyle = FontStyle.Regular;
            scintilla.NativeInterface.StyleGetFont(32, out fontName);
            fontSize = scintilla.NativeInterface.StyleGetSize(32);
        }

        #endregion

        #region Functional

        //public static void Copy(Scintilla scintilla)
        //{
        //    scintilla.Clipboard.Copy();
        //}

        //public static void Paste(Scintilla scintilla)
        //{
        //    scintilla.Clipboard.Paste();
        //}

        //public static void Cut(Scintilla scintilla)
        //{
        //    scintilla.Clipboard.Cut();
        //}

        /// <summary>
        /// Show the find/replace dialog. 
        /// </summary>
        /// <param name="scintilla"></param>
        public static void ShowFindWindow(Scintilla scintilla)
        {
            scintilla.FindReplace.ShowFind();
        }

        #endregion
    }
}
