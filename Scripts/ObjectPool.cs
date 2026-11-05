using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T mPrefab;
    private readonly Queue<T> mPool = new Queue<T>();
    private readonly HashSet<T> mInPool = new HashSet<T>();
    private readonly Transform mParent;
    private readonly int mGrowBase;
    private int mCurrentCapacity;

    public int InPoolCount => mPool.Count;
    public int ActiveCount { get; private set; }
    public int TotalCreated { get; private set; }

    public ObjectPool(T prefab, int preCount = 0, Transform parent = null)
    {
        mPrefab = prefab;
        mGrowBase = preCount > 0 ? preCount : 1;
        mParent = parent;
        GrowTo(mGrowBase);
    }

    public T Get()
    {
        if (mPool.Count == 0)
        {
            int next = mCurrentCapacity >= mGrowBase ? mCurrentCapacity * 2 : mGrowBase;
            GrowTo(next);
        }

        T obj = mPool.Dequeue();
        if (obj != null)
        {
            mInPool.Remove(obj);
            obj.gameObject.SetActive(true);
            ActiveCount++;
        }
        return obj;
    }

    public void Release(T obj)
    {
        if (obj == null || mInPool.Contains(obj)) return;
        obj.gameObject.SetActive(false);
        mPool.Enqueue(obj);
        mInPool.Add(obj);
        if (ActiveCount > 0) ActiveCount--;
    }

    public void Clear()
    {
        foreach (T obj in mPool)
        {
            if (obj != null) Object.Destroy(obj.gameObject);
        }
        mPool.Clear();
        mInPool.Clear();
        ActiveCount = 0;
        mCurrentCapacity = 0;
    }

    private void GrowTo(int target)
    {
        if (target <= mCurrentCapacity) return;
        for (int i = 0; i < target - mCurrentCapacity; i++)
        {
            T obj = Object.Instantiate(mPrefab, mParent);
            obj.gameObject.SetActive(false);
            TotalCreated++;
            mPool.Enqueue(obj);
            mInPool.Add(obj);
        }
        mCurrentCapacity = target;
    }
}
