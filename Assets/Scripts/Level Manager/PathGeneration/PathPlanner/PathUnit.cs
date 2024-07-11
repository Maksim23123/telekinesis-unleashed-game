using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathUnit 
{
    public int Id { get; set; }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
