using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverterApp
{
    public class ImageConverter
    {
        private readonly string imageFolderPath = "C:\\Users\\adamr\\Documents\\GitHub\\image-converter\\ImageConverter\\ImageConverterApp\\Images\\";
        
        public ImageConverter()
        {
            Console.WriteLine("Creating new instance of ImageConverter class...");
        }
        
        public void SaveBmpImage(string imagePath, string newPath)
        {
            using (var image = Image.Load(imageFolderPath + imagePath))
            {
                Console.WriteLine($"Saving bmp image to {imageFolderPath + newPath}");
                image.SaveAsBmp(imageFolderPath + newPath);
            }
        }
    }
}
