using System;
using System.Collections.Generic;

[Serializable]
public class ObjectData
{
    public Dictionary<string, string> variableValues = new Dictionary<string, string>();
    public Dictionary<string, ObjectData> objectDataUnits = new Dictionary<string, ObjectData>();
}
