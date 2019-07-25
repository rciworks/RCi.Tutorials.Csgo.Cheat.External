namespace RCi.Tutorials.Csgo.Cheat.External
{
    /// <summary>
    /// https://github.com/frk1/hazedumper/blob/master/csgo.hpp
    /// </summary>
    public static class Offsets
    {
        public static int dwLocalPlayer;
        public static int m_vecOrigin;
        public static int m_vecViewOffset;

        /// <summary />
        static Offsets()
        {
            #region // find offsets file

            const string OFFSETS_FILENAME = "offsets.txt";
            var directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            System.IO.FileInfo fileInfo;
            do
            {
                fileInfo = System.Linq.Enumerable.FirstOrDefault(directoryInfo.GetFiles(), fi => string.Equals(fi.Name, OFFSETS_FILENAME));
                directoryInfo = directoryInfo.Parent;
            } while (fileInfo is null && !(directoryInfo is null));
            if (fileInfo is null)
            {
                throw new System.IO.FileNotFoundException($"Cannot find '{OFFSETS_FILENAME}'.");
            }

            #endregion

            #region // parse file and set values

            var fieldInfos = typeof(Offsets).GetFields();
            foreach (var line in System.IO.File.ReadAllLines(fileInfo.FullName))
            {
                var match = System.Text.RegularExpressions.Regex.Match(line, @"\A(?<name>.+) = (?<value>.+)\z");
                if (!match.Success)
                {
                    continue;
                }

                var fieldValueStr = match.Groups["value"].Value;
                if (!int.TryParse(fieldValueStr, out var fieldValue) &&
                    !int.TryParse(fieldValueStr.Substring(2, fieldValueStr.Length - 2), System.Globalization.NumberStyles.HexNumber, null, out fieldValue))
                {
                    continue;
                }

                // find corresponding field and set value
                var fieldInfo = System.Linq.Enumerable.FirstOrDefault(fieldInfos, fi => string.Equals(fi.Name, match.Groups["name"].Value) && fi.FieldType == typeof(int));
                fieldInfo?.SetValue(default, fieldValue);
            }

            #endregion
        }
    }
}
