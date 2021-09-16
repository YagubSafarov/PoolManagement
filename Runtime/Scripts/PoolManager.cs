namespace PoolManagement
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    public class PoolManager : MonoBehaviour
    {
        [SerializeField]
        private PoolItem[] m_poolItems;
        [SerializeField]
        private Transform m_poolParent;

        private Dictionary<string, List<GameObject>> m_entity;

        private void Awake()
        {
            CheckPoolList();

            m_entity = new Dictionary<string, List<GameObject>>();

            foreach (var item in m_poolItems)
            {
                if (!m_entity.ContainsKey(item.GetId()))
                {
                    m_entity.Add(item.GetId(), new List<GameObject>());
                }

                for (int i = 0; i < item.PreinstanceCount; i++)
                {
                    CreateNewInstance(item);
                }
            }
        }

        private bool GetPoolItem(string id, out PoolItem poolItem)
        {
            for (int i = 0, iMax = m_poolItems.Length; i < iMax; i++)
            {
                if (m_poolItems[i].GetId() == id)
                {
                    poolItem = m_poolItems[i];
                    return true;
                }
            }

            poolItem = default;
            return false;
        }

        private void CheckPoolList()
        {

            var duplicates = m_poolItems
                .GroupBy(i => i)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            if (duplicates.Count() > 0)
            {
                foreach (var d in duplicates)
                    Debug.LogError($"Duplicate key {d.GetId()}");
                throw (new System.Exception($"Pool list has duplicates"));
            }
        } 

        private void CreateNewInstance(PoolItem template)
        {
            var go = Instantiate(template.Prefab, m_poolParent);
            go.name = go.name.Remove(go.name.Length - 7, 7);
            IPoolListener listener = go.GetComponent<IPoolListener>();
            if (listener != null)
                listener.OnCreatePoolItem();
            go.SetActive(false);
            m_entity[template.GetId()].Add(go);
        }

        private void CreateNewInstance(GameObject template)
        {
            var go = Instantiate(template, m_poolParent);
            IPoolListener listener = go.GetComponent<IPoolListener>();
            if (listener != null)
                listener.OnCreatePoolItem();
            go.SetActive(false);
            m_entity[template.name].Add(go);
        }

        public GameObject Pool(string id, Transform setForParent = null)
        {
            if (m_entity.ContainsKey(id))
            {
                if (m_entity[id].Count == 0)
                {
                    PoolItem item;
                    if (GetPoolItem(id, out item))
                    {
                        CreateNewInstance(item);
                    }
                    else
                    {
                        throw (new System.Exception($"Can't find {id}"));
                    }
                }

                var result = m_entity[id][0];
                m_entity[id].RemoveAt(0);
                result.transform.SetParent(setForParent);
                result.SetActive(true);
                return result;
            }
            throw (new System.Exception($"Prefab do't contain in the list {id}"));
        }

        public void Unpool(GameObject prefab)
        {
            prefab.SetActive(false);
            prefab.transform.SetParent(m_poolParent);
            m_entity[prefab.name].Add(prefab);
        }
    }
}
