using UnityEngine;

public static class LayerMaskExtensions
{
    /// <summary>
    /// Checks if the given layer is included in the LayerMask.
    /// </summary>
    /// <param name="layerMask">The LayerMask to check against.</param>
    /// <param name="layer">The layer index to check for (0-31).</param>
    /// <returns>True if the layer is included in the LayerMask, false otherwise.</returns>
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}