using System.Drawing;

namespace PrettierCodeshots.Core.Drawing
{
    public class ColourSchemeBase
    {
        public ColourSchemeBase() { }

        public Color BackgroundColour = Color.FromArgb(255, 40, 44, 52);
        public Color KeywordColour = Color.FromArgb(255, 224, 108, 117);
        public Color TypeColour = Color.FromArgb(255, 229, 192, 123);
        public Color StringColour = Color.FromArgb(255, 152, 195, 121);
        public Color OperatorColour = Color.FromArgb(255, 198, 120, 221);
        public Color UnknownColour = Color.FromArgb(255, 171, 178, 191);
    }
}
