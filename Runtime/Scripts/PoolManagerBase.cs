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

        protected int m_tempId;
        protected int m_tempI;
        protected int m_tempIMax;
        protected int m_tempU;
        protected int m_tempUMax;
        protected int m_tempMaxInstance;

        protected PoolPrefab m_tempPoolPrefab;


        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            m_prefabCount = m_prefabs.Length;

            m_maxInstanceCount = 0;


            for (m_tempI = 0, m_tempIMax = m_prefabCount; m_tempI < m_tempIMax; m_tempI++)
            {
                m_tempMaxInstance = m_prefabs[m_tempI].GetInstanceCount();
                if (m_tempMaxInstance > m_maxInstanceCount)
                {
                    m_maxInstanceCount = m_tempMaxInstance;
                }
            }

            m_instances = new PoolPrefab[m_prefabCount, m_maxInstanceCount];


            for (m_tempI = 0, m_tempIMax = m_prefabCount; m_tempI < m_tempIMax; m_tempI++)
            {
                for (m_tempU = 0, m_tempUMax = m_prefabs[m_tempI].GetInstanceCount(); m_tempU < m_tempUMax; m_tempU++)
                {
                    m_tempPoolPrefab = Instantiate(m_prefabs[m_tempI]);
                    m_tempPoolPrefab.SetId(m_tempI);
                    m_tempPoolPrefab.SetIndex(m_tempU);

                    if (m_tempPoolPrefab.IsSetParentOnActivate())
                    {
                        m_tempPoolPrefab.transform.SetParent(m_instanceParent);
                    }
                    if (m_tempPoolPrefab.IsChangeActivate())
                    {
                        m_tempPoolPrefab.gameObject.SetActive(false);
                    }

                    m_instances[m_tempI, m_tempU] = m_tempPoolPrefab;

                    CallInitHandler(m_tempPoolPrefab.gameObject);
                }
            }
        }

        public void InitPrefabs(PoolPrefab[] prefabs)
        {
            m_prefabs = prefabs;
        }

        public virtual void GetInstances(int id, int count, ref List<PoolPrefab> prefabs)
        {
            m_tempU = 0;
            m_tempUMax = prefabs.Count;

            for (m_tempU = 0, m_tempUMax = m_maxInstanceCount; m_tempU < m_tempUMax; m_tempU++)
            {
                m_tempPoolPrefab = m_instances[id, m_tempU];
                if (m_tempPoolPrefab == null)
                    return;

                if (!m_tempPoolPrefab.GetActivity())
                {
                    m_tempPoolPrefab.SetActivity(true);
                    if (m_tempPoolPrefab.IsSetParentOnActivate())
                    {
                        m_tempPoolPrefab.transform.SetParent(null);
                    }
                    if (m_tempPoolPrefab.IsChangeActivate())
                    {
                        m_tempPoolPrefab.gameObject.SetActive(true);
                    }
                    prefabs.Add(m_tempPoolPrefab);
                    CallSpawnHandler(m_tempPoolPrefab.gameObject);

                    m_tempU++;
                    if (m_tempU >= count)
                    {
                        OnGetInstances(prefabs, m_tempUMax, m_tempU);
                        return;
                    }
                }
            }
            OnGetInstances(prefabs, m_tempUMax, m_tempU);
        }

        public virtual void GetInstances(int id, int count, ref PoolPrefab[] prefabs)
        {
            m_tempU = 0;
            for (m_tempU = 0, m_tempUMax = m_maxInstanceCount; m_tempU < m_tempUMax; m_tempU++)
            {
                m_tempPoolPrefab = m_instances[id, m_tempU];
                if (m_tempPoolPrefab == null)
                    return;

                if (!m_tempPoolPrefab.GetActivity())
                {
                    m_tempPoolPrefab.SetActivity(true);
                    if (m_tempPoolPrefab.IsSetParentOnActivate())
                    {
                        m_tempPoolPrefab.transform.SetParent(null);
                    }
                    if (m_tempPoolPrefab.IsChangeActivate())
                    {
                        m_tempPoolPrefab.gameObject.SetActive(true);
                    }
                    prefabs[m_tempU] = m_tempPoolPrefab;
                    CallSpawnHandler(m_tempPoolPrefab.gameObject);

                    m_tempU++;
                    if (m_tempU >= count)
                    {
                        OnGetInstances(prefabs, m_tempU);
                        return;
                    }
                }
            }
            OnGetInstances(prefabs, m_tempU);
        }

        public virtual PoolPrefab GetInstance(int id)
        {
            for (m_tempU = 0, m_tempUMax = m_maxInstanceCount; m_tempU < m_tempUMax; m_tempU++)
            {
                m_tempPoolPrefab = m_instances[id, m_tempU];
                if (m_tempPoolPrefab == null)
                    return null;

                if (!m_tempPoolPrefab.GetActivity())
                {
                    m_tempPoolPrefab.SetActivity(true);
                    if (m_tempPoolPrefab.IsSetParentOnActivate())
                    {
                        m_tempPoolPrefab.transform.SetParent(null);
                    }
                    if (m_tempPoolPrefab.IsChangeActivate())
                    {
                        m_tempPoolPrefab.gameObject.SetActive(true);
                    }
                    OnGetInstance(m_tempPoolPrefab);
                    CallSpawnHandler(m_tempPoolPrefab.gameObject);

                    return m_tempPoolPrefab;
                }
            }
            return null;
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
            m_tempU = 0;
            for (m_tempI = 0, m_tempIMax = instances.Length; m_tempI < m_tempIMax; m_tempI++)
            {
                DisposeInstance(instances[m_tempI]);
                m_tempU++;
                if (m_tempU >= count)
                {
                    return;
                }
            }
        }

        public virtual void DisposeInstance(IEnumerable<PoolPrefab> instances, int count)
        {
            m_tempU = 0;
            foreach (PoolPrefab instance in instances)
            {
                DisposeInstance(instance);
                m_tempU++;
                if (m_tempU >= count)
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