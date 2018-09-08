using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Syroot.Windows.IO;
using System.Data.SqlClient;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Path = System.IO.Path;
using System.Collections;
using System.Windows.Threading;

namespace Touchstone_Brother_IpP.Models
{


    public class PDFManagment
    {

        private static string CheckDir(string dir)
        {

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
        private static string UserRoamingDataFolder
        {
            get
            { 
                string folderBase = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string dir = string.Format(@"{0}\{1}\Data\", folderBase, Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute)).OfType<AssemblyProductAttribute>().FirstOrDefault().Product);
                return CheckDir(dir);
            }
        }

        private static readonly string DownloadsFolder = KnownFolders.Downloads.Path;
        private  readonly string DataFolder = UserRoamingDataFolder;
        private static string SourceFolder
        {
            get
            {
                var folder = UserRoamingDataFolder + @"SourcePDFs\";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return folder;
            }
        }


        #region Flushing

        //Flushes all pdf which contain the words "@ipostparcels"
        //---------------------------------------------------------
        public void Flush()
        {

            List<string> pdfFiles = new List<string>();
            List<string> tempPdfFiles = new List<string>();
            
            foreach (var file in Directory.GetFiles(DownloadsFolder).Where(f => f.Contains(".pdf")).ToArray())
                tempPdfFiles.Add(file);

            foreach(var file in tempPdfFiles)
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (PdfReader reader = new PdfReader(fs))
                {
                    string[] textlines = new string[] { };
                    var text = PdfTextExtractor.GetTextFromPage(reader, 1, new SimpleTextExtractionStrategy());
                    textlines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(var line in textlines)
                        if (line.Contains("@ipostparcels"))
                            pdfFiles.Add(file);
                }

            pdfFiles.ForEach(file => File.Move(file, (SourceFolder + Path.GetFileName(file))));

        }
        
        #endregion /Flushing

        public void ReadData()
        {
            while (true)
            {
                ArrayList arrayList = new ArrayList();

                DataTable results = new DataTable();
                string cn_string = Properties.Settings.Default.dbCustomersConnectionString;
                using (SqlConnection cn_connection = new SqlConnection(cn_string))
                using (SqlCommand command = new SqlCommand("SELECT * FROM tbl_Customers", cn_connection))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {

                    dataAdapter.Fill(results);
                }

                Application.Current.Dispatcher.Invoke(new Action(() => { MainWindow.labelsPage.TestBinding.ItemsSource = results.DefaultView; }));
                Thread.Sleep(100);
            }
        }



        //Label
        //Class for Labels
        //---------------------------------------------------------
        public class Label
        {
            public string PDFtext { get; set; }
            public string[] ResultArr { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Barcode { get; set; }
            public string DeliveryDate { get; set; }
            public string ConsignmentNumber { get; set; }
            public string PostCode { get; set; }
            public string Telephone { get; set; }
            public string Location { get; set; }
            public string LocationNumber { get; set; }
            public string ParcelNumber { get; set; }
            public string ParcelSize { get; set; }
            public string Weight { get; set; }
        }

    }
}
