using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager
{
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    private HashSet<string> loadingPanels = new HashSet<string>();
    private Transform panelsParent;

    public void Init(Transform parent)
    {
        panelsParent = parent;
    }

    public async UniTask<T> GetPanel<T>() where T : BasePanel
    {
        string name = typeof(T).Name;
        if (panelDic.ContainsKey(name))
        {
            panelDic[name].gameObject.SetActive(true);
            return panelDic[name] as T;
        }

        // 防止并发调用导致同一面板被加载两次
        if (loadingPanels.Contains(name))
        {
            await UniTask.WaitUntil(() => panelDic.ContainsKey(name));
            return panelDic[name] as T;
        }
        loadingPanels.Add(name);

        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(name);
        if (prefab == null)
        {
            loadingPanels.Remove(name);
            return null;
        }

        GameObject obj = UnityEngine.Object.Instantiate(prefab, panelsParent);
        T panel = obj.GetComponent<T>();
        if (panel == null)
        {
            UnityEngine.Object.Destroy(obj);
            loadingPanels.Remove(name);
            return null;
        }

        panelDic[name] = panel;
        loadingPanels.Remove(name);
        return panel;
    }

    public async UniTask<T> ShowPanel<T>() where T : BasePanel
    {
        T panel = await GetPanel<T>();
        if (panel != null) panel.Show();
        return panel;
    }

    public void HidePanel<T>() where T : BasePanel
    {
        string name = typeof(T).Name;
        if (panelDic.ContainsKey(name))
        {
            panelDic[name].Hide();
        }
    }

    public bool IsShow<T>() where T : BasePanel
    {
        string name = typeof(T).Name;
        return panelDic.ContainsKey(name) && panelDic[name].IsShow;
    }

    public void DestroyAllPanels(params Type[] except)
    {
        loadingPanels.Clear();
        HashSet<string> keep = new HashSet<string>();
        if (except != null)
        {
            foreach (Type t in except) keep.Add(t.Name);
        }

        List<string> keys = new List<string>(panelDic.Keys);
        foreach (string key in keys)
        {
            if (keep.Contains(key)) continue;
            BasePanel panel = panelDic[key];
            if (panel != null) UnityEngine.Object.Destroy(panel.gameObject);
            panelDic.Remove(key);
        }
    }
}
