using Assets.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Design;
using UnityEngine;

public static class Shop_DataSaveSystem
{
    public static int[] ConvertToIntArray(string inputStates)
    {
        int[] numbers = Array.ConvertAll(inputStates.Split(','), int.Parse);
        return numbers;
    }

    public static T GetStateFromIndex<T>(string inputStates, int index)
    {
        int[] numbers = Array.ConvertAll(inputStates.Split(','), int.Parse);
        return (T)Enum.GetValues(typeof(T)).GetValue(numbers[index]);
    }

    public static int ConvertEnumToIntRepresentation<T>(T _enum)
    {
        return Array.IndexOf(Enum.GetValues(typeof(T)), _enum);
    }

    public static string Stringify(int[] productStatesArray)
    {
        string defaultProductStates = "";
        for (int i = 0; i < productStatesArray.Length; i++)
        {
            if (i == 0) { defaultProductStates += $"{productStatesArray[i]}"; }
            else if (i == 1) { defaultProductStates += $",{productStatesArray[i]}"; }
            else if (i < 4)
            {
                defaultProductStates += $",{productStatesArray[i]}";
            }
            else
            {
                defaultProductStates += $",{productStatesArray[i]}";
            }
        }
        return defaultProductStates;
    }

    public static string GetSequencedDefaultProductStates(int productsCount)
    {
        string defaultProductStates = "";
        for (int i = 0; i < productsCount; i++)
        {
            if (i == 0) { defaultProductStates += $"{0}"; }
            else if (i == 1) { defaultProductStates += $",{1}"; }
            else if (i < 4)
            {
                defaultProductStates += $",{2}";
            }
            else
            {
                defaultProductStates += $",{3}";
            }
        }
        return defaultProductStates;
    }
}
