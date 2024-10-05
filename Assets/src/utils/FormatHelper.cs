using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FormatHelper
{
    public static string CleanNumber(int number)
    {
        if (number >= 1000000000)
        { 
            return (number / 1000000000f).ToString("0,00") + " B";
        }
        else if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0.00") + " M";
        }
        else
        {
            return number.ToString("N0");
        }
    }

}
