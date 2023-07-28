namespace PrettierCodeshots.Core.Drawing
{
    public class ColourScheme
    {
        // TODO(nki): Load ColourScheme from config. For now let's use the default one.
        public static ColourSchemeBase GetColourScheme()
        {
            return new ColourSchemeBase();
        }
    }
}
