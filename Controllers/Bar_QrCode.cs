using Microsoft.AspNetCore.Mvc;
using ZXing.Common;
using ZXing;
using ZXing.Windows.Compatibility;
using ZXing.QrCode;
using System.Text.Unicode;
using mvc4.Models;

namespace mvc4.Controllers
{
    public class Bar_QrCode : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //ZXing.Windows.Compatibility is only supported in windows;

        public IActionResult Barcode(string textData)
        {
            var barcodeWriter = new BarcodeWriter()
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 300,
                    Width = 800,
                    PureBarcode = false,
                    Margin = 10
                }
            };

            var bitmap = barcodeWriter.Write(textData);
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                return File(ms.ToArray(), "image/png");
            }
        }

        public IActionResult Qrcode(string inves)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions{
                    Width = 500,
                    Height = 500,
                    DisableECI = true,
                    CharacterSet = "UTF-8"
                }
            };
            var bitmap = barcodeWriter.Write(inves);
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            //    return File(ms.ToArray(), "image/png");
            //}
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }
    }
}
