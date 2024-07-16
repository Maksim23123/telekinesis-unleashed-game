using System;
using System.Collections.Generic;

[Serializable]
public class ObjectData
{
    private Dictionary<string, string> variableValues = new Dictionary<string, string>();
    private Dictionary<string, ObjectData> objectDataUnits = new Dictionary<string, ObjectData>();

    public Dictionary<string, string> VariableValues { get => variableValues; set => variableValues = value; }
    public Dictionary<string, ObjectData> ObjectDataUnits { get => objectDataUnits; set => objectDataUnits = value; }
}
