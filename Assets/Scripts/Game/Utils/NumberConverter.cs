public class NumberConverter
{
    public static string ConvertNumberToString(double number)
    {
        string[] suffixes = { "", "K", "M", "B", "T", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q",
                              "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "Aa", "Bb", "Cc", "Dd", "Ee", "Ff", "Gg", "Hh", "Ii", "Jj", "Kk", "Ll",
                              "Mm", "Nn", "Oo", "Pp", "Qq", "Rr", "Ss", "Tt", "Uu", "Vv", "Ww", "Xx", "Yy", "Zz" };

        int suffixIndex = 0;
        double convertedNumber = number;

        while (convertedNumber >= 1000.0 && suffixIndex < suffixes.Length - 1)
        {
            convertedNumber /= 1000.0;
            suffixIndex++;
        }
        if (number < 1000)
            return $"{convertedNumber:F0} {suffixes[suffixIndex]}";
        else
            return $"{convertedNumber:F3} {suffixes[suffixIndex]}";
    }
}
