using System.Drawing;
using ZXing;
using ZXing.Common;

namespace WifiRemote
{
  /// <summary>
  /// Helper class for QR Barcodes
  /// </summary>
  public class QRCodeGenerator
  {
    /// <summary>
    /// Encodes the given message String as QR Barcode and returns the bitmap
    /// </summary>
    /// <param name="_message">Message that should be encoded</param>
    /// <returns>Bitmap that contains the QR Barcode</returns>
    internal static Bitmap Generate(string _message)
    {

      var writer = new BarcodeWriter()
      {
        Format = BarcodeFormat.QR_CODE,
        Options = new EncodingOptions()
        {
          Height = 400,
          Width = 400
        }
      };

      var bitmap = writer.Write(_message);
      return bitmap;
    }
  }
}
