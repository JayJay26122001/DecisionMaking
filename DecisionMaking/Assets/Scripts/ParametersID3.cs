using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParametersID3
{
    public List<Booleans> infos = new List<Booleans>();
    public States action;

    public ParametersID3(States act)
    {
        action = act;
    }
}

[Serializable]
public class Booleans
{
    public string name;
    public bool boolean;

    public Booleans(string name)
    {
        this.name = name;
    }
}
