using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bpac;

namespace Touchstone_Brother_IpP.Models
{
    public class PrintManagement
    {
        private static string DataFolder = PDFManagment.DataFolder;
        private static string SourceFolder = PDFManagment.SourceFolder;
        private static string LabelLocation = FindLBXFile(DataFolder, "Label.lbx");

        public static string FindLBXFile(string _Directory, string filename)
        {
            var file = Directory.GetFiles(_Directory, filename, SearchOption.AllDirectories).FirstOrDefault();
            if (file != null)
                return file;
            return null;
        }


        public void Initialize()
        {

        }


        public void PrinterSetup()
        {

        }

        public void Print(TLabel tLabel)
        {
            DocumentClass document = new DocumentClass();
            document.Open(LabelLocation);
            var barcodeIndex = document.GetBarcodeIndex("Barcode");

            #region Document Object Text
            document.GetObject("Name").Text = tLabel.Name;
            document.GetObject("Address").Text = tLabel.Address;
            document.SetBarcodeData(barcodeIndex, tLabel.Barcode);
            document.GetObject("DeliveryDate").Text = tLabel.DeliveryDate;
            document.GetObject("ConsignmentNumber").Text = tLabel.ConsignmentNumber;
            document.GetObject("PostCode").Text = tLabel.PostCode;
            document.GetObject("Telephone").Text = tLabel.Telephone;
            document.GetObject("Location").Text = tLabel.Location;
            document.GetObject("LocationNumber").Text = tLabel.LocationNumber;
            document.GetObject("ParcelNumber").Text = tLabel.ParcelNumber;
            #endregion

            document.StartPrint((tLabel.Name + " Print Job"), PrintOptionConstants.bpoDefault);
            document.PrintOut(1, PrintOptionConstants.bpoDefault);
            int ErrorCode = document.ErrorCode;

            Console.WriteLine("Error Code > " + ErrorCode);

            document.EndPrint();
            document.Close();
        }
    }
}
