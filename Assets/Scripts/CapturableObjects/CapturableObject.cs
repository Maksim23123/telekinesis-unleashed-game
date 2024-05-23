using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CapturableObject : MonoBehaviour
{
     public abstract void ProcessManipulation(Vector3 direction, float power);
}
