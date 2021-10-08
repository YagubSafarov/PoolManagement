namespace PoolManagement
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(PoolManager))]
    public class PoolManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Remove empty"))
            {
                PoolManager poolManager = (PoolManager)target;
                poolManager.PoolItems = poolManager.PoolItems.Where(i => i.Prefab != null).ToArray();
                EditorUtility.SetDirty(poolManager);
            }
            if (GUILayout.Button("Remove dublicates"))
            {
                PoolManager poolManager = (PoolManager)target;
                poolManager.PoolItems = poolManager.PoolItems.GroupBy(i=>i.Prefab.name).Select(x => x.First()).ToArray();
                EditorUtility.SetDirty(poolManager);
            }
        }
    }
}
