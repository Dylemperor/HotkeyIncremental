public static class NumberFormatter
{
    public static string Format(double number)
    {
        if (number >= 1e48)
            return (number / 1e48).ToString("F2") + "QuDe";
        if (number >= 1e45)
            return (number / 1e45).ToString("F2") + "QaDe";
        if (number >= 1e42)
            return (number / 1e42).ToString("F2") + "TreDe";
        if (number >= 1e39)
            return (number / 1e39).ToString("F2") + "DuoDe";
        if (number >= 1e36)
            return (number / 1e36).ToString("F2") + "UnDe";
        if (number >= 1e33)
            return (number / 1e33).ToString("F2") + "Dc";
        if (number >= 1e30)
            return (number / 1e30).ToString("F2") + "No";
        if (number >= 1e27)
            return (number / 1e27).ToString("F2") + "Oc";
        if (number >= 1e24)
            return (number / 1e24).ToString("F2") + "Sp";
        if (number >= 1e21)
            return (number / 1e21).ToString("F2") + "Sx";
        if (number >= 1e18)
            return (number / 1e18).ToString("F2") + "Qu";
        if (number >= 1e15)
            return (number / 1e15).ToString("F2") + "Qa";
        if (number >= 1e12)
            return (number / 1e12).ToString("F2") + "T";
        if (number >= 1e9)
            return (number / 1e9).ToString("F2") + "B";
        if (number >= 1e6)
            return (number / 1e6).ToString("F2") + "M";
        if (number >= 1e3)
            return (number / 1e3).ToString("F2") + "K";
        return number.ToString("F0");
    }
}
