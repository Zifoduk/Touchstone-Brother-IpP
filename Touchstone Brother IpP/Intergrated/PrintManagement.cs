using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bpac;
using System.Collections;
using System.Windows.Media;
using System.Windows.Forms;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class PrintManagement
    {
        private static string DataFolder = PDFManagement.DataFolder;
        private static string SourceFolder = PDFManagement.SourceFolder;
        private static string TemplatesFolder = PDFManagement.CheckDir(DataFolder + @"Templates\");
        private static string LabelLocation = FindLBXFile(TemplatesFolder, "Label.lbx");

        public static string FindLBXFile(string _Directory, string filename)
        {
            var file = Directory.GetFiles(_Directory, filename, SearchOption.AllDirectories).FirstOrDefault();
            if (file != null)
                return file;
            return null;
        }

        public PrintManagement()
        {
            CreateTemplateFiles(TemplatesFolder);
        }

        public static void CreateTemplateFiles(string _Directory)
        {
            if (!File.Exists(TemplatesFolder + @"Label.lbx"))
                File.WriteAllBytes(_Directory + "Label.lbx", Properties.Resources.Label);
        }


        private enum ToPrint { No, Yes, Retry };
        private ToPrint PrinterErrorCheck()
        {
            var error = -1;
            DocumentClass document = new DocumentClass();
            try
            {
                document.Open(LabelLocation);
            
                object[] printers = (object[])document.Printer.GetInstalledPrinters();
                Console.WriteLine("Printers Found");
                foreach (var printer in printers)
                    Console.WriteLine(printer);
                Console.WriteLine("End Printers Found");
                string TemplateMedia = document.GetMediaName();
                string PrinterMedia = document.Printer.GetMediaName();
                Console.WriteLine("Template Media: " + TemplateMedia + ", Printer Media: " + PrinterMedia);

                if(printers.Length == 0)
                {
                    error = 4258;
                    document.Close();
                    var result = PMessageBox.Show("Error: e" + error + "No printer aviliable" + ": refer wiki > e" + error,
                        "fault in Printers", MessageBoxButtons.RetryCancel);
                    if (result == DialogResult.Retry)                    
                        return ToPrint.Retry;                    
                    return ToPrint.No;
                }
                else if(printers.Length > 0)
                {
                    if (!document.Printer.IsPrinterOnline(printers[0].ToString()))
                    {
                        error = 4261;
                        document.Close();
                        var result = PMessageBox.Show("Error: e" + error + "\nTheres printer connected" +
                                "\nRefer wiki > e" + error, "No printer connection", MessageBoxButtons.RetryCancel);
                        if (result == DialogResult.Retry)
                            return ToPrint.Retry;
                        return ToPrint.No;
                    }
                    else if (TemplateMedia != PrinterMedia)
                    {
                        if (PrinterMedia == "")
                        {
                            error = 3465;
                            document.Close();
                            var result = PMessageBox.Show("Error: e" + error + "No media installed in connected printer" + ": refer wiki > e" + error,
                                "Check Printers", MessageBoxButtons.RetryCancel);
                            if (result == DialogResult.Retry)
                                return ToPrint.Retry;
                            return ToPrint.No;
                        }
                        else
                        {
                            error = 3461;
                            document.Close();
                            var result = PMessageBox.Show("Error: e" + error + "\nMedia installed in printer: " + PrinterMedia +
                                "\nIs not equal to document media: " + TemplateMedia + "\nMake sure media in printer is: " + TemplateMedia + "\nRefer wiki > e" + error,
                                "Check Media", MessageBoxButtons.RetryCancel);
                            if (result == DialogResult.Retry)
                                return ToPrint.Retry;
                            return ToPrint.No;
                        }
                    }
                    error = -1;
                    document.Close();
                    return ToPrint.Yes;
                }
                return ToPrint.No;
            }
            catch(IndexOutOfRangeException e)
            {
                Console.WriteLine(e);
                error = 4261;
                document.Close();
                var result = PMessageBox.Show("Error: e" + error + "No Brother printer connected found" + ": refer wiki > e" + error,
                    "Check Printers", MessageBoxButtons.RetryCancel);
                if (result == DialogResult.Retry)
                    return ToPrint.Retry;
                return ToPrint.No;
            }
        }

        void HandlePrinted(int status, object value) { Console.WriteLine("Printed event called"); }
        public void Print(TLabel tLabel)
        {
            var _toprint = PrinterErrorCheck();
            if (_toprint == ToPrint.Yes)
            {
                DocumentClass document = new DocumentClass();
                document.Printed += new IPrintEvents_PrintedEventHandler(HandlePrinted);
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
            else if(_toprint == ToPrint.Retry)
                restartPrint(tLabel);
            
        }
        public void restartPrint(TLabel tLabel)
        {
            Print(tLabel);
        }
    }
}
