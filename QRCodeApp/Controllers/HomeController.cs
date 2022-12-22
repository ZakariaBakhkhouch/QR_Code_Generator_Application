using Microsoft.AspNetCore.Mvc;
using QRCodeApp.Models;
using System.Diagnostics;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace QRCodeApp.Controllers
{
    public static class BitmapExtension
    {
        public static byte[] CovertBitmapToByteArray(this Bitmap bitmap)
        {
            using(var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult CreateQRCode()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateQRCode(QRCodeApp.Models.QRCode model)
        {
            if (!ModelState.IsValid) return View();

            using(QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())

            using(QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(model.Text, QRCodeGenerator.ECCLevel.Q))

            using(QRCoder.QRCode qrCode = new QRCoder.QRCode(qRCodeData))
            {
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                byte[] bitmapArray = qrCodeImage.CovertBitmapToByteArray();
                string url = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bitmapArray));
                ViewBag.QRCodeImage = url;
            }

            return View();
        }
    }
}