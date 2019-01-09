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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class PrintManagement
    {
        private static string DataFolder = LocalFilesManagement.DataFolder;
        private static string SourceFolder = LocalFilesManagement.SourceFolder;
        private static string TemplatesFolder = LocalFilesManagement.CheckDir(DataFolder + @"Templates\");
        private static string LabelLocation = FindLBXFile(TemplatesFolder, "Label.lbx");
        private static string QRLocation = FindLBXFile(TemplatesFolder, "QR.lbx");

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
            {
                File.WriteAllBytes(_Directory + "Label.lbx", Properties.Resources.Label);
                LabelLocation = TemplatesFolder + @"Label.lbx";
            }
            if (!File.Exists(TemplatesFolder + @"QR.lbx"))
            {
                File.WriteAllBytes(_Directory + "QR.lbx", Properties.Resources.QRNormal);
                QRLocation = TemplatesFolder + @"Label.lbx";
            }
        }


        private enum ToPrint { No, Yes, Retry };
        private ToPrint PrinterErrorCheck(string Location)
        {
            var error = -1;
            DocumentClass document = new DocumentClass();
            try
            {
                document.Open(Location);
            
                object[] printers = (object[])document.Printer.GetInstalledPrinters();
                Console.WriteLine("Printers Found");
                foreach (var printer in printers)
                {
                    Console.WriteLine(printer);
                }
                Console.WriteLine("End Printers Found");
                string TemplateMedia = document.GetMediaName();
                string PrinterMedia = document.Printer.GetMediaName();
                Console.WriteLine("Template Media: " + TemplateMedia + ", Printer Media: " + PrinterMedia);

                if(printers.Length == 0)
                {
                    error = 4258;
                    document.Close();
                    var result = (DialogResult)PMessageBox.Show("Error: e" + error + "No printer aviliable" + ": refer wiki > e" + error,
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
                        var result = (DialogResult)PMessageBox.Show("Error: e" + error + "\nTheres printer connected" +
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
                            var result = (DialogResult)PMessageBox.Show("Error: e" + error + "No media installed in connected printer" + ": refer wiki > e" + error,
                                "Check Printers", MessageBoxButtons.RetryCancel);
                            if (result == DialogResult.Retry)
                                return ToPrint.Retry;
                            return ToPrint.No;
                        }
                        else
                        {
                            error = 3461;
                            document.Close();
                            var result = (DialogResult)PMessageBox.Show("Error: e" + error + "\nMedia installed in printer: " + PrinterMedia +
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
            catch(Exception e)
            {
                Console.WriteLine(e);
                error = 4261;
                document.Close();
                var result = (DialogResult)PMessageBox.Show("Error: e" + error + "No Brother printer connected found" + ": refer wiki > e" + error,
                    "Check Printers", MessageBoxButtons.RetryCancel);
                if (result == DialogResult.Retry)
                    return ToPrint.Retry;
                return ToPrint.No;
            }
        }

        void HandlePrinted(int status, object value) { Console.WriteLine("Printed event called"); }
        public void Print(TLabel tLabel)
        {
            var _toprint = PrinterErrorCheck(LabelLocation);
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
                document.GetObject("Weight").Text = tLabel.Weight;
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

        public void Print(Bitmap QRCode)
        {
            var _toprint = PrinterErrorCheck(QRLocation);
            if (_toprint == ToPrint.Yes)
            {
                DocumentClass document = new DocumentClass();
                document.Printed += new IPrintEvents_PrintedEventHandler(HandlePrinted);
                document.Open(QRLocation);

                #region Document Object Text
                using (MemoryStream ms = new MemoryStream())
                {
                    QRCode.Save(ms, ImageFormat.Bmp);
                    document.GetObject("QRImage").SetData(0, ms, 4);
                }
                #endregion

                document.StartPrint((QRCode.GetHashCode().ToString() + " Print Job"), PrintOptionConstants.bpoDefault);
                document.PrintOut(1, PrintOptionConstants.bpoDefault);
                int ErrorCode = document.ErrorCode;

                Console.WriteLine("Error Code > " + ErrorCode);

                document.EndPrint();
                document.Close();
            }
            else if (_toprint == ToPrint.Retry)
                restartPrint(QRCode);
        }
        public void Print(BitmapImage QRCode)
        {
            try
            {
                var _toprint = PrinterErrorCheck(QRLocation);
                if (_toprint == ToPrint.Yes)
                {
                    DocumentClass document = new DocumentClass();
                    document.Printed += new IPrintEvents_PrintedEventHandler(HandlePrinted);
                    document.Open(QRLocation);

                    #region Document Setting
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BitmapEncoder enc = new BmpBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(QRCode));
                        enc.Save(ms);
                        Bitmap bitmap = new Bitmap(ms);
                        App.LocalFilesManagement.Save(bitmap, null, SaveConfig.QRCode, true);
                        IObjects data = document.Objects;
                        foreach (IObject obj in data)
                        {
                            Console.WriteLine(obj.Name);
                        }
                        Console.WriteLine(data);
                        document.GetObject("QRImage").SetData(0, App.LocalFilesManagement.TempBmpFile, 4);
                        App.LocalFilesManagement.ClearTmp(true);
                    }
                    #endregion

                    document.StartPrint((QRCode.GetHashCode().ToString() + " Print Job"), PrintOptionConstants.bpoDefault);
                    document.PrintOut(1, PrintOptionConstants.bpoDefault);
                    int ErrorCode = document.ErrorCode;

                    Console.WriteLine("Error Code > " + ErrorCode);

                    document.EndPrint();
                    document.Close();
                }
                else if (_toprint == ToPrint.Retry)
                    restartPrint(QRCode);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void restartPrint(Bitmap QRCode)
        {
            Print(QRCode);
        }
        public void restartPrint(BitmapImage QRCode)
        {
            Print(QRCode);
        }
    }
}
