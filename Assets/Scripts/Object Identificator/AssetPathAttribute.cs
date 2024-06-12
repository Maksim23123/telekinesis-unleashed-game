using UnityEngine;

public class AssetPathAttribute : PropertyAttribute
{
    public System.Type assetType;

    public AssetPathAttribute(System.Type assetType)
    {
        this.assetType = assetType;
    }
}