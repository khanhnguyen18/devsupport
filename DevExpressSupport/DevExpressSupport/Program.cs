using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DevExpressSupport
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ReplaceData rd = new ReplaceData();
            //rd.OldString = "old";
            //rd.NewString = "news";

            //List<ReplaceData> lstReplaces = new List<ReplaceData>();
            //lstReplaces.Add(rd);


            //SerializationHelper.SerializeHelper.WriteObjectToXML(".\\Replace.xml", lstReplaces);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ReplaceForm());

   
        }
    }
}