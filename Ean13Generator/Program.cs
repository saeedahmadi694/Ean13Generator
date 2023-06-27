using Ean13Generator;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

var barcode = new Barcode("9786005691757", true,130,50, $"ISBN : 978-600-5691-75-7");
var img = barcode.GetImage();

img.Save("b.jpg");

