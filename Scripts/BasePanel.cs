using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    private float alphaSpeed = 4;
    private bool isShow;
    private bool fading;
    private UnityAction hideCallback;

    public bool IsShow => isShow;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        Init();
    }

    public abstract void Init();

    public virtual void Show()
    {
        if (isShow) return;
        isShow = true;
        fading = true;
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void Hide(UnityAction callback = null)
    {
        if (!isShow) return;
        isShow = false;
        fading = true;
        hideCallback = callback;
        canvasGroup.blocksRaycasts = false;
    }

    protected virtual void Update()
    {
        if (!fading) return;

        if (isShow && canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += alphaSpeed * Time.unscaledDeltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                fading = false;
            }
        }
        else if (!isShow && canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= alphaSpeed * Time.unscaledDeltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                fading = false;
                hideCallback?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}
