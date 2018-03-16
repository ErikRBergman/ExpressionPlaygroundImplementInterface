namespace Serpent.IntermediateLanguageTools.Helpers
{
    using System;
    using System.Text;

    public static class Tabs
    {
        public const string Four = "                ";

        public const string One = "    ";

        public const string Three = "            ";

        public const string Two = "        ";

        private const int GetStaticTabsLimit = 4;

        public static string GetTabs(int count)
        {
            if (count <= GetStaticTabsLimit)
            {
                return GetStaticTabs(count);
            }

            var builder = new StringBuilder(count * 4);

            while (count > 3)
            {
                builder.Append(Four);
                count -= 4;
            }

            builder.Append(GetStaticTabs(count));

            return builder.ToString();
        }

        private static string GetStaticTabs(int count)
        {
            switch (count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return One;
                case 2:
                    return Two;
                case 3:
                    return Three;
                case 4:
                    return Four;
                default:
                    throw new NotImplementedException("Number of tabs not supported");
            }
        }
    }
}