using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ChevronPool : MonoBehaviour
{
    public const int MaxSize = 16;
    const float FloorLiftMeters = 0.02f;
    const float MinSegmentSqr = 0.0001f;

    [SerializeField] Transform prefab;
    [SerializeField] Transform graphSpace;
    [SerializeField] Transform container;

    ObjectPool<Transform> pool;
    readonly Transform[] active = new Transform[MaxSize];
    int activeCount;

    void Awake()
    {
        if (container == null)
            container = transform;

        pool = new ObjectPool<Transform>(
            CreateInstance,
            OnGet,
            OnRelease,
            OnDestroyInstance,
            collectionCheck: false,
            defaultCapacity: MaxSize,
            maxSize: MaxSize);

        var warmed = new Transform[MaxSize];
        for (int i = 0; i < MaxSize; i++)
            warmed[i] = pool.Get();
        for (int i = 0; i < MaxSize; i++)
            pool.Release(warmed[i]);
    }

    public void Show(IReadOnlyList<Vector3> localPoints)
    {
        Hide();

        if (pool == null || graphSpace == null || prefab == null || localPoints == null || localPoints.Count < 2)
            return;

        int segments = localPoints.Count - 1;
        if (segments > MaxSize)
            segments = MaxSize;

        for (int i = 0; i < segments; i++)
        {
            Vector3 a = graphSpace.TransformPoint(localPoints[i]);
            Vector3 b = graphSpace.TransformPoint(localPoints[i + 1]);
            Vector3 flat = b - a;
            flat.y = 0f;
            if (flat.sqrMagnitude < MinSegmentSqr)
                continue;

            Transform chevron = pool.Get();
            active[activeCount] = chevron;
            activeCount++;

            Vector3 mid = (a + b) * 0.5f;
            mid.y += FloorLiftMeters;
            chevron.SetPositionAndRotation(mid, Quaternion.LookRotation(flat, Vector3.up));
        }
    }

    public void Hide()
    {
        if (pool == null)
        {
            activeCount = 0;
            return;
        }

        for (int i = 0; i < activeCount; i++)
        {
            if (active[i] != null)
                pool.Release(active[i]);
            active[i] = null;
        }

        activeCount = 0;
    }

    Transform CreateInstance()
    {
        Transform instance = Instantiate(prefab, container);
        instance.gameObject.SetActive(false);
        return instance;
    }

    static void OnGet(Transform instance)
    {
        instance.gameObject.SetActive(true);
    }

    static void OnRelease(Transform instance)
    {
        instance.gameObject.SetActive(false);
    }

    static void OnDestroyInstance(Transform instance)
    {
        if (instance != null)
            Destroy(instance.gameObject);
    }
}
