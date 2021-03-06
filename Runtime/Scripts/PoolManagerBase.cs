namespace PoolManagement
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PoolManagerBase : MonoBehaviour
    {
        [SerializeField]
        protected PoolPrefab[] m_prefabs;
        protected PoolPrefab[,] m_instances;
        protected int m_prefabCount;
        protected int m_maxInstanceCount;
        [SerializeField]
        protected Transform m_instanceParent;
        
        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            m_prefabCount = m_prefabs.Length;

            m_maxInstanceCount = 0;


            for (int m_tempI = 0, m_tempIMax = m_prefabCount; m_tempI < m_tempIMax; m_tempI++)
            {
                int m_tempMaxInstance = m_prefabs[m_tempI].GetInstanceCount();
                if (m_tempMaxInstance > m_maxInstanceCount)
                {
                    m_maxInstanceCount = m_tempMaxInstance;
                }
            }

            m_instances = new PoolPrefab[m_prefabCount, m_maxInstanceCount];
            
            for (int m_tempI = 0, m_tempIMax = m_prefabCount; m_tempI < m_tempIMax; m_tempI++)
            {
                for (int m_tempU = 0, m_tempUMax = m_prefabs[m_tempI].GetInstanceCount(); m_tempU < m_tempUMax; m_tempU++)
                {
                    PoolPrefab tempPoolPrefab = Instantiate(m_prefabs[m_tempI]);
                    tempPoolPrefab.SetId(m_tempI);
                    tempPoolPrefab.SetIndex(m_tempU);

                    if (tempPoolPrefab.IsSetParentOnActivate())
                    {
                        tempPoolPrefab.transform.SetParent(m_instanceParent);
                    }
                    if (tempPoolPrefab.IsChangeActivate())
                    {
                        tempPoolPrefab.gameObject.SetActive(false);
                    }

                    m_instances[m_tempI, m_tempU] = tempPoolPrefab;

                    CallInitHandler(tempPoolPrefab.gameObject);
                }
            }
        }

        public void InitPrefabs(PoolPrefab[] prefabs)
        {
            m_prefabs = prefabs;
        }

        public virtual void GetInstances(int id, int count, ref List<PoolPrefab> prefabs)
        {
            int insertCount = 0;
            int startIndex = prefabs.Count;

            for (int m_tempU = 0, m_tempUMax = m_maxInstanceCount; m_tempU < m_tempUMax; m_tempU++)
            {
                PoolPrefab tempPoolPrefab = m_instances[id, m_tempU];
                if (tempPoolPrefab == null)
                    throw new System.Exception($"Pool not contain inactive item with id: {id}");

                if (!tempPoolPrefab.GetActivity())
                {
                    tempPoolPrefab.SetActivity(true);
                    if (tempPoolPrefab.IsSetParentOnActivate())
                    {
                        tempPoolPrefab.transform.SetParent(null);
                    }
                    if (tempPoolPrefab.IsChangeActivate())
                    {
                        tempPoolPrefab.gameObject.SetActive(true);
                    }
                    prefabs.Add(tempPoolPrefab);
                    
                    CallSpawnHandler(tempPoolPrefab.gameObject);

                    insertCount++;
                    if (insertCount >= count)
                    {
                        OnGetInstances(prefabs, m_tempUMax, m_tempU);
                        return;
                    }
                }
            }
            OnGetInstances(prefabs, startIndex, insertCount);
        }

        public virtual void GetInstances(int id, int count, ref PoolPrefab[] prefabs)
        {
            int insertCount = 0;
            for (int m_tempU = 0, m_tempUMax = m_maxInstanceCount; m_tempU < m_tempUMax; m_tempU++)
            {
                PoolPrefab tempPoolPrefab = m_instances[id, m_tempU];
                if (tempPoolPrefab == null)
                    throw new System.Exception($"Pool not contain inactive item with id: {id}");

                if (!tempPoolPrefab.GetActivity())
                {
                    tempPoolPrefab.SetActivity(true);
                    if (tempPoolPrefab.IsSetParentOnActivate())
                    {
                        tempPoolPrefab.transform.SetParent(null);
                    }
                    if (tempPoolPrefab.IsChangeActivate())
                    {
                        tempPoolPrefab.gameObject.SetActive(true);
                    }
                    prefabs[m_tempU] = tempPoolPrefab;
                    CallSpawnHandler(tempPoolPrefab.gameObject);

                    insertCount++;
                    if (m_tempU >= count)
                    {
                        OnGetInstances(prefabs, m_tempU);
                        return;
                    }
                }
            }
            OnGetInstances(prefabs, insertCount);
        }

        public virtual PoolPrefab GetInstance(int id)
        {
            for (int m_tempU = 0, m_tempUMax = m_maxInstanceCount; m_tempU < m_tempUMax; m_tempU++)
            {
                PoolPrefab tempPoolPrefab = m_instances[id, m_tempU];
                if (tempPoolPrefab == null)
                {
                    throw new System.Exception($"Pool not contain inactive item with id: {id}");
                }

                if (!tempPoolPrefab.GetActivity())
                {
                    tempPoolPrefab.SetActivity(true);
                    if (tempPoolPrefab.IsSetParentOnActivate())
                    {
                        tempPoolPrefab.transform.SetParent(null);
                    }
                    if (tempPoolPrefab.IsChangeActivate())
                    {
                        tempPoolPrefab.gameObject.SetActive(true);
                    }
                    OnGetInstance(tempPoolPrefab);
                    CallSpawnHandler(tempPoolPrefab.gameObject);

                    return tempPoolPrefab;
                }
            }
            throw new System.Exception($"Pool not contain inactive item with id: {id}");
        }

        private void CallInitHandler(GameObject instance)
        {
            IPoolItemInit[] handlers = instance.GetComponents<IPoolItemInit>();
            foreach (var handler in handlers)
            {
                handler.OnPoolItemInit();
            }
        }

        private void CallSpawnHandler(GameObject instance)
        {
            IPoolItemSpawn[] handlers = instance.GetComponents<IPoolItemSpawn>();
            foreach (var handler in handlers)
            {
                handler.OnPoolItemSpawn();
            }
        }

        private void CallDespawnHandler(GameObject instance)
        {
            IPoolItemDespawn[] handlers = instance.GetComponents<IPoolItemDespawn>();
            foreach (var handler in handlers)
            {
                handler.OnPoolItemDespawn();
            }
        }

        public virtual void DisposeInstance(PoolPrefab instance)
        {
            CallDespawnHandler(instance.gameObject);
            instance.SetActivity(false);
            if (instance.IsSetParentOnActivate())
            {
                instance.transform.SetParent(m_instanceParent);
            }
            if (instance.IsChangeActivate())
            {
                instance.gameObject.SetActive(false);
            }
            OnDisposeInstance(instance);
        }

        public virtual void DisposeInstance(PoolPrefab[] instances, int count)
        {
            int disposeCount = 0;
            for (int m_tempI = 0, m_tempIMax = instances.Length; m_tempI < m_tempIMax; m_tempI++)
            {
                DisposeInstance(instances[m_tempI]);
                disposeCount++;
                if (disposeCount >= count)
                {
                    return;
                }
            }
        }

        public virtual void DisposeInstance(IEnumerable<PoolPrefab> instances, int count)
        {
            int disposeCount = 0;
            foreach (PoolPrefab instance in instances)
            {
                DisposeInstance(instance);
                disposeCount++;
                if (disposeCount >= count)
                {
                    return;
                }
            }
        }

        protected virtual void OnGetInstance(PoolPrefab instance)
        {

        }

        protected virtual void OnGetInstances(PoolPrefab[] instance, int count)
        {

        }

        protected virtual void OnGetInstances(IEnumerable<PoolPrefab> instance, int startIndex, int count)
        {

        }

        protected virtual void OnDisposeInstance(PoolPrefab instance)
        {

        }
    }
}