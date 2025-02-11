using Project.Models.Enums_;
using Project.Models.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.QrCode.Internal;
using ZXing.Rendering;
using QRCode = QRCoder.QRCode;

namespace Infrastructure.QR_Generator
{
    public class QRGenerator
    {
        public static Bitmap CreatePlaceQR(PlaceDto dto, string imgPath)
        {
            var qrCode = QR_TypeExtension.ToQrCode(dto.GUID, QR_Type.Places);
            return Create(qrCode, imgPath);
        }
        public static Bitmap CreateProfileQR(UserDto dto)
        {
            var qrCode = QR_TypeExtension.ToQrCode(dto.GUID, QR_Type.Profile);
            return Create(qrCode);
        }


        private static Bitmap Create(string code, string imgPath)
        {
            var bw = new ZXing.BarcodeWriter();

            var encOptions = new ZXing.Common.EncodingOptions
            {
                Width = 1700,
                Height = 1700,
                Margin = 0,
                PureBarcode = false
            };

            encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

            bw.Renderer = new BitmapRenderer();
            bw.Options = encOptions;
            bw.Format = ZXing.BarcodeFormat.QR_CODE;
            Bitmap bm = bw.Write(code);
            Bitmap overlay = new Bitmap(imgPath);

            int deltaHeigth = bm.Height - overlay.Height;
            int deltaWidth = bm.Width - overlay.Width;

            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));

            return bm;
        }

        private static Bitmap Create(string code)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }
    }
}
