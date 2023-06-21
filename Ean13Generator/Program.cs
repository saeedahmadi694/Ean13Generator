

using Ean13Generator;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

Console.WriteLine("Hello, World!");

var font = SystemFonts.CreateFont("Arial", 20);
var barcode = new Barcode("9786229813997", true,130,50, font, $"ISBN : 9786229813997");
var img = barcode.GetImage();


//int width = img.Width;
//int height = img.Height + 50;
//Image<Rgba32> newImage = new Image<Rgba32>(width, height);
//newImage.Mutate(x => x.DrawImage(img, new Point(0, 50), 1));

//newImage.Mutate(x => x.DrawText("ISBN", font, Color.White, new PointF(10, 10)));

img.Save("b.jpg");

