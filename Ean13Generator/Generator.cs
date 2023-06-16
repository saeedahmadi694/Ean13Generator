using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Ean13Generator;

public static class Ean13Generator
{
    public static void Generator()
    {
        //var b= new Barcode()
        string ean13 = GenerateEan13();
        int width = 320;
        int height = 300;
        string imagePath = "barcode.jpg";
        var font = SystemFonts.CreateFont("Arial", 20);

        using (var image = new Image<Rgba32>(width, height, Color.White))
        {
            // Draw the barcode
            image.Mutate(ctx => DrawBarcode(ctx, ean13));

            // Add the human-readable text
            //image.Mutate(ctx => ctx
            //    .DrawText(ean13.Substring(0, 1), font, Color.Black, new PointF(2, 2))
            //    .DrawText(ean13.Substring(1, 6), font, Color.Black, new PointF(15, 2))
            //    .DrawText(ean13.Substring(7), font, Color.Black, new PointF(75, 2)));

            // Add the caption
            string caption = "ISBN: " + ean13;
            var captionFont = font;
            var captionSize = TextMeasurer.Measure(caption, new TextOptions(captionFont));
            var captionPosition = new PointF((width - captionSize.Width) / 2, height - captionSize.Height - 2);
            image.Mutate(ctx => ctx.DrawText(caption, captionFont, Color.Black, captionPosition));

            // Save the image
            image.Save(imagePath);
            Console.WriteLine("done");

        }
    }

    static string GenerateEan13()
    {
        // Generate a random 12-digit number
        //Random random = new Random();
        //int[] digits = Enumerable.Range(0, 12).Select(i => random.Next(0, 10)).ToArray();

        //// Calculate the check digit
        //int evenSum = digits.Where((d, i) => i % 2 == 0).Sum();
        //int oddSum = digits.Where((d, i) => i % 2 == 1).Sum();
        //int totalSum = evenSum + 3 * oddSum;
        //int checkDigit = (10 - (totalSum % 10)) % 10;

        //// Combine the digits and check digit to form the EAN-13 code
        //string code = string.Concat(digits.Select(d => d.ToString())) + checkDigit;

        return "9786229813997";
    }

    static void DrawBarcode(IImageProcessingContext ctx, string ean13)
    {
        int width = ctx.GetCurrentSize().Width;
        int height = ctx.GetCurrentSize().Height - 20;
        int thinWidth = 1;
        int thickWidth = 3;

        // Create a path builder for the barcode
        var pathBuilder = new PathBuilder();

        // Draw the start character
        pathBuilder.AddLine(0, 0, thinWidth, 0);
        pathBuilder.AddLine(thinWidth, 0, thinWidth, height);

        // Draw the left-hand side of the barcode
        int pos = thinWidth;
        for (int i = 0; i < 6; i++)
        {
            int digit = int.Parse(ean13[i].ToString());
            bool[] bars = Ean13LeftHandDigits[digit];
            for (int j = 0; j < bars.Length; j++)
            {
                int barWidth = bars[j] ? thickWidth : thinWidth;
                pathBuilder.AddLine(pos, 0, pos + barWidth, 0);
                pathBuilder.AddLine(pos + barWidth, 0, pos + barWidth, height);
                pos += barWidth;
            }
        }

        // Draw the center guard bars
        pathBuilder.AddLine(pos, 0, pos + thinWidth, 0);
        pathBuilder.AddLine(pos + thinWidth, 0, pos + thinWidth, height);
        pos += thinWidth;

        pathBuilder.AddLine(pos, 0, pos + thickWidth, 0);
        pathBuilder.AddLine(pos + thickWidth, 0, pos + thickWidth, height);
        pos += thickWidth;

        pathBuilder.AddLine(pos, 0, pos + thinWidth, 0);
        pathBuilder.AddLine(pos + thinWidth, 0, pos + thinWidth, height);
        pos += thinWidth;

        // Draw the right-hand side of the barcode
        for (int i = 6; i < 12; i++)
        {
            int digit = int.Parse(ean13[i].ToString());
            bool[] bars = Ean13RightHandDigits[digit];
            for (int j = 0; j < bars.Length; j++)
            {
                int barWidth = bars[j] ? thickWidth : thinWidth;
                pathBuilder.AddLine(pos, 0, pos + barWidth, 0);
                pathBuilder.AddLine(pos + barWidth, 0, pos + barWidth, height);
                pos += barWidth;
            }
        }

        // Draw the end character
        pathBuilder.AddLine(pos, 0, pos + thinWidth, 0);
        pathBuilder.AddLine(pos + thinWidth, 0, pos + thinWidth, height);

        // Create a graphics path from the path builder
        var graphicsPath = pathBuilder.Build();

        // Draw the path onto the image
        ctx.Draw(new DrawingOptions(), new Pen(Color.Black, 5), graphicsPath);
    }

    static readonly bool[][] Ean13LeftHandDigits = new bool[][]
    {
        new bool[] { false, false, false, true, true, false, true },
        new bool[] { false, false, true, true, false, false, true },
        new bool[] { false, false, true, false, false, true, true },
        new bool[] { false, true, true, true, true, false, true },
        new bool[] { false, true, false, false, false, true, true },
        new bool[] { false, true, true, false, false, false, true },
        new bool[] { false, true, false, true, true, true, true },
        new bool[] { false, true, true, true, false, true, true },
        new bool[] { false, true, true, false, true, true, true },
        new bool[] { false, false, false, true, false, true, true }
    };

    static readonly bool[][] Ean13RightHandDigits = new bool[][]
    {
        new bool[] { true, true, true, false, false, true, false },
        new bool[] { true, true, false, false, true, true, false },
        new bool[] { true, true, false, true, true, false, false },
        new bool[] { true, false, true, false, false, true, false },
        new bool[] { true, false, false, true, true, true, false },
        new bool[] { true, false, true, true, true, false, false },
        new bool[] { true, false, false, false, false, true, false },
        new bool[] { true, false, false, true, false, false, false },
        new bool[] { true, false, true, false, true, false, false },
        new bool[] { true, false, false, false, true, false, false }
    };
}
