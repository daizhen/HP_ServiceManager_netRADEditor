using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScintillaNET;
using SM.Smorgasbord.RADParser;

namespace netRADEditor
{
    public partial class Main : Form
    {
        private const int SCE_C_COMMENT = 1;
        private const int SCE_C_COMMENTLINE = 2;
        private const int SCE_C_WORD = 5;
        private const int SCE_C_STRING = 6;
        private const int SCE_C_CHARACTER = 7;

        public Main()
        {
            InitializeComponent();
            AddScintilla();
        }

        private void AddScintilla()
        {
            Scintilla textBox = new Scintilla();
            //textBox.Location = new Point(20, 10);
            SetCodeViewerStyle(textBox);
            textBox.Width = this.ClientRectangle.Width;
            textBox.Height = this.ClientRectangle.Height;
            this.Controls.Add(textBox);
        }

        /// <summary>
        /// Set the code style.
        /// </summary>
        /// <param name="viewer"></param>
        private void SetCodeViewerStyle(Scintilla viewer)
        {
            string keywordList = "if then else for to do while in and or not true false isin";

            //STYLE_DEFAULT
            viewer.NativeInterface.StyleSetSize(32, 12);
            viewer.NativeInterface.StyleSetFont(32, "Courier New");
            viewer.NativeInterface.StyleClearAll();
            //SCLEX_CPP
            viewer.NativeInterface.SetLexer(3);

            viewer.NativeInterface.SetKeywords(0, keywordList);
            viewer.NativeInterface.StyleSetFore(SCE_C_WORD, 0x00FF0000); //Key
            viewer.NativeInterface.StyleSetFore(SCE_C_STRING, 0x001515A3); //string
            viewer.NativeInterface.StyleSetFore(SCE_C_CHARACTER, 0x00008000); //char
            viewer.NativeInterface.StyleSetFore(SCE_C_COMMENT, 0x00008000); //Comments
            viewer.NativeInterface.StyleSetFore(SCE_C_COMMENTLINE, 0x00008000); //Comments
          
            MenuItem itemFormat = new MenuItem("Format");
            itemFormat.Click += new EventHandler(viewer_FormatCodeClick);

            MenuItem itemSerialize = new MenuItem("Serialize");
            itemSerialize.Click += new EventHandler(viewer_SerializeCodeClick);

            viewer.ContextMenu = new ContextMenu();
            viewer.ContextMenu.MenuItems.Add(itemFormat);
            viewer.ContextMenu.MenuItems.Add(itemSerialize);
        }

        protected void viewer_FormatCodeClick(Object sender, EventArgs args)
        {
            Scintilla scintilla = (sender as MenuItem).Parent.GetContextMenu().SourceControl as Scintilla;
           
            try
            {
                RADLexer lexer = new RADLexer(scintilla.Text);
                lexer.Build();
                RadParser parser = new RadParser(lexer.Tokens);
                string formatedStr = parser.ParseStatements().ToString(0);
                scintilla.Text = formatedStr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void viewer_SerializeCodeClick(Object sender, EventArgs args)
        {
            Scintilla scintilla = (sender as MenuItem).Parent.GetContextMenu().SourceControl as Scintilla;
            try
            {
                RADLexer lexer = new RADLexer(scintilla.Text);
                lexer.Build();
                RadParser parser = new RadParser(lexer.Tokens);
                string formatedStr = parser.ParseStatements().ToString();
                scintilla.Text = formatedStr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
