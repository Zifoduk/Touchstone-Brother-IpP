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
using System.Windows.Controls;

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
        public static string DataFolder = UserRoamingDataFolder;
        public static string SourceFolder
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

        //=========================================================
        public void Initialize()
        {
            Flush();
            ReadData();
            ExtractData();
        }

        //=========================================================
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

        #endregion

        //=========================================================
        #region Read Data
        public void ReadData()
        {
            ArrayList arrayList = new ArrayList();

            DataTable results = new DataTable();
            string cn_string = Properties.Settings.Default.dbCustomersConnectionString;
            using (SqlConnection cn_connection = new SqlConnection(cn_string))
            using (SqlCommand command = new SqlCommand("SELECT * FROM tbl_Customers", cn_connection))
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
            {
                SqlConnection connect = new SqlConnection();
                connect.ConnectionString = cn_string;
                connect.Open();
                if (connect.State == ConnectionState.Open)
                    Console.WriteLine("lol");
                dataAdapter.Fill(results);

            }

            MainWindow.labelsPage.TestBinding.ItemsSource = results.DefaultView;

            var array = results.Rows[0].ItemArray.Select(x => x.ToString()).ToArray();
        }
        #endregion

        //=========================================================
        #region Extract Flushed PDF Data

        public List<TLabel> SourceLabels = new List<TLabel>();
        public string[] FileList(string path, string[] format)
        {
            var files = Directory.EnumerateFiles(path, "*.*").Where(f => format.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)).ToArray();
            return files;
        }
        public int[] DataPositions(string[] PageInformation)
        {
            int[] positions = new int[3];
            positions[0] = Array.FindIndex(PageInformation, row => row.Contains("___"));
            positions[1] = Array.FindIndex(PageInformation, row => row.Contains("Next Day"));
            positions[2] = PageInformation.Length;
            return positions;
        }
        public TLabel DataSorter(string[] PageInformation)
        {
            TLabel label = new TLabel();

            int[] dp = DataPositions(PageInformation);

            //temp ID
            label.ID = new Random().Next(1, 50);

            //Name
            label.Name = PageInformation[dp[0] + 2];

            //Address
            var tempAdArry = new List<string>();
            for (var i = (dp[0] + 3); i < (dp[1] - 2); i++)
                tempAdArry.Add(PageInformation[i]);
            label.Address = string.Join(",\r\n", tempAdArry.ToArray());

            //Barcode
            label.Barcode = PageInformation[dp[0] + 1];

            //Delivery Date
            label.DeliveryDate = PageInformation[dp[1] - 1];

            //Consignment Number
            label.ConsignmentNumber = PageInformation[dp[1] + 1];

            //PostCode
            label.PostCode = PageInformation[dp[1] + 3];

            //Telephone Number
            label.Telephone = PageInformation[dp[1] + 5];

            //Location
            label.Location = PageInformation[dp[2] - 2];


            //Location Number
            label.LocationNumber = PageInformation[dp[2] - 1];

            //Parcel Number
            label.ParcelNumber = PageInformation[dp[1] + 4];

            //Parcel Size
            label.ParcelSize = PageInformation[dp[2] - 3];

            //Weight
            label.Weight = PageInformation[dp[1] + 2];

            return label;
        }

        public void ExtractData()
        {
            var SourceFiles = FileList(SourceFolder, new string[] {".pdf"});
            foreach(var file in SourceFiles)
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (PdfReader reader = new PdfReader(fileStream))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        string[] PageLines = new string[] { };
                        var text = PdfTextExtractor.GetTextFromPage(reader, page, new SimpleTextExtractionStrategy());
                        PageLines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        TLabel tempLabel = DataSorter(PageLines);
                        SourceLabels.Add(tempLabel);
                    }
                }
        }
        #endregion

        //=========================================================
        public void PushToList()
        {
            MainWindow.labelsPage.TestBinding.ItemsSource = SourceLabels;
        }





        //Label
        //Class for Labels
        //---------------------------------------------------------

    }

    public class TLabel
    {
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
        public int ID { get; set; }
    }
}
