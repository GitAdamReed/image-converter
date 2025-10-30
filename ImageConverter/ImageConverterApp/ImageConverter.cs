using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIC;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
using SharpDX.WIC;

namespace ImageConverterApp
{
    public static class ImageConverter
    {
        private static readonly string _imageFolderPath = "C:\\Users\\adamr\\Documents\\GitHub\\image-converter\\ImageConverter\\ImageConverterApp\\Images\\";

        public static string? ImageFolderPath
        {
            get
            {
                return _imageFolderPath;
            }
        }

        private static byte[] GetImageBytes(string imagePath)
        {
            using (FileStream image = File.Open(_imageFolderPath + imagePath, FileMode.Open))
            {
                byte[] buffer = new byte[image.Length];
                image.Read(buffer, 0, (Int32)image.Length);
                return buffer;
            }
        }

        public static void ConvertImage(string currImagePath, string newImagePath)
        {
            using (FileStream fileToConvert = File.Open(_imageFolderPath + currImagePath, FileMode.Open))
            {
                Console.WriteLine($"Image size of {fileToConvert}: {fileToConvert.Length}");
                using (FileStream newFile = File.Create(_imageFolderPath + newImagePath))
                {
                    fileToConvert.CopyTo(newFile);
                    Console.WriteLine($"Image size of {newFile}: {newFile.Length}");
                }
            }
        }

        // Mostly sample code from WIC-DotNet GitHub repo
        public static void ConvertJxrImage(string currImagePath, string newImagePath)
        {
            var wic = WICImagingFactory.Create();

            using var fileStream = File.Open(_imageFolderPath + currImagePath, FileMode.Open, FileAccess.ReadWrite);

            var decoder = wic.CreateDecoderFromStream(fileStream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnDemand/*lossless decoding/encoding*/);

            var frame = decoder.GetFrame(0);

            using var memoryStream = new MemoryStream();

            var encoder = wic.CreateEncoder(decoder.GetContainerFormat());

            encoder.Initialize(memoryStream.AsCOMStream(), WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);

            var frameEncoder = encoder.CreateNewFrame();
            frameEncoder.Initialize(null);
            frameEncoder.SetSize(frame.GetSize()); // lossless decoding/encoding
            frameEncoder.SetResolution(frame.GetResolution()); // lossless decoding/encoding
            frameEncoder.SetPixelFormat(frame.GetPixelFormat()); // lossless decoding/encoding

            frameEncoder.AsMetadataBlockWriter().InitializeFromBlockReader(frame.AsMetadataBlockReader());

            var metadataWriter = frameEncoder.GetMetadataQueryWriter();

            metadataWriter.SetMetadataByName("System.Keywords", new string[] { "lossless", "re-encode", "with", "metadata" });

            frameEncoder.WriteSource(frame);

            frameEncoder.Commit();
            encoder.Commit();

            memoryStream.Flush();
            memoryStream.Position = 0;
            //fileStream.Position = 0;
            //fileStream.SetLength(0);

            using (FileStream targetFileStream = File.Create(_imageFolderPath + newImagePath))
            {
                memoryStream.CopyTo(targetFileStream);
            }

            Console.WriteLine("Image successfully lossless re-encoded with metadata!");
            
        }

        // ChatGPT provided method
        public static void ConvertJxrToPng(string inputPath, string outputPath)
        {
            ImagingFactory factory = new();

            // Open the .jxr image using WIC
            using (var decoder = new BitmapDecoder(factory, _imageFolderPath + inputPath, DecodeOptions.CacheOnDemand))
            using (var frame = decoder.GetFrame(0))
            using (var converter = new FormatConverter(factory))
            {
                // Disable automatic color management
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA,
                    BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);


                // Create a standard 8-bit PNG
                using (var bitmap = new System.Drawing.Bitmap(converter.Size.Width, converter.Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    converter.CopyPixels(data.Stride, data.Scan0, data.Height * data.Stride);
                    bitmap.UnlockBits(data);

                    // Apply gamma correction to the image to make it a bit darker
                    var adjustedBitmap = ApplyGammaCorrection(bitmap, 2.2f);

                    adjustedBitmap.Save(_imageFolderPath + outputPath, ImageFormat.Png);
                }
            }

            Console.WriteLine("Conversion completed successfully!");
        }

        public static void ConvertJxrImage(string inputPath, string outputPath, ImageFormat targetFormat, float gammaCorrection = 0)
        {
            ImagingFactory factory = new();

            // Open the .jxr image using WIC
            using (var decoder = new BitmapDecoder(factory, _imageFolderPath + inputPath, DecodeOptions.CacheOnDemand))
            using (var frame = decoder.GetFrame(0))
            using (var converter = new FormatConverter(factory))
            {
                // Disable automatic color management
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA,
                    BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);


                // Create a standard 8-bit PNG
                using (var bitmap = new System.Drawing.Bitmap(converter.Size.Width, converter.Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    converter.CopyPixels(data.Stride, data.Scan0, data.Height * data.Stride);
                    bitmap.UnlockBits(data);

                    if (gammaCorrection != 0)
                    {
                        // Apply gamma correction to the image
                        var adjustedBitmap = ApplyGammaCorrection(bitmap, gammaCorrection);
                        adjustedBitmap.Save(_imageFolderPath + outputPath, targetFormat);
                    }
                    else
                    {
                        bitmap.Save(_imageFolderPath + outputPath, targetFormat);
                    }
                }
            }

            Console.WriteLine("Conversion completed successfully!");
        }

        public static void ConvertJxrImageRecursive(string inputFolderPath, string outputFolderPath, ImageFormat targetFormat, float gammaCorrection = 0)
        {
            string[] filesToConvert = Directory.GetFiles(inputFolderPath, "*.jxr");
            foreach (var file in filesToConvert)
            {
                var fileName = file.Split("\\").LastOrDefault();
                Console.WriteLine($"Converting {fileName}...");
                try
                {
                    ConvertJxrImage($"test_dir\\{fileName}", $"{fileName.Split(".").First()}.png", targetFormat, gammaCorrection);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to convert image {fileName}");
                    Console.WriteLine(e.Message);
                }
            }
        }

        // ChatGPT provided method
        private static System.Drawing.Bitmap ApplyGammaCorrection(System.Drawing.Bitmap original, float gamma)
        {
            System.Drawing.Bitmap adjustedBitmap = new(original.Width, original.Height);
            using (Graphics g = Graphics.FromImage(adjustedBitmap))
            {
                ImageAttributes attributes = new();
                attributes.SetGamma(gamma);
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                    0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            return adjustedBitmap;
        }
    }
}
