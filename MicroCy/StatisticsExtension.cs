﻿using System;
using System.Collections.Generic;

namespace DIOS.Core
{
  public static class StatisticsExtension
  {
    public static DistributionStats GetDistributionStatistics(this List<float> values)
    {
      float mean = 0;
      float coeffVar = 0;
      float median = 0;
      var count = values.Count;
      if (count >= 20)
      {
        values.Sort();
        int quarter = count / 4;
        int half = count / 2;
        int countWithoutDistributionTails = count - 2*quarter;

        median = values[half];
        mean = values.Mean(quarter, countWithoutDistributionTails);
        coeffVar = values.CalculateCoefficientVariable(mean, quarter, countWithoutDistributionTails);
      }
      else if (count > 0)
      {
        mean = values.Mean(0, count);
      }

      return new DistributionStats
      {
        Median = median,
        Mean = mean,
        CoeffVar = coeffVar
      };
    }

    /// <summary>
    /// Removes a range of elements from both sides of the list
    /// </summary>
    /// <param name="values"></param>
    /// <param name="length">length of tails to remove</param>
    private static void RemoveDistributionTails(this List<float> values, int length)
    {
      values.RemoveRange(values.Count - length, length);
      values.RemoveRange(0, length);
    }

    private static float Mean(this List<float> values, int startingIndex, int endIndex)
    {
      float avgSum = 0;
      var count = endIndex - startingIndex;
      for (var i = startingIndex; i < endIndex; i++)
      {
        avgSum += values[i];
      }
      return avgSum / count;
    }

    private static double SquaredSum(this List<float> values, float mean, int startingIndex, int endIndex)
    {
      double Sum = 0;
      for (var i = startingIndex; i < endIndex; i++)
      {
        Sum += Math.Pow(values[i] - mean, 2);
      }
      return Sum;
    }

    private static double StdDeviation(this List<float> values, float mean, int startingIndex, int endIndex)
    {
      var count = endIndex - startingIndex;
      var sqSum = values.SquaredSum(mean, startingIndex, endIndex);
      double stdDev = Math.Sqrt(sqSum / count - 1);
      return stdDev;
    }

    private static float CalculateCoefficientVariable(this List<float> values, float mean, int startingIndex, int endIndex)
    {
      var stdDev = values.StdDeviation(mean, startingIndex, endIndex);
      double coeffVar = stdDev / mean * 100;
      if (double.IsNaN(coeffVar))
        coeffVar = 0;
      return (float)coeffVar;
    }
  }
}
