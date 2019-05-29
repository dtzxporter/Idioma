using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows;

namespace Idioma
{
    internal class FontGenerator
    {
        public static void GenerateSpriteSheetFontThing(string FontFamily, string FileName, int FontSize = 72, int TopOffset = 4, int RightOffset = -4, string GenericChar = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|} ¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ")
        {
            //Setup a bitmap image
            using (Bitmap sheet = new Bitmap(1024, 2048))
            {
                using (Graphics graphics = Graphics.FromImage(sheet))
                {
                    List<int> ListOfRequiredNumbers = Enumerable.Range(32, 224).ToList();
                    //Set transparent BG
                    graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                    //A list of positions
                    Dictionary<short, RectangleF> CharsAndPositions = new Dictionary<short, RectangleF>();
                    //Setup font
                    using (Font font = new Font(FontFamily, FontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Point))
                    {
                        //X and Y
                        float X = 0;
                        float Y = 0;
                        //Loop
                        foreach (char c in GenericChar.ToCharArray())
                        {
                            //95x125
                            SizeF SizeOfChar = graphics.MeasureString(c.ToString(), font);
                            short CharASCII = BitConverter.ToInt16(Encoding.Unicode.GetBytes(c.ToString()), 0);
                            if ((X + SizeOfChar.Width + 4) > 1024)
                            {
                                Y += 126;
                                X = 0;
                            }
                            CharsAndPositions.Add(CharASCII, new RectangleF(X, Y, SizeOfChar.Width, SizeOfChar.Height));
                            //Once we get the size, we must calculate the thingy
                            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                            graphics.DrawString(c.ToString(), font, new SolidBrush(Color.White), new PointF(X, Y));
                            X += SizeOfChar.Width;
                        }
                        //Loop through required
                        string DirectoryToDump = Path.GetDirectoryName(FileName);
                        if (!Directory.Exists(Path.Combine(DirectoryToDump, "font_files\\")))
                        {
                            Directory.CreateDirectory(Path.Combine(DirectoryToDump, "font_files\\"));
                        }
                        using (BinaryWriter writeFile = new BinaryWriter(File.Create(Path.Combine(DirectoryToDump, "font_files\\normalFont"))))
                        {
                            //Write the offset
                            writeFile.Write((int)((24 * CharsAndPositions.Count) + 16));
                            //Size in pixels
                            writeFile.Write((int)20);
                            //Number of entries
                            writeFile.Write((int)CharsAndPositions.Count);
                            //Other offset
                            writeFile.Write((int)(24 * CharsAndPositions.Count) + 33);
                            //Loop through and output entries
                            foreach (short required in CharsAndPositions.Keys)
                            {
                                //Write charactor code l,t,r,b
                                writeFile.Write((short)required);
                                //Check
                                if (CharsAndPositions.ContainsKey(Convert.ToByte(required)))
                                {
                                    //We have a position
                                    RectangleF Position = CharsAndPositions[Convert.ToByte(required)];
                                    //Write margin
                                    writeFile.Write((sbyte)0);
                                    //Calculate width and height
                                    float Width = (19f * Position.Width) / 91.9552f;
                                    float Height = (16f * Position.Height) / 83.0464f;
                                    //Calculate the top and right margins
                                    float TopSpacing = (-17f * Height) / 16f;
                                    float RightSpacing = (17f * Width) / 19f;
                                    writeFile.Write(Convert.ToSByte(TopSpacing + TopOffset));
                                    //Check
                                    if (required == 32)
                                    {
                                        //Override space
                                        writeFile.Write(Convert.ToSByte(RightSpacing));
                                    }
                                    else
                                    {
                                        writeFile.Write(Convert.ToSByte(RightSpacing + RightOffset));
                                    }
                                    //Write width and height
                                    writeFile.Write(Convert.ToByte(Width));
                                    writeFile.Write(Convert.ToByte(Height));
                                    writeFile.Write((byte)0);
                                    //Write UVLeft
                                    float UVLeft = Convert.ToSingle(Position.X / 1024);
                                    //Threshold
                                    UVLeft += 0.01f;
                                    //Write
                                    writeFile.Write(UVLeft);
                                    //Write UVTop
                                    float UVTop = Convert.ToSingle(Position.Y / 2048);
                                    //Threshhold
                                    UVTop += 0.01f;
                                    //Write
                                    writeFile.Write(UVTop);
                                    //Write UVRight
                                    float UVRight = (Position.Width / 1024) + UVLeft;
                                    //Threshhold
                                    UVRight -= 0.01f;
                                    //Write
                                    writeFile.Write(UVRight);
                                    //Write UVBottom
                                    float UVBottom = (Position.Height / 2048) + UVTop;
                                    //Threshhold
                                    UVBottom -= 0.01f;
                                    //Write
                                    writeFile.Write(UVBottom);
                                }
                            }
                            byte[] rawData = { 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x6E, 0x6F, 0x72, 0x6D, 0x61, 0x6C, 0x46, 0x6F, 0x6E, 0x74, 0x00, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x67, 0x61, 0x6D, 0x65, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x32, 0x70, 0x63, 0x00 };
                            writeFile.Write(rawData);
                        }
                        using (BinaryWriter writeFile = new BinaryWriter(File.Create(Path.Combine(DirectoryToDump, "font_files\\boldFont"))))
                        {
                            //Write the offset
                            writeFile.Write((int)((24 * CharsAndPositions.Count) + 16));
                            //Size in pixels
                            writeFile.Write((int)20);
                            //Number of entries
                            writeFile.Write((int)CharsAndPositions.Count);
                            //Other offset
                            writeFile.Write((int)(24 * CharsAndPositions.Count) + 31);
                            //Loop through and output entries
                            foreach (short required in CharsAndPositions.Keys)
                            {
                                //Write charactor code l,t,r,b
                                writeFile.Write((short)required);
                                //Check
                                if (CharsAndPositions.ContainsKey(Convert.ToByte(required)))
                                {
                                    //We have a position
                                    RectangleF Position = CharsAndPositions[Convert.ToByte(required)];
                                    //Write margin
                                    writeFile.Write((sbyte)0);
                                    //Calculate width and height
                                    float Width = (19f * Position.Width) / 91.9552f;
                                    float Height = (16f * Position.Height) / 83.0464f;
                                    //Calculate the top and right margins
                                    float TopSpacing = (-17f * Height) / 16f;
                                    float RightSpacing = (17f * Width) / 19f;
                                    writeFile.Write(Convert.ToSByte(TopSpacing + TopOffset));
                                    //Check
                                    if (required == 32)
                                    {
                                        //Override space
                                        writeFile.Write(Convert.ToSByte(RightSpacing));
                                    }
                                    else
                                    {
                                        writeFile.Write(Convert.ToSByte(RightSpacing + RightOffset));
                                    }
                                    //Write width and height
                                    writeFile.Write(Convert.ToByte(Width));
                                    writeFile.Write(Convert.ToByte(Height));
                                    writeFile.Write((byte)0);
                                    //Write UVLeft
                                    float UVLeft = Convert.ToSingle(Position.X / 1024);
                                    //Threshold
                                    UVLeft += 0.01f;
                                    //Write
                                    writeFile.Write(UVLeft);
                                    //Write UVTop
                                    float UVTop = Convert.ToSingle(Position.Y / 2048);
                                    //Threshhold
                                    UVTop += 0.01f;
                                    //Write
                                    writeFile.Write(UVTop);
                                    //Write UVRight
                                    float UVRight = (Position.Width / 1024) + UVLeft;
                                    //Threshhold
                                    UVRight -= 0.01f;
                                    //Write
                                    writeFile.Write(UVRight);
                                    //Write UVBottom
                                    float UVBottom = (Position.Height / 2048) + UVTop;
                                    //Threshhold
                                    UVBottom -= 0.01f;
                                    //Write
                                    writeFile.Write(UVBottom);
                                }
                            }
                            byte[] rawData = { 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x62, 0x6F, 0x6C, 0x64, 0x46, 0x6F, 0x6E, 0x74, 0x00, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x67, 0x61, 0x6D, 0x65, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x32, 0x70, 0x63, 0x00 };
                            writeFile.Write(rawData);
                        }
                        using (BinaryWriter writeFile = new BinaryWriter(File.Create(Path.Combine(DirectoryToDump, "font_files\\bigFont"))))
                        {
                            //Write the offset
                            writeFile.Write((int)((24 * CharsAndPositions.Count) + 16));
                            //Size in pixels
                            writeFile.Write((int)20);
                            //Number of entries
                            writeFile.Write((int)CharsAndPositions.Count);
                            //Other offset
                            writeFile.Write((int)(24 * CharsAndPositions.Count) + 30);
                            //Loop through and output entries
                            foreach (short required in CharsAndPositions.Keys)
                            {
                                //Write charactor code l,t,r,b
                                writeFile.Write((short)required);
                                //Check
                                if (CharsAndPositions.ContainsKey(Convert.ToByte(required)))
                                {
                                    //We have a position
                                    RectangleF Position = CharsAndPositions[Convert.ToByte(required)];
                                    //Write margin
                                    writeFile.Write((sbyte)0);
                                    //Calculate width and height
                                    float Width = (19f * Position.Width) / 91.9552f;
                                    float Height = (16f * Position.Height) / 83.0464f;
                                    //Calculate the top and right margins
                                    float TopSpacing = (-17f * Height) / 16f;
                                    float RightSpacing = (17f * Width) / 19f;
                                    writeFile.Write(Convert.ToSByte(TopSpacing + TopOffset));
                                    //Check
                                    if (required == 32)
                                    {
                                        //Override space
                                        writeFile.Write(Convert.ToSByte(RightSpacing));
                                    }
                                    else
                                    {
                                        writeFile.Write(Convert.ToSByte(RightSpacing + RightOffset));
                                    }
                                    //Write width and height
                                    writeFile.Write(Convert.ToByte(Width));
                                    writeFile.Write(Convert.ToByte(Height));
                                    writeFile.Write((byte)0);
                                    //Write UVLeft
                                    float UVLeft = Convert.ToSingle(Position.X / 1024);
                                    //Threshold
                                    UVLeft += 0.01f;
                                    //Write
                                    writeFile.Write(UVLeft);
                                    //Write UVTop
                                    float UVTop = Convert.ToSingle(Position.Y / 2048);
                                    //Threshhold
                                    UVTop += 0.01f;
                                    //Write
                                    writeFile.Write(UVTop);
                                    //Write UVRight
                                    float UVRight = (Position.Width / 1024) + UVLeft;
                                    //Threshhold
                                    UVRight -= 0.01f;
                                    //Write
                                    writeFile.Write(UVRight);
                                    //Write UVBottom
                                    float UVBottom = (Position.Height / 2048) + UVTop;
                                    //Threshhold
                                    UVBottom -= 0.01f;
                                    //Write
                                    writeFile.Write(UVBottom);
                                }
                            }
                            byte[] rawData = { 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x62, 0x69, 0x67, 0x46, 0x6F, 0x6E, 0x74, 0x00, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x67, 0x61, 0x6D, 0x65, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x32, 0x70, 0x63, 0x00 };
                            writeFile.Write(rawData);
                        }
                        using (BinaryWriter writeFile = new BinaryWriter(File.Create(Path.Combine(DirectoryToDump, "font_files\\extraBigFont"))))
                        {
                            //Write the offset
                            writeFile.Write((int)((24 * CharsAndPositions.Count) + 16));
                            //Size in pixels
                            writeFile.Write((int)20);
                            //Number of entries
                            writeFile.Write((int)CharsAndPositions.Count);
                            //Other offset
                            writeFile.Write((int)(24 * CharsAndPositions.Count) + 35);
                            //Loop through and output entries
                            foreach (short required in CharsAndPositions.Keys)
                            {
                                //Write charactor code l,t,r,b
                                writeFile.Write((short)required);
                                //Check
                                if (CharsAndPositions.ContainsKey(Convert.ToByte(required)))
                                {
                                    //We have a position
                                    RectangleF Position = CharsAndPositions[Convert.ToByte(required)];
                                    //Write margin
                                    writeFile.Write((sbyte)0);
                                    //Calculate width and height
                                    float Width = (19f * Position.Width) / 91.9552f;
                                    float Height = (16f * Position.Height) / 83.0464f;
                                    //Calculate the top and right margins
                                    float TopSpacing = (-17f * Height) / 16f;
                                    float RightSpacing = (17f * Width) / 19f;
                                    writeFile.Write(Convert.ToSByte(TopSpacing + TopOffset));
                                    //Check
                                    if (required == 32)
                                    {
                                        //Override space
                                        writeFile.Write(Convert.ToSByte(RightSpacing));
                                    }
                                    else
                                    {
                                        writeFile.Write(Convert.ToSByte(RightSpacing + RightOffset));
                                    }
                                    //Write width and height
                                    writeFile.Write(Convert.ToByte(Width));
                                    writeFile.Write(Convert.ToByte(Height));
                                    writeFile.Write((byte)0);
                                    //Write UVLeft
                                    float UVLeft = Convert.ToSingle(Position.X / 1024);
                                    //Threshold
                                    UVLeft += 0.01f;
                                    //Write
                                    writeFile.Write(UVLeft);
                                    //Write UVTop
                                    float UVTop = Convert.ToSingle(Position.Y / 2048);
                                    //Threshhold
                                    UVTop += 0.01f;
                                    //Write
                                    writeFile.Write(UVTop);
                                    //Write UVRight
                                    float UVRight = (Position.Width / 1024) + UVLeft;
                                    //Threshhold
                                    UVRight -= 0.01f;
                                    //Write
                                    writeFile.Write(UVRight);
                                    //Write UVBottom
                                    float UVBottom = (Position.Height / 2048) + UVTop;
                                    //Threshhold
                                    UVBottom -= 0.01f;
                                    //Write
                                    writeFile.Write(UVBottom);
                                }
                            }
                            byte[] rawData = { 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x65, 0x78, 0x74, 0x72, 0x61, 0x42, 0x69, 0x67, 0x46, 0x6F, 0x6E, 0x74, 0x00, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x67, 0x61, 0x6D, 0x65, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x32, 0x70, 0x63, 0x00 };
                            writeFile.Write(rawData);
                        }
                        using (BinaryWriter writeFile = new BinaryWriter(File.Create(Path.Combine(DirectoryToDump, "font_files\\objectiveFont"))))
                        {
                            //Write the offset
                            writeFile.Write((int)((24 * CharsAndPositions.Count) + 16));
                            //Size in pixels
                            writeFile.Write((int)20);
                            //Number of entries
                            writeFile.Write((int)CharsAndPositions.Count);
                            //Other offset
                            writeFile.Write((int)(24 * CharsAndPositions.Count) + 36);
                            //Loop through and output entries
                            foreach (short required in CharsAndPositions.Keys)
                            {
                                //Write charactor code l,t,r,b
                                writeFile.Write((short)required);
                                //Check
                                if (CharsAndPositions.ContainsKey(Convert.ToByte(required)))
                                {
                                    //We have a position
                                    RectangleF Position = CharsAndPositions[Convert.ToByte(required)];
                                    //Write margin
                                    writeFile.Write((sbyte)0);
                                    //Calculate width and height
                                    float Width = (19f * Position.Width) / 91.9552f;
                                    float Height = (16f * Position.Height) / 83.0464f;
                                    //Calculate the top and right margins
                                    float TopSpacing = (-17f * Height) / 16f;
                                    float RightSpacing = (17f * Width) / 19f;
                                    writeFile.Write(Convert.ToSByte(TopSpacing + TopOffset));
                                    //Check
                                    if (required == 32)
                                    {
                                        //Override space
                                        writeFile.Write(Convert.ToSByte(RightSpacing));
                                    }
                                    else
                                    {
                                        writeFile.Write(Convert.ToSByte(RightSpacing + RightOffset));
                                    }
                                    //Write width and height
                                    writeFile.Write(Convert.ToByte(Width));
                                    writeFile.Write(Convert.ToByte(Height));
                                    writeFile.Write((byte)0);
                                    //Write UVLeft
                                    float UVLeft = Convert.ToSingle(Position.X / 1024);
                                    //Threshold
                                    UVLeft += 0.01f;
                                    //Write
                                    writeFile.Write(UVLeft);
                                    //Write UVTop
                                    float UVTop = Convert.ToSingle(Position.Y / 2048);
                                    //Threshhold
                                    UVTop += 0.01f;
                                    //Write
                                    writeFile.Write(UVTop);
                                    //Write UVRight
                                    float UVRight = (Position.Width / 1024) + UVLeft;
                                    //Threshhold
                                    UVRight -= 0.01f;
                                    //Write
                                    writeFile.Write(UVRight);
                                    //Write UVBottom
                                    float UVBottom = (Position.Height / 2048) + UVTop;
                                    //Threshhold
                                    UVBottom -= 0.01f;
                                    //Write
                                    writeFile.Write(UVBottom);
                                }
                            }
                            byte[] rawData = { 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x6F, 0x62, 0x6A, 0x65, 0x63, 0x74, 0x69, 0x76, 0x65, 0x46, 0x6F, 0x6E, 0x74, 0x00, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x67, 0x61, 0x6D, 0x65, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x32, 0x70, 0x63, 0x00 };
                            writeFile.Write(rawData);
                        }
                        using (BinaryWriter writeFile = new BinaryWriter(File.Create(Path.Combine(DirectoryToDump, "font_files\\smallFont"))))
                        {
                            //Write the offset
                            writeFile.Write((int)((24 * CharsAndPositions.Count) + 16));
                            //Size in pixels
                            writeFile.Write((int)20);
                            //Number of entries
                            writeFile.Write((int)CharsAndPositions.Count);
                            //Other offset
                            writeFile.Write((int)(24 * CharsAndPositions.Count) + 32);
                            //Loop through and output entries
                            foreach (short required in CharsAndPositions.Keys)
                            {
                                //Write charactor code l,t,r,b
                                writeFile.Write((short)required);
                                //Check
                                if (CharsAndPositions.ContainsKey(Convert.ToByte(required)))
                                {
                                    //We have a position
                                    RectangleF Position = CharsAndPositions[Convert.ToByte(required)];
                                    //Write margin
                                    writeFile.Write((sbyte)0);
                                    //Calculate width and height
                                    float Width = (19f * Position.Width) / 91.9552f;
                                    float Height = (16f * Position.Height) / 83.0464f;
                                    //Calculate the top and right margins
                                    float TopSpacing = (-17f * Height) / 16f;
                                    float RightSpacing = (17f * Width) / 19f;
                                    writeFile.Write(Convert.ToSByte(TopSpacing + TopOffset));
                                    //Check
                                    if (required == 32)
                                    {
                                        //Override space
                                        writeFile.Write(Convert.ToSByte(RightSpacing));
                                    }
                                    else
                                    {
                                        writeFile.Write(Convert.ToSByte(RightSpacing + RightOffset));
                                    }
                                    //Write width and height
                                    writeFile.Write(Convert.ToByte(Width));
                                    writeFile.Write(Convert.ToByte(Height));
                                    writeFile.Write((byte)0);
                                    //Write UVLeft
                                    float UVLeft = Convert.ToSingle(Position.X / 1024);
                                    //Threshold
                                    UVLeft += 0.01f;
                                    //Write
                                    writeFile.Write(UVLeft);
                                    //Write UVTop
                                    float UVTop = Convert.ToSingle(Position.Y / 2048);
                                    //Threshhold
                                    UVTop += 0.01f;
                                    //Write
                                    writeFile.Write(UVTop);
                                    //Write UVRight
                                    float UVRight = (Position.Width / 1024) + UVLeft;
                                    //Threshhold
                                    UVRight -= 0.01f;
                                    //Write
                                    writeFile.Write(UVRight);
                                    //Write UVBottom
                                    float UVBottom = (Position.Height / 2048) + UVTop;
                                    //Threshhold
                                    UVBottom -= 0.01f;
                                    //Write
                                    writeFile.Write(UVBottom);
                                }
                            }
                            byte[] rawData = { 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x73, 0x6D, 0x61, 0x6C, 0x6C, 0x46, 0x6F, 0x6E, 0x74, 0x00, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x67, 0x61, 0x6D, 0x65, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x32, 0x70, 0x63, 0x00 };
                            writeFile.Write(rawData);
                        }
                        using (BinaryWriter writeFile = new BinaryWriter(File.Create(Path.Combine(DirectoryToDump, "font_files\\consoleFont"))))
                        {
                            //Write the offset
                            writeFile.Write((int)((24 * CharsAndPositions.Count) + 16));
                            //Size in pixels
                            writeFile.Write((int)20);
                            //Number of entries
                            writeFile.Write((int)CharsAndPositions.Count);
                            //Other offset
                            writeFile.Write((int)(24 * CharsAndPositions.Count) + 34);
                            //Loop through and output entries
                            foreach (short required in CharsAndPositions.Keys)
                            {
                                //Write charactor code l,t,r,b
                                writeFile.Write((short)required);
                                //Check
                                if (CharsAndPositions.ContainsKey(Convert.ToByte(required)))
                                {
                                    //We have a position
                                    RectangleF Position = CharsAndPositions[Convert.ToByte(required)];
                                    //Write margin
                                    writeFile.Write((sbyte)0);
                                    //Calculate width and height
                                    float Width = (19f * Position.Width) / 91.9552f;
                                    float Height = (16f * Position.Height) / 83.0464f;
                                    //Calculate the top and right margins
                                    float TopSpacing = (-17f * Height) / 16f;
                                    float RightSpacing = (17f * Width) / 19f;
                                    writeFile.Write(Convert.ToSByte(TopSpacing + TopOffset));
                                    //Check
                                    if (required == 32)
                                    {
                                        //Override space
                                        writeFile.Write(Convert.ToSByte(RightSpacing));
                                    }
                                    else
                                    {
                                        writeFile.Write(Convert.ToSByte(RightSpacing + RightOffset));
                                    }
                                    //Write width and height
                                    writeFile.Write(Convert.ToByte(Width));
                                    writeFile.Write(Convert.ToByte(Height));
                                    writeFile.Write((byte)0);
                                    //Write UVLeft
                                    float UVLeft = Convert.ToSingle(Position.X / 1024);
                                    //Threshold
                                    UVLeft += 0.01f;
                                    //Write
                                    writeFile.Write(UVLeft);
                                    //Write UVTop
                                    float UVTop = Convert.ToSingle(Position.Y / 2048);
                                    //Threshhold
                                    UVTop += 0.01f;
                                    //Write
                                    writeFile.Write(UVTop);
                                    //Write UVRight
                                    float UVRight = (Position.Width / 1024) + UVLeft;
                                    //Threshhold
                                    UVRight -= 0.01f;
                                    //Write
                                    writeFile.Write(UVRight);
                                    //Write UVBottom
                                    float UVBottom = (Position.Height / 2048) + UVTop;
                                    //Threshhold
                                    UVBottom -= 0.01f;
                                    //Write
                                    writeFile.Write(UVBottom);
                                }
                            }
                            byte[] rawData = { 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x63, 0x6F, 0x6E, 0x73, 0x6F, 0x6C, 0x65, 0x46, 0x6F, 0x6E, 0x74, 0x00, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x2F, 0x67, 0x61, 0x6D, 0x65, 0x66, 0x6F, 0x6E, 0x74, 0x73, 0x32, 0x70, 0x63, 0x00 };
                            writeFile.Write(rawData);
                        }
                    }
                }
                //Save the bitmap
                System.Windows.Media.Imaging.WriteableBitmap writeableBitmap = new System.Windows.Media.Imaging.WriteableBitmap(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(sheet.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()));
                //Write it
                TgaWriter.Write(writeableBitmap, File.Create(FileName));
            }
        }
    }
}
