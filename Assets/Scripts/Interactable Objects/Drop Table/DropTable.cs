using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Drop Table", fileName ="DropTable")]
public class DropTable : ScriptableObject
{
    [SerializeField] private List<DropSlot> _dropSlots = new();

    public GameObject GetRandomDrop()
    {
        float fullWeight = _dropSlots.Sum(x => x.Weight);
        float randomWeightChoice = fullWeight * Random.value;
        int randomDropIndex = 0;
        float currentWeight = 0;
        for (; randomDropIndex < _dropSlots.Count; randomDropIndex++)
        {
            currentWeight += _dropSlots[randomDropIndex].Weight;
            if (currentWeight >= randomWeightChoice)
            {
                break;
            }
        }
        return _dropSlots[randomDropIndex].GameObject;
    }
}
