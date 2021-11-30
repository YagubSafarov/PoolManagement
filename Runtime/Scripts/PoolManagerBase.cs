namespace PoolManagement
{
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
                    m_tempPoolPrefab = Instantiate(m_prefabs[m_tempI], m_instanceParent);
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
                }
            }
        }

        public void InitPrefabs(PoolPrefab[] prefabs)
        {
            m_prefabs = prefabs;
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

                    return m_tempPoolPrefab;
                }
            }
            return null;
        }

        public virtual void DisposeInstance(PoolPrefab instance)
        {
            instance.SetActivity(false);
            if (instance.IsSetParentOnActivate())
            {
                instance.transform.SetParent(m_instanceParent);
            }
            if (instance.IsChangeActivate())
            {
                instance.gameObject.SetActive(false);
            }
        }
    }
}