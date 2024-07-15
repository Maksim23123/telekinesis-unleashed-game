using System;
using UnityEngine;

public class AssetPathAttribute : PropertyAttribute
{
    public Type AssetType { get; set; }

    public AssetPathAttribute(Type assetType)
    {
        AssetType = assetType;
    }
}
