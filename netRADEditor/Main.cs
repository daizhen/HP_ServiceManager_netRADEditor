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
using System.IO;

namespace netRADEditor
{
    public partial class Main : Form
    {
        private const int SCE_C_COMMENT = 1;
        private const int SCE_C_COMMENTLINE = 2;
        private const int SCE_C_WORD = 5;
        private const int SCE_C_STRING = 6;
        private const int SCE_C_CHARACTER = 7;

        private Dictionary<string, EditorContext> Contexts = new Dictionary<string, EditorContext>();
        
        public Main()
        {
            InitializeComponent();
            AddNewTab();
        }

        /// <summary>
        /// Get the current active Scintilla editor
        /// </summary>
        private Scintilla CurrentEditor
        {
            get
            {
                TabPage currentTabPage = tabControlMain.SelectedTab;
                string currentTabKey = currentTabPage.Name;
                //Get current context;
                EditorContext context = Contexts[currentTabKey];
                return context.Editor;
            }
        }

        private Scintilla AddScintilla(Control parent)
        {
            Scintilla textBox = new Scintilla();
            //textBox.Location = new Point(20, 10);
            SetCodeViewerStyle(textBox);

            //Set margine to show line number.
            textBox.Margins[0].Width = 20;
            textBox.Dock = DockStyle.Fill;
            textBox.AllowDrop = true;
            parent.Controls.Add(textBox);
            return textBox;
        }

        /// <summary>
        /// Add a new tab to the tab page to the container.
        /// </summary>
        /// <returns></returns>
        private TabPage AddNewTab()
        {
            string newTabKey = Guid.NewGuid().ToString();
            tabControlMain.TabPages.Add(newTabKey, "Unsaved tab");
            Scintilla editorControl = AddScintilla(tabControlMain.TabPages[newTabKey]);

            EditorContext newContext = new EditorContext();
            newContext.Editor = editorControl;
            newContext.FileName = string.Empty;
            Contexts.Add(newTabKey, newContext);
            return tabControlMain.TabPages[newTabKey];
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
            ScintillaUtil.SetLineWrapping(viewer);
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

        private void FormatCode(Scintilla scintilla)
        {
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

        private void SerializeCode(Scintilla scintilla)
        {
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

        #region Events

        protected void viewer_FormatCodeClick(Object sender, EventArgs args)
        {
            Scintilla scintilla = (sender as MenuItem).Parent.GetContextMenu().SourceControl as Scintilla;
            FormatCode(scintilla);
        }

        protected void viewer_SerializeCodeClick(Object sender, EventArgs args)
        {
            Scintilla scintilla = (sender as MenuItem).Parent.GetContextMenu().SourceControl as Scintilla;
            SerializeCode(scintilla);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTab();
        }

        /// <summary>
        /// Open file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string content = ReadFile(fileDialog.FileName);

                //Create a new tab page.
                TabPage currentTabPage = AddNewTab();
                //Active this tab.
                tabControlMain.SelectedTab = currentTabPage;
                string currentTabKey = currentTabPage.Name;
                //Get current context;
                EditorContext context = Contexts[currentTabKey];
                //Insert the content into current editor control.
                if (context != null)
                {
                    context.Editor.InsertText(content);
                    context.FileName = fileDialog.FileName;
                    currentTabPage.Text = fileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// Close current tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get current tab page.
            TabPage currentTabPage = tabControlMain.SelectedTab;
            string currentTabKey = currentTabPage.Name;
            //Remove the tab from its container
            tabControlMain.Controls.RemoveByKey(currentTabKey);
            //Remove the context.
            Contexts.Remove(currentTabKey);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get current tab page.
            TabPage currentTabPage = tabControlMain.SelectedTab;
            string currentTabKey = currentTabPage.Name;
            //Get current context;
            EditorContext context = Contexts[currentTabKey];
            if (!string.IsNullOrEmpty(context.FileName))
            {
                SaveFile(context.FileName, context.Editor.Text);
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SaveFile(saveFileDialog.FileName, context.Editor.Text);
                    tabControlMain.SelectedTab.Text = saveFileDialog.FileName;
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            //Get current tab page.
            TabPage currentTabPage = tabControlMain.SelectedTab;
            string currentTabKey = currentTabPage.Name;
            //Get current context;
            EditorContext context = Contexts[currentTabKey];
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFile(saveFileDialog.FileName, context.Editor.Text);
                tabControlMain.SelectedTab.Text = saveFileDialog.FileName;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        /// <summary>
        /// Read a text file from local.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The content of the file.</returns>
        private string ReadFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                reader.Close();
                stream.Dispose();
                return content;
            }
            return string.Empty;
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        private void SaveFile(string fileName,string content)
        {
            FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Close();
            stream.Dispose();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //CurrentEditor.Clipboard.
            if (CurrentEditor.UndoRedo.CanUndo)
            {
                CurrentEditor.UndoRedo.Undo();
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (CurrentEditor.UndoRedo.CanRedo)
            {
                CurrentEditor.UndoRedo.Redo();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditor.Clipboard.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditor.Clipboard.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditor.Selection.Clear();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditor.Clipboard.Cut();
        }

        private void formatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatCode(CurrentEditor);
        }

        private void serializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerializeCode(CurrentEditor);
        }

        private void findReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentEditor.FindReplace.ShowFind();
        }

        private void toolStripMenuItemFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = CurrentEditor.Styles[32].Font;
            DialogResult result = fontDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ScintillaUtil.SetFont(CurrentEditor, fontDialog.Font);
            }

        }

        private void wrapLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wrapLineToolStripMenuItem.Checked)
            {
                wrapLineToolStripMenuItem.Checked = false;
                foreach (KeyValuePair<string, EditorContext> kvPair in Contexts)
                {
                    ScintillaUtil.ClearLineWrapping(kvPair.Value.Editor);
                }
            }
            else
            {
                wrapLineToolStripMenuItem.Checked = true;
                foreach (KeyValuePair<string, EditorContext> kvPair in Contexts)
                {
                    ScintillaUtil.SetLineWrapping(kvPair.Value.Editor);
                }
            }
        }
    }
}
