using UnityEngine;

public static class Helper
{
    public static Color TransparentColor(Color input)
    {
        return new Color(input.r, input.g, input.b, 0);
    }
}
