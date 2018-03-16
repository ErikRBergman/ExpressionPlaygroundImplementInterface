namespace Serpent.IntermediateLanguageTools.Helpers
{
    using System.Text;

    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendTabbed(this StringBuilder stringBuilder, int tabCount, string text)
        {
            return stringBuilder.Append(Tabs.GetTabs(tabCount) + text);
        }

        public static StringBuilder AppendLineTabbed(this StringBuilder stringBuilder, int tabCount, string text)
        {
            return stringBuilder.AppendLine(Tabs.GetTabs(tabCount) + text);
        }
    }
}