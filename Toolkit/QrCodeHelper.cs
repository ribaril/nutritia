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
    /// <summary>
    /// Classe statique haut niveau aidant l'utilisation de la librairie QrCode.Net
    /// </summary>
    public static class QrCodeHelper
    {
        /// <summary>
        /// Méthode retournant un objet d'image Bitmap d'un code QR
        /// </summary>
        /// <param name="text">Le texte à encoder</param>
        /// <param name="errorCorrection">Le niveau de correction d'erreur choisi. Détermine le pourcentage de la structure du codeQR est utilisé pour corriger les erreurs.</param>
        /// <returns>Objet BitmapImage du codeQR</returns>
        public static BitmapImage getQrBitmap(string text, ErrorCorrectionLevel errorCorrection = ErrorCorrectionLevel.M)
        {
            QrCode qrCode;
            QrEncoder qrEncoder = new QrEncoder(errorCorrection);
            //Peut techniquement échouer et le cas n'est pas géré.
            qrEncoder.TryEncode(text, out qrCode);

            //Détermine l'épaisseur des traits, la couleur de fond et la couleur du code QR.
            GraphicsRenderer gRenderer = new GraphicsRenderer(
                new FixedModuleSize(2, QuietZoneModules.Two),
                Brushes.Black, Brushes.White);
            BitmapImage bi = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                //Utilise les bytes du codeQR encodé qui se trouve dans l'objet qrCode dans un MemoryStream pour faire une BitmapImage.
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
