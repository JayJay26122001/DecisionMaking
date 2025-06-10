using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ID3 : MonoBehaviour
{
    public List<ParametersID3> parameters = new List<ParametersID3>();

    public float StateGeneralProportion(States act)
    {
        int count = 0;
        foreach(ParametersID3 p in parameters)
        {
            if(p.action == act)
            {
                count++;
            }
        }
        return (float)count/parameters.Count;
    }
    public float StateSpecificProportion(States act, int boolIndex, bool aux)
    {
        int actCount = 0, boolCount = 0;
        foreach (ParametersID3 p in parameters)
        {
            if (p.infos[boolIndex].boolean == aux)
            {
                boolCount++;
                if(p.action == act)
                {
                    actCount++;
                }
            }
        }
        return (float)actCount / boolCount;
    }

    public float BooleanProportion(int boolIndex)
    {
        int count = 0;
        foreach (ParametersID3 p in parameters)
        {
            if (p.infos[boolIndex].boolean)
            {
                count++;
            }
        }
        return (float)count / parameters.Count;
    }

    public float GeneralEntropy()
    {
        float entropy = 0;
        for(int i = 0; i < System.Enum.GetValues(typeof(States)).Length; i++)
        {
            entropy += -StateGeneralProportion((States)i) * Mathf.Log(StateGeneralProportion((States)i), 2);
        }
        return entropy;
    }
    public float SpecificEntropy(int boolIndex, bool aux)
    {
        float entropy = 0;
        for (int i = 0; i < System.Enum.GetValues(typeof(States)).Length; i++)
        {
            entropy += -StateSpecificProportion((States)i, boolIndex, aux) * Mathf.Log(StateSpecificProportion((States)i, boolIndex, aux), 2);
        }
        return entropy;
    }

    public float InformationGain(int boolIndex)
    {
        float gain = GeneralEntropy() - (BooleanProportion(boolIndex) * SpecificEntropy(boolIndex, true)) - ((1 - BooleanProportion(boolIndex)) * SpecificEntropy(boolIndex, false));
        return gain;
    }
}
