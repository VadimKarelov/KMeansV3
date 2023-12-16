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
}