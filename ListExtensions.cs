using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0) return default(T);
            if (list.Count == 1) return list[0];
            return list[Random.Range(0, list.Count)];

        }
        
        public static void Shuffle<T>(this List<T> inputList)
        {
            for (int i = 0; i < inputList.Count - 1; i++)
            {
                T temp = inputList[i];
                int rand = Random.Range(i, inputList.Count);
                inputList[i] = inputList[rand];
                inputList[rand] = temp;
            }
            
        }
    }
}