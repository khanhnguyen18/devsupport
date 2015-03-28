using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraRichEdit.API.Native;

namespace DevExpressSupport
{
    public partial class ReplaceForm : Form
    {
        List<ReplaceData> lstReplaces;

        public ReplaceForm()
        {
            InitializeComponent();
            lstReplaces = SerializationHelper.SerializeHelper.ReadFileToImport<List<ReplaceData>>
                (".\\Replace.txt");

            if (lstReplaces == null || lstReplaces.Count == 0)
            {
                txtStatus.Text = "Nothing to replace";
            }

        }

        //Scroll 2 text editor

        private void richEditControl1_TextChanged(object sender, EventArgs e)
        {
            string content = richEditControl1.Text;
            foreach (ReplaceData rd in lstReplaces)
            {
                content = content.Replace(rd.OldString,rd.NewString);
            }

            this.richEditControl2.Text = content;




            foreach (ReplaceData rd in lstReplaces)
            {
                if(content.IndexOf(rd.NewString) == -1) continue;

                Document document = this.richEditControl2.Document;
                DocumentPosition pos = document.CreatePosition(content.IndexOf(rd.NewString));
                DocumentRange range = document.CreateRange(pos, rd.NewString.Length);

                CharacterProperties prop = document.BeginUpdateCharacters(range);
                prop.ForeColor = Color.Red;
                document.EndUpdateCharacters(prop);
            }

        }
    }
}