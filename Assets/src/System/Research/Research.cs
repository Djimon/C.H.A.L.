using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Research
{
    public string Name;
    public Sprite Image;
    public int GoldCost;
    public Dictionary<string, int> DNACostDict;
    public int DurationInRounds;
    public int remainingRounds;
    public bool researchFound = false;
    public bool researchStarted = false;
    public bool researchCompleted = false;

    public Research(string name, Sprite image, int goldCost, Dictionary<string, int> dnaCostDict, int rounds)
    {
        Name = name;
        Image = image;
        GoldCost = goldCost;
        DNACostDict = dnaCostDict;
        DurationInRounds = rounds;
    }


    public void DoResearch()
    {
        if (researchFound && !researchStarted)
        {
            researchStarted = true;
            remainingRounds = DurationInRounds;
            
        }
        else if(researchFound && researchStarted)
        {
            remainingRounds--;
        }
    }

    
}

