using System;
using System.Drawing;
using System.Linq;

namespace KMeansV3.Models;

/// <summary>
/// Представляет возможность раскрашивать графики последовательно в нобходимые цвета
/// </summary>
internal class ColorManager
{
    private Color[] _colors;
    /// <summary>
    /// Индекс выданного цвета
    /// </summary>
    private int _colorIndex;

    public ColorManager()
    {
        _colorIndex = -1;

        _colors = InitColors();
    }

    public Color GetNextColor()
    {
        _colorIndex++;

        if (_colorIndex >= _colors.Length)
            _colorIndex = 0;

        return _colors[_colorIndex];
    }

    private Color[] InitColors()
    {
        Color[] res = new Color[10];

        // из прототипа
        Array colorsArray = Enum.GetValues(typeof(KnownColor));
        KnownColor[] allColors = new KnownColor[colorsArray.Length];
        Array.Copy(colorsArray, allColors, colorsArray.Length);
        KnownColor[] tenColors = new KnownColor[10] {allColors[29], allColors[34], allColors[36], allColors[52], allColors[58],
            allColors[78], allColors[126], allColors[139], allColors[39], allColors[165] };

        res = tenColors.Select(c => Color.FromKnownColor(c)).ToArray();

        return res;
    }
}