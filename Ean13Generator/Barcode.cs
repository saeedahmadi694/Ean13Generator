using Ean13Generator.Types;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Ean13Generator
{
    public enum Type
    {

        EAN13,
    }

    public enum LabelPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum AlignmentPosition
    {
        Center,
        Left,
        Right
    }

    public enum ImageFormat
    {
        Bmp,
        Gif,
        Jpeg,
        Pbm,
        Png,
        Tga,
        Tiff,
        Webp,
    }

    public class Barcode
    {
        private readonly string _data;
        private readonly string _caption;
        private readonly int _captionSpace = 20;
        private string _encodedData;
        private readonly Color _foregroundColor = Color.Black;
        private readonly Color _backgroundColor = Color.White;
        private int _width = 600;
        private int _height = 250;
        private readonly bool _showLabel = false;
        private readonly List<int> _specialLine = new() { 0, 2, 46, 48, 92, 94 };

        private Font _labelFont;

        private readonly AlignmentPosition _alignmentPosition = AlignmentPosition.Center;

        public Barcode(string data, string caption)
        {
            _data = data;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, bool showLabel, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, bool showLabel, Font labelFont, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _labelFont = labelFont;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, int width, int height, string caption)
        {
            _data = data;
            _width = width;
            _height = height;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, bool showLabel, int width, int height, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _width = width;
            _height = height;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, bool showLabel, int width, int height, Font labelFont, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _width = width;
            _height = height;
            _labelFont = labelFont;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, bool showLabel, int width, int height,
            AlignmentPosition alignmentPosition, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _width = width;
            _height = height;
            _alignmentPosition = alignmentPosition;
            _caption = caption;

            InitializeType();
        }

        public Barcode(string data, bool showLabel, int width, int height,
            AlignmentPosition alignmentPosition, Font labelFont, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _width = width;
            _height = height;
            _alignmentPosition = alignmentPosition;
            _labelFont = labelFont;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, bool showLabel, int width, int height,
            AlignmentPosition alignmentPosition, Color backgroundColor, Color foregroundColor, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _width = width;
            _height = height;
            _alignmentPosition = alignmentPosition;
            _backgroundColor = backgroundColor;
            _foregroundColor = foregroundColor;
            _caption = caption;

            InitializeType();
        }
        public Barcode(string data, bool showLabel, int width, int height,
            AlignmentPosition alignmentPosition, Color backgroundColor, Color foregroundColor, Font labelFont, string caption)
        {
            _data = data;
            _showLabel = showLabel;
            _width = width;
            _height = height;
            _alignmentPosition = alignmentPosition;
            _backgroundColor = backgroundColor;
            _foregroundColor = foregroundColor;
            _labelFont = labelFont;
            _caption = caption;

            InitializeType();
        }

        private void InitializeType()
        {
            IBarcode barcode;

            barcode = new EAN13(_data);

            _encodedData = barcode.GetEncoding();
        }

        public void SaveImageFile(string path, ImageFormat imageFormat = ImageFormat.Jpeg)
        {
            using (var image = GenerateImage())
                image.Save(path, getImageEncoder(imageFormat));
        }
        public async Task SaveImageFileAsync(string path, ImageFormat imageFormat = ImageFormat.Jpeg)
        {
            using (var image = GenerateImage())
                await image.SaveAsync(path, getImageEncoder(imageFormat));
        }

        public string GetBase64Image()
        {
            using (var image = GenerateImage())
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, getImageEncoder(ImageFormat.Png));
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
        public byte[] GetByteArray()
        {
            using (var image = GenerateImage())
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, getImageEncoder(ImageFormat.Png));
                return memoryStream.ToArray();
            }
        }
        public byte[] GetByteArray(ImageFormat imageFormat)
        {
            using (var image = GenerateImage())
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, getImageEncoder(imageFormat));
                return memoryStream.ToArray();
            }
        }

        private IImageEncoder getImageEncoder(ImageFormat imageFormat)
        {
            if (imageFormat == ImageFormat.Bmp)
            {
                return new BmpEncoder();
            }

            if (imageFormat == ImageFormat.Gif)
            {
                return new GifEncoder();
            }

            if (imageFormat == ImageFormat.Jpeg)
            {
                return new JpegEncoder();
            }

            if (imageFormat == ImageFormat.Pbm)
            {
                return new PbmEncoder();
            }

            if (imageFormat == ImageFormat.Png)
            {
                return new PngEncoder();
            }

            if (imageFormat == ImageFormat.Tga)
            {
                return new TgaEncoder();
            }

            if (imageFormat == ImageFormat.Tiff)
            {
                return new TiffEncoder();
            }

            if (imageFormat == ImageFormat.Webp)
            {
                return new WebpEncoder();
            }

            throw new ArgumentException();
        }


        public Image GetImage()
        {
            return GenerateImage();
        }

        private Image GenerateImage()
        {

            float labelHeight = 0F, labelWidth = 0F;
            TextOptions labelTextOptions = null;

            if (_showLabel)
            {
                labelTextOptions = new TextOptions(GetEffeciveFont())
                {
                    Dpi = 200,
                };

                var labelSize = TextMeasurer.Measure(_data, labelTextOptions);
                labelHeight = labelSize.Height;
                labelWidth = labelSize.Width;
            }

            var iBarWidth = _width / _encodedData.Length;
            var shiftAdjustment = 0;
            var iBarWidthModifier = 1;

            switch (_alignmentPosition)
            {
                case AlignmentPosition.Center:
                    shiftAdjustment = (_width % _encodedData.Length) / 2;
                    break;
                case AlignmentPosition.Left:
                    shiftAdjustment = 0;
                    break;
                case AlignmentPosition.Right:
                    shiftAdjustment = (_width % _encodedData.Length);
                    break;
                default:
                    shiftAdjustment = (_width % _encodedData.Length) / 2;
                    break;
            }

            if (iBarWidth <= 0)
                throw new Exception(
                    "EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

            //draw image
            var pos = 0;
            var halfBarWidth = (int)(iBarWidth * 0.5);

            var image = new Image<Rgba32>(_width, _height + _captionSpace);

            if (!string.IsNullOrEmpty(_caption))
            {
                var size = TextMeasurer.Measure(_caption, labelTextOptions);
                int labelY = 0;
                int labelX = 0;

                var font = GetEffeciveFont();
                image.Mutate(x => x.DrawText(_caption, font, _foregroundColor, new Point(labelX, labelY)));
            }


            image.Mutate(imageContext =>
            {
                imageContext.BackgroundColor(_backgroundColor);

                var pen = new Pen(_foregroundColor, iBarWidth / iBarWidthModifier);
                var drawingOptions = new DrawingOptions
                {
                    GraphicsOptions = new GraphicsOptions
                    {
                        Antialias = true,
                        AlphaCompositionMode = PixelAlphaCompositionMode.Src,
                    }
                };

                var moreLength = 0;
                while (pos < _encodedData.Length)
                {

                    if (_encodedData[pos] == '1')
                    {
                        if (_specialLine.Any(c => c == pos))
                        {
                            moreLength = 8;
                        }
                        imageContext.DrawLines(drawingOptions, pen,
                            new PointF(pos * iBarWidth + shiftAdjustment + halfBarWidth, _captionSpace),
                            new PointF(pos * iBarWidth + shiftAdjustment + halfBarWidth, _height + moreLength)
                        );
                        moreLength = 0;
                    }

                    pos++;
                }
            });


            if (_showLabel)
            {
                var font = GetEffeciveFont();
                float xEnd = iBarWidth + shiftAdjustment + halfBarWidth - 10;

                float yPosition = _height + 2;

                image.Mutate(c => c.DrawText(_data.Substring(0, 1), font, Color.Black, new PointF(xEnd, yPosition)));

                xEnd += 20 * iBarWidth;
                image.Mutate(c => c.DrawText(_data.Substring(1, 6), font, Color.Black, new PointF(xEnd, yPosition)));

                xEnd += 46 * iBarWidth;
                image.Mutate(c => c.DrawText(_data.Substring(7), font, Color.Black, new PointF(xEnd, yPosition)));


            }

            return image;
        }

        private Font GetEffeciveFont()
        {
            FontCollection collection = new();
            //FontFamily family = collection.Add("arial.ttf");
            FontFamily family = collection.Add("wwwroot/assets/fonts/arial.ttf");

            return family.CreateFont(10, FontStyle.Regular);
        }
    }
}