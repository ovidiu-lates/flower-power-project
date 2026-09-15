using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Helpers;

public static class HelpersImplementation
{
    public static string CleanName(string name)
    {
        return string.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
