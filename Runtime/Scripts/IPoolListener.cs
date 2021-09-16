namespace PoolManagement
{
    public interface IPoolListener
    {
        void OnCreatePoolItem();
        void OnEnablePoolItem();
        void OnDisablePoolItem();
    }
}