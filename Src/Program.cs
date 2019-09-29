using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFToMarkdown
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please include the slides filename as parameter");
                return;
            }

            MagickReadSettings settings = new MagickReadSettings();
            // Settings the density to 300 dpi will create an image with a better quality
            settings.Density = new Density(300, 300);

            // If a slides folder does not exist, create it
            Directory.CreateDirectory("Slides");

            var filename = Path.GetFileNameWithoutExtension(args[0]).Trim().Replace(" ", string.Empty);

            if (File.Exists($"{filename}_notes.md"))
            {
                Console.WriteLine("A lecture notes file for that set of slides already exists");
                return;
            }

            var notes = File.OpenWrite($"{filename}_notes.md");
            var newline = new UTF8Encoding(true).GetBytes(Environment.NewLine);

            using (MagickImageCollection images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(args[0], settings);
                
                int page = 1;
                foreach (MagickImage image in images)
                {
                    var file = new FileInfo($@"Slides\{filename}_slide_{page}.png");
                    image.Write(file, MagickFormat.Png);
                    byte[] info = new UTF8Encoding(true).GetBytes($@"![](Slides\{filename}_slide_{page}.png)");
                    notes.Write(info, 0, info.Length);
                    notes.Write(newline, 0, newline.Length);
                    notes.Write(newline, 0, newline.Length);
                    notes.Write(newline, 0, newline.Length);
                    notes.Write(newline, 0, newline.Length);
                    page++;
                }
            }

            notes.Close();
        }
    }
}
