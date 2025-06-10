using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParametersID3
{
    public List<Booleans> infos = new List<Booleans> {new Booleans("Healthy"), new Booleans("NearPlayer"), new Booleans("NearCure") };
    public States action;
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
