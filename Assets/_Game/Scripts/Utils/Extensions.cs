using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RockPaperPistol.Utils
{
    public static class Extensions
{
    public static List<T> Shuffle<T>(this List<T> original)
    {
        List<T> shuffledList = original
            .OrderBy(x => Random.value)
            .ToList();

        return shuffledList;
    }
}
}

