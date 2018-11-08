using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static QRCoder.PayloadGenerator;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class BarcodeManagement
    {

        public BarcodeManagement()
        {

        }

        public BitmapImage GenerateDisplayQR(string Data)
        {
            try
            {
                if (!App.LocalFilesManagement.CheckTempFile(Data, @".tmp"))
                {
                    QRCodeGenerator generator = new QRCodeGenerator();
                    QRCodeData qRCodeData = generator.CreateQrCode(Data, QRCodeGenerator.ECCLevel.H);
                    QRCode qRCode = new QRCode(qRCodeData);
                    Bitmap QRImage = qRCode.GetGraphic(40, Color.Black, Color.White, Properties.Resources.QRIcon, 30, 3, true);
                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        App.OfflineManagement.OfflineExport(OfflineConfig.CustomerQR, qrImage: QRImage, key: Data);
                        QRImage.Save(ms, ImageFormat.Bmp);
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        return bitmapImage;
                    }
                }
                else
                {
                    return App.LocalFilesManagement.DecodeTempQRFile(Data); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Unable to generate QR");
                Bitmap ErrorQR = Properties.Resources.ErrorQR;
                BitmapImage ErrorBitmapImage = new BitmapImage();
                using (MemoryStream mems = new MemoryStream())
                {
                    ErrorQR.Save(mems, ImageFormat.Bmp);
                    ErrorBitmapImage.BeginInit();
                    mems.Seek(0, SeekOrigin.Begin);
                    ErrorBitmapImage.StreamSource = mems;
                    ErrorBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    ErrorBitmapImage.EndInit();
                    return ErrorBitmapImage;
                }
            }
        }
        public BitmapImage GeneratePrintQR(string Data)
        {
            try
            {
                QRCodeGenerator generator = new QRCodeGenerator();
                QRCodeData qRCodeData = generator.CreateQrCode(Data, QRCodeGenerator.ECCLevel.H);
                QRCode qRCode = new QRCode(qRCodeData);
                Bitmap QRImage = qRCode.GetGraphic(40, Color.Black, Color.White, Properties.Resources.QRIcon, 20, 3, true);
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream())
                {
                    QRImage.Save(ms, ImageFormat.Bmp);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Unable to generate QR");
                //Fix this for failed print
                return null;
            }
        }
    }
}