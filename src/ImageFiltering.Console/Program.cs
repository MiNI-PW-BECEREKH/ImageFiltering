// See https://aka.ms/new-console-template for more information
using ImageFiltering.Common.Models;
using ImageFiltering.Dithering;
using ImageFiltering.Extensions;
using ImageFiltering.MedianCutQuantization;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;


var image = OpenImage();
var kernel = new Kernel(new[,] { { 1, 1, 1,1,1,1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 } }, new Point(4, 4));
var edgeDetection = new Kernel(new[,] { { 0, -1, 0}, { 0, 1, 0 }, { 0, 0, 0 } }, new Point(1, 1), 0);
var gaussianSmoothing = new Kernel(new[,] { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } }, new Point(1, 1));
var sharpenKernel = new Kernel(new[,] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } }, new Point(1, 1));
var embossKernel = new Kernel(new[,] { { -1, -1, -1 }, { 0, 1, 0 }, { 1, 1, 1 } }, new Point(1, 1));


var img = image.AverageDitheringLAB(4,4,4,2,true);
//image.ContrastEnhancement(-128);
SaveImage("lena.png", img);



static WriteableBitmap OpenImage()
{
    var bitmap = new BitmapImage(new Uri(@"C:\Users\suhey\Desktop\girl.jpg"));
    return new WriteableBitmap(bitmap);
}

static void SaveImage(string filename, BitmapSource image)
{
    if (filename != string.Empty)
    {
        using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate))
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
        }
    }
}

