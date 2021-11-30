namespace PoolManagement
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    public class PoolManagerEditor : ScriptableObject
    {
        [MenuItem("Tools/PoolManagement/Scan")]
        private static void Scan()
        {
            string[] guids = AssetDatabase.FindAssets("t:prefab");
            string path = null;
            PoolPrefab prefab = null;
            PoolManagerBase manager = null;
            List<PoolPrefab> prefabs = new List<PoolPrefab>();
            List<PoolManagerBase> managers = new List<PoolManagerBase>();

            for (int i = 0, iMax = guids.Length; i < iMax; i++)
            {
                path = AssetDatabase.GUIDToAssetPath(guids[i]);
                prefab = AssetDatabase.LoadAssetAtPath<PoolPrefab>(path);
                manager = AssetDatabase.LoadAssetAtPath<PoolManagerBase>(path);
                if (manager)
                {
                    managers.Add(manager);
                }
                if (!prefab)
                    continue;
                prefabs.Add(prefab);
            }

            for (int i = 0, iMax = prefabs.Count; i < iMax; i++)
            {
                prefabs[i].SetId(i);
                EditorUtility.SetDirty(prefabs[i]);
            }

            PoolPrefab[] poolPrefabsArray = prefabs.ToArray();
            for (int i = 0, iMax = managers.Count; i < iMax; i++)
            {
                managers[i].InitPrefabs(poolPrefabsArray);
                EditorUtility.SetDirty(managers[i]);
            }
            Debug.Log("End pool scan");
        }
    }
}