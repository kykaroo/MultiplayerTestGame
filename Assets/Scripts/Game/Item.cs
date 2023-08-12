using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Item<T> : Item where T: ItemInfo
{
    public T itemInfo;
}

public abstract class Item : MonoBehaviour
{
    public GameObject itemGameObject;
    
    public abstract void Use();
}
