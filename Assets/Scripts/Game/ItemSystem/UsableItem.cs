using System;
using UnityEngine;

namespace Game.ItemSystem
{
    public abstract class UsableItem<T> : UsableItem where T: ItemInfo
    {
        [SerializeField] protected T itemInfo;
    }

    public abstract class UsableItem : MonoBehaviour
    {
        public GameObject itemGameObject;
        
        public virtual void M1ButtonAction() { }
        public virtual void M2ButtonAction() { }
        public virtual void RButtonAction() { }

        public virtual void OnItemChange() { }
        
        public virtual void SetCamera(UnityEngine.Camera camera) { }
    }
}