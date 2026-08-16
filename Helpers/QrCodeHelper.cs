using QRCoder;

namespace smart_table.Helpers
{
    public class QrCodeHelper
    {
        public static byte[] GeneratePngQrCode(string inputText)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}
