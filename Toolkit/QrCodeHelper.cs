using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace Nutritia.Toolkit
{
    public static class QrCodeHelper
    {

        public static BitmapImage getQrBitmap(string text, ErrorCorrectionLevel errorCorrection = ErrorCorrectionLevel.M)
        {
            QrCode qrCode;
            QrEncoder qrEncoder = new QrEncoder(errorCorrection);
            qrEncoder.TryEncode(text, out qrCode);

            GraphicsRenderer gRenderer = new GraphicsRenderer(
                new FixedModuleSize(2, QuietZoneModules.Two),
                Brushes.Black, Brushes.White);
            BitmapImage bi = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                gRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = memoryStream;
                bi.EndInit();
            }
            return bi;
        }

    }
}
