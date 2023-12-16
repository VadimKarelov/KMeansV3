using System;
using System.ComponentModel;

namespace KMeansV3.Models;

public partial class ManyDimensionalPoint : INotifyPropertyChanged
{
    public string Name { get; set; }
    
    public double[] Values { get; set; }

    public ManyDimensionalPoint Clone(ManyDimensionalPoint point)
    {
        return new ManyDimensionalPoint()
        {
            Values = point.Values
        };
    }

    public override string ToString()
    {
        return $"{Name}:({string.Join(',', Values)})";
    }
    
    public double RangeToPoint(ManyDimensionalPoint otherPoint)
    {
        return RangeToPoint(this, otherPoint);
    }

    public static double RangeToPoint(ManyDimensionalPoint firstPoint, ManyDimensionalPoint secondPoint)
    {
        if (firstPoint.Values.Length != secondPoint.Values.Length)
            throw new ArgumentException("Мерность точек не совпадает!");

        double len = 0;

        for (int i = 0; i < firstPoint.Values.Length; i++)
        {
            len += Math.Pow(firstPoint.Values[i] - secondPoint.Values[i], 2);
        }

        return Math.Sqrt(len);
    }
}