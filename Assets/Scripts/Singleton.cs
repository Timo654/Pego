using UnityEngine;

// code stolen from Zombie, thanks!

public interface IPersistentObject
{
    void Initialize();
}

public static class PersistentObjects
{
    public static void Initialize(GameObject prefab)
    {
        var root = new GameObject();
        root.SetActive(false);
        var instance = Object.Instantiate(prefab, root.transform);

        foreach (var component in instance.GetComponentsInChildren<IPersistentObject>(includeInactive: true))
            component.Initialize();

        Object.DontDestroyOnLoad(root);
        instance.transform.DetachChildren();
        Object.Destroy(root);
    }
}

[DisallowMultipleComponent]
public abstract class MonoSingleton : MonoBehaviour, IPersistentObject
{
    private protected MonoSingleton()
    {

    }

    void IPersistentObject.Initialize()
    {
        MakeCurrent();
    }
    public abstract void MakeCurrent();
}


public abstract class MonoSingleton<T> : MonoSingleton
    where T : MonoSingleton<T>
{
    public static T Instance { get; private set; }
    public sealed override void MakeCurrent()
    {
        Instance = (T)this;
    }
}