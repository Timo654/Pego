using UnityEngine;

public class PersistentObjectsInit
{
    private static GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("PersistentObjects");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnSubsystemRegistration()
    {
        PersistentObjects.Initialize(GetPrefab());
    }
}
