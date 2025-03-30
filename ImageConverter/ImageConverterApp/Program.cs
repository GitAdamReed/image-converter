using System.Drawing.Imaging;

namespace ImageConverterApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ImageConverter.ConvertImage("s2_spatial_anomoly.jxr", "new_png_image_1.png");
            //ImageConverter.ConvertJxrImage("s2_spatial_anomoly_1.jxr", "converted_jxr_1.bmp");
            //ImageConverter.ConvertJxrToPng("s2_spatial_anomoly_1.jxr", "converted_jxr_1.png");
            ImageConverter.ConvertJxrImage("s2_fault.jxr", "s2_fault.png", ImageFormat.Png);
        }
    }
}
