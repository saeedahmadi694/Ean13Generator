using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Text;

namespace Ean13Generator
{
    public class Ean13
    {
        private string _sName = "EAN13";

        private float _fMinimumAllowableScale = .8f;
        private float _fMaximumAllowableScale = 2.0f;

        // This is the nomimal size recommended by the EAN.
        private float _fWidth = 37.29f;
        private float _fHeight = 25.93f;
        private float _fFontSize = 8.0f;
        private float _fScale = 1.0f;

        // Left Hand Digits.
        private string[] _aOddLeft = { "0001101", "0011001", "0010011", "0111101",
                                          "0100011", "0110001", "0101111", "0111011",
                                          "0110111", "0001011" };

        private string[] _aEvenLeft = { "0100111", "0110011", "0011011", "0100001",
                                           "0011101", "0111001", "0000101", "0010001",
                                           "0001001", "0010111" };

        // Right Hand Digits.
        private string[] _aRight = { "1110010", "1100110", "1101100", "1000010",
                                        "1011100", "1001110", "1010000", "1000100",
                                        "1001000", "1110100" };

        private string _sQuiteZone = "000000000";

        private string _sLeadTail = "101";

        private string _sSeparator = "01010";

        private string _sCountryCode = "00";
        private string _sManufacturerCode;
        private string _sProductCode;
        private string _sChecksumDigit;
        private Font _font;

        public Ean13()
        {
            _font = SystemFonts.CreateFont("Arial", this._fFontSize * this.Scale);

        }
        public Ean13(string countryCode, string mfgNumber, string productId, string checkDigit)
        {
            this.CountryCode = countryCode;
            this.ManufacturerCode = mfgNumber;
            this.ProductCode = productId;
            this.ChecksumDigit = checkDigit;

            _font = SystemFonts.CreateFont("Arial", this._fFontSize * this.Scale);
            Generator();
        }


        public void Generator()
        {
            //var b= new Barcode()
            int width = 320;
            int height = 300;
            string imagePath = "barcode.jpg";

            using (var image = new Image<Rgba32>(width, height, Color.White))
            {
                var imagess = DrawEan13Barcode(image, new PointF(0, 0));
                // Save the image
                imagess.Save(imagePath);
                Console.WriteLine("done");

            }
        }


        public Image<Rgba32> DrawEan13Barcode(Image<Rgba32> image, PointF pt)
        {
            float width = this.Width * this.Scale;
            float height = this.Height * this.Scale;

            // EAN13 Barcode should be a total of 113 modules wide.
            float lineWidth = width / 113f;

            System.Text.StringBuilder strbEAN13 = new System.Text.StringBuilder();
            System.Text.StringBuilder sbTemp = new System.Text.StringBuilder();

            PointF textPosition = new PointF(pt.X, pt.Y + height);

            // Calculate the Check Digit.
            this.CalculateChecksumDigit();

            sbTemp.AppendFormat("{0}{1}{2}{3}",
                this.CountryCode,
                this.ManufacturerCode,
                this.ProductCode,
                this.ChecksumDigit);

            string sTemp = sbTemp.ToString();

            string sLeftPattern = "";

            // Convert the left hand numbers.
            sLeftPattern = ConvertLeftPattern(sTemp.Substring(0, 7));

            // Build the UPC Code.
            strbEAN13.AppendFormat("{0}{1}{2}{3}{4}{1}{0}",
                this._sQuiteZone, this._sLeadTail,
                sLeftPattern,
                this._sSeparator,
                ConvertToDigitPatterns(sTemp.Substring(7), this._aRight));

            string sTempUPC = strbEAN13.ToString();

            float fTextHeight = 0;
            image.Mutate(c =>
            {
                c.DrawText(sTempUPC, _font, Color.Black, textPosition);
                fTextHeight = c.GetCurrentSize().Height;
            });

            // Draw the barcode lines.
            float xStart = pt.X;
            float xEnd = 0;

            for (int i = 0; i < strbEAN13.Length; i++)
            {
                if (sTempUPC[i] == '1')
                {
                    if (xStart == pt.X)
                        xStart = xEnd;

                    // Save room for the UPC number below the bar code.
                    if ((i > 12 && i < 55) || (i > 57 && i < 101))
                        // Draw space for the number
                        image.Mutate(c => c.Fill(Color.Black, new RectangleF(xStart, pt.Y, lineWidth, height - fTextHeight)));
                    else
                        // Draw a full line.
                        image.Mutate(c => c.Fill(Color.Black, new RectangleF(xStart, pt.Y, lineWidth, height)));
                }

                xStart += lineWidth;
                xEnd = xStart;
            }

            // Draw the upc numbers below the line.
            float yPosition = pt.Y + (height - fTextHeight);

            // Draw 1st digit of the country code.
            var size = TextMeasurer.Measure(sTemp.Substring(0, 1), new TextOptions(_font));
            xEnd += size.Width;
            image.Mutate(c => c.DrawText(sTemp.Substring(0, 1), SystemFonts.CreateFont("Arial", this._fFontSize * this.Scale), Color.Black, new PointF(xEnd, yPosition)));

            xEnd += 43 * lineWidth;

            size = TextMeasurer.Measure(sTemp.Substring(1, 6), new TextOptions(_font));
            xEnd += size.Width;
            // Draw MFG Number.
            image.Mutate(c => c.DrawText(sTemp.Substring(1, 6), SystemFonts.CreateFont("Arial", this._fFontSize * this.Scale), Color.Black, new PointF(xEnd, yPosition)));

            xEnd += 11 * lineWidth;

            // Draw Product ID.
            image.Mutate(c => c.DrawText(sTemp.Substring(7), SystemFonts.CreateFont("Arial", this._fFontSize * this.Scale), Color.Black, new PointF(xEnd, yPosition)));
            return image;
        }

        private string ConvertLeftPattern(string sLeft)
        {
            switch (sLeft.Substring(0, 1))
            {
                case "0":
                    return CountryCode0(sLeft.Substring(1));

                case "1":
                    return CountryCode1(sLeft.Substring(1));

                case "2":
                    return CountryCode2(sLeft.Substring(1));

                case "3":
                    return CountryCode3(sLeft.Substring(1));

                case "4":
                    return CountryCode4(sLeft.Substring(1));

                case "5":
                    return CountryCode5(sLeft.Substring(1));

                case "6":
                    return CountryCode6(sLeft.Substring(1));

                case "7":
                    return CountryCode7(sLeft.Substring(1));

                case "8":
                    return CountryCode8(sLeft.Substring(1));

                case "9":
                    return CountryCode9(sLeft.Substring(1));

                default:
                    return "";
            }
        }


        private string CountryCode0(string sLeft)
        {
            // 0 Odd Odd  Odd  Odd  Odd  Odd 
            return ConvertToDigitPatterns(sLeft, this._aOddLeft);
        }


        private string CountryCode1(string sLeft)
        {
            // 1 Odd Odd  Even Odd  Even Even 
            System.Text.StringBuilder sReturn = new StringBuilder();
            // The two lines below could be replaced with this:
            // sReturn.Append( ConvertToDigitPatterns( sLeft.Substring( 0, 2 ), this._aOddLeft ) );
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aOddLeft));
            // The two lines below could be replaced with this:
            // sReturn.Append( ConvertToDigitPatterns( sLeft.Substring( 4, 2 ), this._aEvenLeft ) );
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aEvenLeft));
            return sReturn.ToString();
        }


        private string CountryCode2(string sLeft)
        {
            // 2 Odd Odd  Even Even Odd  Even 
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aEvenLeft));
            return sReturn.ToString();
        }


        private string CountryCode3(string sLeft)
        {
            // 3 Odd Odd  Even Even Even Odd 
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aOddLeft));
            return sReturn.ToString();
        }


        private string CountryCode4(string sLeft)
        {
            // 4 Odd Even Odd  Odd  Even Even 
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aEvenLeft));
            return sReturn.ToString();
        }


        private string CountryCode5(string sLeft)
        {
            // 5 Odd Even Even Odd  Odd  Even 
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aEvenLeft));
            return sReturn.ToString();
        }


        private string CountryCode6(string sLeft)
        {
            // 6 Odd Even Even Even Odd  Odd 
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aOddLeft));
            return sReturn.ToString();
        }


        private string CountryCode7(string sLeft)
        {
            // 7 Odd Even Odd  Even Odd  Even
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aEvenLeft));
            return sReturn.ToString();
        }


        private string CountryCode8(string sLeft)
        {
            // 8 Odd Even Odd  Even Even Odd 
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aOddLeft));
            return sReturn.ToString();
        }


        private string CountryCode9(string sLeft)
        {
            // 9 Odd Even Even Odd  Even Odd 
            System.Text.StringBuilder sReturn = new StringBuilder();
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(0, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(1, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(2, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(3, 1), this._aOddLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(4, 1), this._aEvenLeft));
            sReturn.Append(ConvertToDigitPatterns(sLeft.Substring(5, 1), this._aOddLeft));
            return sReturn.ToString();
        }


        private string ConvertToDigitPatterns(string inputNumber, string[] patterns)
        {
            System.Text.StringBuilder sbTemp = new StringBuilder();
            int iIndex = 0;
            for (int i = 0; i < inputNumber.Length; i++)
            {
                iIndex = Convert.ToInt32(inputNumber.Substring(i, 1));
                sbTemp.Append(patterns[iIndex]);
            }
            return sbTemp.ToString();
        }


        public void CalculateChecksumDigit()
        {
            string sTemp = this.CountryCode + this.ManufacturerCode + this.ProductCode;
            int iSum = 0;
            int iDigit = 0;

            // Calculate the checksum digit here.
            for (int i = sTemp.Length; i >= 1; i--)
            {
                iDigit = Convert.ToInt32(sTemp.Substring(i - 1, 1));
                if (i % 2 == 0)
                {   // odd
                    iSum += iDigit * 3;
                }
                else
                {   // even
                    iSum += iDigit * 1;
                }
            }

            int iCheckSum = (10 - (iSum % 10)) % 10;
            this.ChecksumDigit = iCheckSum.ToString();

        }


        #region -- Attributes/Properties --

        public string Name
        {
            get
            {
                return _sName;
            }
        }

        public float MinimumAllowableScale
        {
            get
            {
                return _fMinimumAllowableScale;
            }
        }

        public float MaximumAllowableScale
        {
            get
            {
                return _fMaximumAllowableScale;
            }
        }

        public float Width
        {
            get
            {
                return _fWidth;
            }
        }

        public float Height
        {
            get
            {
                return _fHeight;
            }
        }

        public float FontSize
        {
            get
            {
                return _fFontSize;
            }
        }

        public float Scale
        {
            get
            {
                return _fScale;
            }

            set
            {
                if (value < this._fMinimumAllowableScale || value > this._fMaximumAllowableScale)
                    throw new Exception("Scale value out of allowable range.  Value must be between " +
                        this._fMinimumAllowableScale.ToString() + " and " +
                        this._fMaximumAllowableScale.ToString());
                _fScale = value;
            }
        }

        public string CountryCode
        {
            get
            {
                return _sCountryCode;
            }

            set
            {
                while (value.Length < 2)
                {
                    value = "0" + value;
                }
                _sCountryCode = value;
            }
        }

        public string ManufacturerCode
        {
            get
            {
                return _sManufacturerCode;
            }

            set
            {
                _sManufacturerCode = value;
            }
        }

        public string ProductCode
        {
            get
            {
                return _sProductCode;
            }

            set
            {
                _sProductCode = value;
            }
        }

        public string ChecksumDigit
        {
            get
            {
                return _sChecksumDigit;
            }

            set
            {
                int iValue = Convert.ToInt32(value);
                if (iValue < 0 || iValue > 9)
                    throw new Exception("The Check Digit mst be between 0 and 9.");
                _sChecksumDigit = value;
            }
        }

        #endregion -- Attributes/Properties --

    }

}
