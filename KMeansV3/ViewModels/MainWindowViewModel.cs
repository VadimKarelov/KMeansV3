using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using KMeansV3.Models;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Renderable;

namespace KMeansV3.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive]
    [Range(1,10,ErrorMessage = "от 1 до 10")]
    public int NumberOfCenters { get; set; }
    
    [Reactive]
    public ObservableCollection<ManyDimensionalPoint> Points { get; set; }
    
    
    public ReactiveCommand<Unit, Unit> ComputeKMeansCommand { get; set; }
    
    [Reactive]
    public ObservableCollection<AvaPlot> Plots { get; set; }

    [Reactive]
    public List<string> ComputedCenters { get; set; }

    public MainWindowViewModel()
    {
        Plots = new();
        
        Points = new(ReadPointsOrCreateDefault());

        var isCommandsAvailable = this.WhenAny(vm => vm.NumberOfCenters,
            n => n.Value >= 1 && n.Value <= 10);

        ComputeKMeansCommand = ReactiveCommand.Create(KMeans, isCommandsAvailable);
    }

    public void KMeans()
    {
        var a = new KMeansAlghorithm(Points, NumberOfCenters);
        
        ComputedCenters = a.Points.Select(center => string.Join(" ", center.Select(p => p.ToString()))).ToList();
        
        Plots.Clear();

        // график ошибки
        AvaPlot plot1 = new AvaPlot();
        plot1.Plot.AddScatter(Enumerable.Range(1, a.Error.Count).Select(x => (double)x).ToArray(), a.Error.ToArray());
        plot1.Refresh();
        Plots.Add(plot1);

        var dimension = a.Points.First(p => p.Any()).First().Values.Length;

        // связь между центром и его цветом
        Random rn = new();
        Dictionary<int, Color> centersColors = new Dictionary<int, Color>();
        ColorManager cm = new();
        for (int i = 0; i < a.Points.Count(); i++)
            centersColors.Add(i, cm.GetNextColor());
        
        // цикл по координатам, всегда по x будет первая координата, по y - остальные
        for (int i = 1; i < dimension; i++)
        {
            AvaPlot plot = new AvaPlot();
            plot.Plot.XAxis.AxisLabel.Label = $"x1";
            plot.Plot.YAxis.AxisLabel.Label = $"x{i + 1}";

            // идем по центрам
            for (int j = 0; j < a.Points.Count(); j++)
            {
                Color clr = centersColors[j];
                // идем по точкам в центре
                for (int k = 0; k < a.Points[j].Count; k++)
                {
                    var point = a.Points[j][k];
                    plot.Plot.AddMarker(point.Values[0], point.Values[i], color:clr);
                }
            }
            plot.Refresh();
            Plots.Add(plot);
        }
    }
    
    private List<ManyDimensionalPoint> ReadPointsOrCreateDefault()
    {
        string path = "input.json";

        try
        {
            using var reader = new StreamReader(path);
            var inputData = JsonConvert.DeserializeObject<ManyDimensionalPoint[]>(reader.ReadToEnd());

            if (inputData == null || !inputData.Any())
            {
                throw new ArgumentNullException(nameof(inputData));
            }

            return inputData.ToList();
        }
        catch
        {
            Random random = new Random();

            const string pointsLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const int dimensions = 5;

            var res = new List<ManyDimensionalPoint>();

            for (int i = 0; i < pointsLetters.Length; i++)
            {
                List<double> values = new();

                for (int j = 0; j < dimensions; j++)
                {
                    values.Add(random.Next(-5, 50));
                }

                res.Add(new ManyDimensionalPoint() { Name = $"Точка {pointsLetters[i]}", Values = values.ToArray() });
            }

            using StreamWriter writer = new StreamWriter(path);
            writer.Write(JsonConvert.SerializeObject(res.ToArray()));

            return res;
        }
    }
}