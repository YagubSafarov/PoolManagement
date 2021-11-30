namespace PoolManagement
{
    using UnityEngine;

    public class PoolPrefab : MonoBehaviour
    {
        [SerializeField]
        private int m_id;
        [SerializeField]
        private int m_index;
        [SerializeField]
        private int m_instanceCount = 1;

        [SerializeField]
        private bool m_setParentOnActivate;
        [SerializeField]
        private bool m_changeActivate;

        private bool m_isActivity;

        public int GetId()
        {
            return m_id;
        }

        public void SetId(int id)
        {
            m_id = id;
        }

        public int GetIndex()
        {
            return m_index;
        }

        public void SetIndex(int index)
        {
            m_index = index;
        }

        public int GetInstanceCount()
        {
            return m_instanceCount;
        }

        public bool GetActivity()
        {
            return m_isActivity;
        }

        internal void SetActivity(bool activate)
        {
            m_isActivity = activate;
            OnActivate(m_isActivity);
        }

        public bool IsSetParentOnActivate()
        {
            return m_setParentOnActivate;
        }

        public bool IsChangeActivate()
        {
            return m_changeActivate;
        }

        public virtual void OnActivate(bool activate)
        {
        }
    }
}