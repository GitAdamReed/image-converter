namespace ImageConverterApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ImageConverter ic = new();
            ic.SaveBmpImage("s2_spatial_anomoly.jxr", "new_bmp_image.bmp");
        }
    }
}
