using Ean13Generator;
using SixLabors.ImageSharp;

Console.WriteLine("dasdas");
var barcode = new Barcode("9786005691757", 2, 90, $"ISBN : 978-600-5691-75-7");
var img = barcode.GetImage();

img.Save("b.jpg");

