using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraRichEdit.API.Native;
using System.Reflection;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraEditors;

namespace DevExpressSupport
{
    public partial class ReplaceForm : Form
    {
        List<ReplaceData> lstReplaces;

        DevExpress.XtraEditors.VScrollBar scrollbar1 = null;
        DevExpress.XtraEditors.VScrollBar scrollbar2 = null;

        public ReplaceForm()
        {
            InitializeComponent();

            scrollbar1 = richEditControl1.Controls[0] as DevExpress.XtraEditors.VScrollBar;
            if (scrollbar1 != null)
                scrollbar1.ValueChanged += new System.EventHandler(scrollbar1_ValueChanged);

            scrollbar2 = richEditControl2.Controls[0] as DevExpress.XtraEditors.VScrollBar;
            if (scrollbar2 != null)
                scrollbar2.ValueChanged += new System.EventHandler(scrollbar2_ValueChanged);


            lstReplaces = SerializationHelper.SerializeHelper.ReadFileToImport<List<ReplaceData>>(".\\Replace.xml");
            if (lstReplaces == null || lstReplaces.Count == 0)
            {
                txtStatus.Text = "Nothing to replace";
            }
        }

        private void richEditControl1_TextChanged(object sender, EventArgs e)
        {
            this.txtStatus.Text = "Typing..............................";
        }

        void scrollbar2_ValueChanged(object sender, System.EventArgs e)
        {
            int current = ComputeScrollValue(this.richEditControl2, this.richEditControl1);
            ScrollRichEditControlVertically(richEditControl1, current);
        }

        void scrollbar1_ValueChanged(object sender, System.EventArgs e)
        {
            int current = ComputeScrollValue(this.richEditControl1, this.richEditControl2);
            ScrollRichEditControlVertically(richEditControl2, current);
        }

        public void GetFromClipBoard()
        {
            richEditControl1.Text = Clipboard.GetText();
        }

        public void SetToClipboard()
        {
            Clipboard.SetText(richEditControl2.Text);
        }

        private void ReplaceForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 49 && e.Modifiers == Keys.Control)
            {
                GetFromClipBoard();
                txtStatus.Text = "Copy from clipboard to left window";
                return;
            }

            if (e.KeyValue == 50 && e.Modifiers == Keys.Control)
            {
                Process();
            }

            if (e.KeyValue == 51 && e.Modifiers == Keys.Control)
            {
                SetToClipboard();

                txtStatus.Text = "All in Clipboard !!!!!";
            }
        }

        private int ComputeScrollValue(RichEditControl rich1, RichEditControl rich2)
        {
            ScrollBarBase sbVertical1 = (ScrollBarBase)typeof(RichEditControl).InvokeMember("VerticalScrollBar",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, rich1, new object[] { });
            ScrollBarBase sbVertical2 = (ScrollBarBase)typeof(RichEditControl).InvokeMember("VerticalScrollBar",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, rich2, new object[] { });
            return sbVertical1.Value - sbVertical2.Value;
        }

        private void ScrollRichEditControlVertically(RichEditControl richEditControl, int scrollPosition)
        {
            RichEditViewVerticalScrollController controller = (RichEditViewVerticalScrollController)typeof(RichEditView).InvokeMember("VerticalScrollController", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, richEditControl.ActiveView, new object[] { });
            controller.ScrollByTopInvisibleHeightDelta(scrollPosition);
            typeof(RichEditView).InvokeMember("OnVerticalScroll", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, richEditControl.ActiveView, new object[] { });
        }

        private void Process()
        {
            string content = richEditControl1.Text;
            foreach (ReplaceData rd in lstReplaces)
            {
                if (rd.Type == "Replace")
                {
                    content = content.Replace(rd.OldString, rd.NewString);
                }
                else if (rd.Type == "NameSpace")
                {
                    if (content.IndexOf(rd.OldString) != -1)
                        content = content.Insert(0, rd.NewString + "\r\n");
                }
                else if (rd.Type == "Manager")
                {
                    if (content.IndexOf(rd.OldString) != -1)
                        content = content.Replace("private void InitializeComponent()",
                            "private ComponentResourceManager manager;\r\nprivate void InitializeComponent()");    
                }
            }

            this.richEditControl2.Text = content;
            Document document2 = this.richEditControl2.Document;

            Document document1 = this.richEditControl1.Document;

            foreach (ReplaceData rd in lstReplaces)
            {
                ISearchResult searchResult = document2.StartSearch(rd.NewString);                
                searchResult.FindNext();

                if (searchResult.CurrentResult != null)
                {
                    DocumentRange range = document2.CreateRange(searchResult.CurrentResult.Start,
                        searchResult.CurrentResult.Length);
                    //document2.CreateBookmark(range, "bm" + searchResult.CurrentResult.Start.ToString());
                    CharacterProperties prop = document2.BeginUpdateCharacters(range);
                    prop.ForeColor = Color.Red;
                    document2.EndUpdateCharacters(prop);


                    ////Old grid view
                    //document1.CaretPosition = document1.CreatePosition(0);
                    //ISearchResult searchResult1 = document1.StartSearch(rd.OldString);
                    //searchResult1.FindNext();

                    //if (searchResult1.CurrentResult != null)
                    //{
                    //    range = document1.CreateRange(searchResult1.CurrentResult.Start,
                    //        searchResult.CurrentResult.Length);
                    //    document1.CreateBookmark(range, "bm" + searchResult.CurrentResult.Start.ToString());
                    //    prop = document1.BeginUpdateCharacters(range);
                    //    prop.ForeColor = Color.Blue;
                    //    document1.EndUpdateCharacters(prop);
                    //}
                    //else
                    //{
                    //    range = document1.CreateRange(0, 0);
                    //    document1.CreateBookmark(range, "bm0");
                    //}
                }


            }
            txtStatus.Text = "DONE !!!";
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Process();
        }

        private void ReplaceForm_Activated(object sender, EventArgs e)
        {
            GetFromClipBoard();
            Process();
            SetToClipboard();
        }

        //int i = 0;
        private void btnNextDiff_Click(object sender, EventArgs e)
        {
            //if (i >= 0 && i <= richEditControl2.Document.Bookmarks.Count)
            //{
            //    Bookmark bookMark2 = richEditControl2.Document.Bookmarks[i];
            //    richEditControl2.Document.SelectBookmark(bookMark2);

            //    Bookmark bookMark1 = richEditControl1.Document.Bookmarks[i];
            //    richEditControl1.Document.SelectBookmark(bookMark1);
            //    i++;
            //}
            //bookMark.Range;
        }
    }
}