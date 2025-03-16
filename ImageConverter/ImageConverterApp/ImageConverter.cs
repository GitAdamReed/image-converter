using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIC;

namespace ImageConverterApp
{
    public static class ImageConverter
    {
        private static readonly string imageFolderPath = "C:\\Users\\adamr\\Documents\\GitHub\\image-converter\\ImageConverter\\ImageConverterApp\\Images\\";
        
        public static void SaveBmpImage(string imagePath, string newPath)
        {
            using (var image = Image.Load(imageFolderPath + imagePath))
            {
                Console.WriteLine($"Saving bmp image to {imageFolderPath + newPath}");
                image.SaveAsBmp(imageFolderPath + newPath);
            }
        }

        private static byte[] GetImageBytes(string imagePath)
        {
            using (FileStream image = File.Open(imageFolderPath + imagePath, FileMode.Open))
            {
                byte[] buffer = new byte[image.Length];
                image.Read(buffer, 0, (Int32)image.Length);
                return buffer;
            }
        }

        public static void ConvertImage(string currImagePath, string newImagePath)
        {
            using (FileStream fileToConvert = File.Open(imageFolderPath + currImagePath, FileMode.Open))
            {
                Console.WriteLine($"Image size of {fileToConvert}: {fileToConvert.Length}");
                using (FileStream newFile = File.Create(imageFolderPath + newImagePath))
                {
                    fileToConvert.CopyTo(newFile);
                    Console.WriteLine($"Image size of {newFile}: {newFile.Length}");
                }
            }
        }
    }
}
