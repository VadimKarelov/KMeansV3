using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using KMeansV3.Models;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace KMeansV3.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [Reactive]
    [Range(1,10,ErrorMessage = "от 1 до 10")]
    public int NumberOfCenters { get; set; }
    
    [Reactive]
    public ObservableCollection<ManyDimensionalPoint> Points { get; set; }
    
    
    public ReactiveCommand<Unit, Unit> ComputeKMeansCommand { get; set; }
    

    public MainWindowViewModel()
    {
        Points = new(ReadPointsOrCreateDefault());

        var isCommandsAvailable = this.WhenAny(vm => vm.NumberOfCenters,
            n => n.Value >= 1 && n.Value <= 10);
    }

    public void KMeans()
    {
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