using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;

namespace KMeansV3.Models;

public class KMeansAlghorithm
{
    public List<double> Error { get; set; }
    
    public List<List<ManyDimensionalPoint>> Points { get; set; }

    public KMeansAlghorithm(IEnumerable<ManyDimensionalPoint> initPoints, int numberOfCenters)
    {
        Error = new();
        
        // размерность точек
        int dimension = initPoints.First().Values.Length;
        
        Random rn = new Random();

        // инициализация центров
        var centers = new ManyDimensionalPoint[numberOfCenters];
        for (int i = 0; i < centers.Length; i++)
        {
            centers[i] = new ManyDimensionalPoint()
            {
                Name = $"Центр {i}",
                Values = new double[dimension]
            };
            
            for (int j = 0; j < dimension; j++)
            {
                centers[i].Values[j] = i * 5 + j;
            }
        }

        // распределение точек по центрам
        var pointsToCenters = new List<List<ManyDimensionalPoint>>();
        for (int i = 0; i < numberOfCenters; i++) pointsToCenters.Add(new());

        double error = 0, previousError = 0; 
        
        do
        {
            previousError = error;
            
            // чистим старые расстояния до центров (удялем запись о принадлежности точек центрам)
            for (int i = 0; i < pointsToCenters.Count(); i++)
            {
                pointsToCenters[i] = new List<ManyDimensionalPoint>();
            }
            
            // раскидываем точки по ближайшим центрам
            foreach (var point in initPoints)
            {
                var minDistance = centers.Min(p => p.RangeToPoint(point));
                var center = centers.First(c => c.RangeToPoint(point) == minDistance);
                var index = centers.IndexOf(center);
             
                // записываем точку к центру
                pointsToCenters[index].Add(point);
            }
            
            // считаем новые координаты центров
            for (int i = 0; i < centers.Length; i++)
            {
                // если точек нет, то новое расстояние до центра не считаем
                if (!pointsToCenters[i].Any()) continue;

                centers[i] = new ManyDimensionalPoint()
                {
                    Name = $"Центр {i}",
                    Values = new double[dimension]
                };
                for (int j = 0; j < dimension; j++)
                {
                    // считаем среднее значение каждой координаты точек, присвоенных центру
                    centers[i].Values[j] = pointsToCenters[i].Average(p => p.Values[j]);
                }
            }

            // считаем ошибку
            error = 0;
            for (int i = 0; i < centers.Length; i++)
            {
                if (!pointsToCenters[i].Any()) continue;
                
                error += pointsToCenters[i].Sum(p => p.RangeToPoint(centers[i]));
            }
            
            Error.Add(error);

        } while (error != previousError);

        Points = pointsToCenters;
    }
}