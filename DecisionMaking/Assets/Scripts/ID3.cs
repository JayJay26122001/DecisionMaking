using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ID3
{
    public static float StateGeneralProportion(States act, List<ParametersID3> parameters)
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
    public static float StateSpecificProportion(States act, int boolIndex, bool aux, List<ParametersID3> parameters)
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

    public static float BooleanProportion(int boolIndex, List<ParametersID3> parameters)
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

    public static float GeneralEntropy(List<ParametersID3> parameters)
    {
        float entropy = 0;
        for(int i = 0; i < System.Enum.GetValues(typeof(States)).Length; i++)
        {
            if(StateGeneralProportion((States)i, parameters) != 0)
            {
                entropy += -StateGeneralProportion((States)i, parameters) * Mathf.Log(StateGeneralProportion((States)i, parameters), 2);
            }
        }
        return entropy;
    }
    public static float SpecificEntropy(int boolIndex, bool aux, List<ParametersID3> parameters)
    {
        float entropy = 0;
        for (int i = 0; i < System.Enum.GetValues(typeof(States)).Length; i++)
        {
            if(StateSpecificProportion((States)i, boolIndex, aux, parameters) != 0)
            {
                entropy += -StateSpecificProportion((States)i, boolIndex, aux, parameters) * Mathf.Log(StateSpecificProportion((States)i, boolIndex, aux, parameters), 2);
            }
        }
        return entropy;
    }

    public static float InformationGain(int boolIndex, List<ParametersID3> parameters)
    {
        float gain = GeneralEntropy(parameters) - (BooleanProportion(boolIndex, parameters) * SpecificEntropy(boolIndex, true, parameters)) - ((1 - BooleanProportion(boolIndex, parameters)) * SpecificEntropy(boolIndex, false, parameters));
        return gain;
    }


    public static int GetLargestIG(List<ParametersID3> parameters)
    {
        float high = float.MinValue;
        int highInd = -1;
        for (int i = 0; i < parameters[0].infos.Count; i++)
        {
            if (high < ID3.InformationGain(i, parameters))
            {
                high = ID3.InformationGain(i, parameters);
                highInd = i;
            }
        }
        return highInd;
    }

    public static List<ParametersID3> RemoveExamples(List<ParametersID3> parameters, int boolIndex, bool aux)
    {
        List<ParametersID3> temp = new List<ParametersID3>();
        foreach (ParametersID3 p in parameters)
        {
            if (p.infos[boolIndex].boolean == aux)
            {
                temp.Add(p);
            }
        }
        return temp;
    }
    public static List<ParametersID3> RemoveParameter(List<ParametersID3> parameters, int boolIndex)
    {
        List<ParametersID3> temp = new List<ParametersID3>();
        for(int i = 0; i < parameters.Count; i++)
        {
            temp.Add(new ParametersID3(parameters[i].action));
            temp[i].infos.Clear();
            for(int j = 0; j < parameters[i].infos.Count; j++)
            {
                if(j != boolIndex)
                {
                    temp[i].infos.Add(parameters[i].infos[j]);
                }
            }
        }
        return temp;
    }

    public static int GetLargestProportion(List<ParametersID3> parameters, int boolIndex, bool aux)
    {
        int highInd = -1;
        float high = float.MinValue;
        for (int i = 0; i < System.Enum.GetValues(typeof(States)).Length; i++)
        {
            float prop = ID3.StateSpecificProportion((States)i, boolIndex, aux, parameters);
            if (prop > high)
            {
                high = prop;
                highInd = i;
            }
        }
        return highInd;
    }

    public static bool VerifyBoolean(List<ParametersID3> parameters, int boolIndex, bool aux, ref int auxIndex)
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(States)).Length; i++)
        {
            float prop = ID3.StateSpecificProportion((States)i, boolIndex, aux, parameters);
            if (prop >= 1)
            {
                auxIndex = i;
                return true;
            }
        }
        return false;
    }
}
