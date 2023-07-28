using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PrettierCodeshots.Core.Drawing
{
    public class Renderer
    {
        public static string DrawAndCopyImage(string text, List<Token> tokens)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var fontFamily = new FontFamily("Cascadia Mono");
            var font = new Font(fontFamily, 16, FontStyle.Regular);
            var solidBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
            var path = $"{Path.GetTempPath()}\\{DateTime.Now.ToFileTimeUtc()}.png";

            int width;
            int height;
            SizeF characterSize;

            ColourSchemeBase colourScheme = ColourScheme.GetColourScheme();

            using (Bitmap boundsBitmap = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(boundsBitmap))
                {
                    var characterAverageText = "The quick brown+fox-jumps/overthe=lazyDog1! "; // TODO(nki): Why doesn't measure string return consistent results?
                    characterSize = g.MeasureString(characterAverageText, font);
                    characterSize.Width /= characterAverageText.Length;
                    var measureText = text.Replace(" ", " ");
                    var size = g.MeasureString(measureText, font).ToSize();
                    width = size.Width + 40;
                    height = text.Split('\n').Length * (int) characterSize.Height + 20;
                }
            }

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    var lineSpacing = fontFamily.GetLineSpacing(FontStyle.Regular);

                    graphics.Clear(colourScheme.BackgroundColour);

                    int startX = 10;
                    int startY = 10;
                    PointF point = new PointF(startX, startY);
                    for (var i = 0; i < tokens.Count; i++)
                    {
                        var token = tokens[i];
                        if (token.Type == TokenType.NewLine)
                        {
                            point.X = startX;
                            point.Y += characterSize.Height;
                            continue;
                        }
                        if (token.Type == TokenType.Space)
                        {
                            if (i > 0 && string.IsNullOrWhiteSpace(tokens[i - 1].Value))
                                point.X += characterSize.Width;
                            else
                                point.X += characterSize.Width;
                            continue;
                        }
                        if (token.Type == TokenType.Tab)
                        {
                            point.X += (4 * characterSize.Width);
                            continue;
                        }

                        // TODO(nki): Hacky. Token.Value shouldn't be starting with \n. This is a result of the poor lexer.
                        if (token.Value.StartsWith("\n"))
                        {
                            point.Y += characterSize.Height;
                            token.Value = token.Value.TrimStart('\n');
                        }

                        switch (token.Type)
                        {
                            case TokenType.Keyword:
                                solidBrush.Color = colourScheme.KeywordColour;
                                break;
                            case TokenType.Type:
                                solidBrush.Color = colourScheme.TypeColour;
                                break;
                            case TokenType.String:
                                solidBrush.Color = colourScheme.StringColour;
                                break;
                            case TokenType.Operator:
                                solidBrush.Color = colourScheme.OperatorColour;
                                break;
                            case TokenType.Unknown:
                            default:
                                solidBrush.Color = colourScheme.UnknownColour;
                                break;
                        }

                        graphics.DrawString(token.Value, font, solidBrush, point);
                        var measureString = graphics.MeasureString(token.Value, font);
                        if (token.Value.Contains("\n"))
                            point.Y += measureString.Height;
                        else
                            point.X += token.Value.Length * characterSize.Width;
                            //point.X += measureString.Width;
                    }
                }
                bitmap.Save(path, ImageFormat.Png);

                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "powershell";
                process.StartInfo.Arguments = "Set-Clipboard -Path " + path;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
#if DEBUG
                var imageProcess = new System.Diagnostics.Process();
                imageProcess.StartInfo.FileName = "powershell";
                imageProcess.StartInfo.Arguments = "start " + path;
                imageProcess.StartInfo.RedirectStandardOutput = true;
                imageProcess.StartInfo.UseShellExecute = false;
                imageProcess.StartInfo.CreateNoWindow = true;
                imageProcess.Start();
#endif
                return path;
            }
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
