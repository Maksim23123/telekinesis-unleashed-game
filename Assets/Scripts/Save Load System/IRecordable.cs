using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecordable
{
    public abstract ObjectData GetObjectData();

    public abstract void SetObjectData(ObjectData objectData);
}
