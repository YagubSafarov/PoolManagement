namespace PoolManagement
{
    using UnityEngine;

    [System.Serializable]
    public struct PoolItem
    {
        public GameObject Prefab;
        public int PreinstanceCount;

        public string GetId()
        {
            return Prefab.name;
        }
    }
}