using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventTrigger : MonoBehaviour,
    IPointerEnterHandler, // 监听鼠标悬停事件
    IPointerDownHandler,  // 监听鼠标按下事件
    ISelectHandler,       // 监听UI元素被选中事件
    ISubmitHandler        // 监听UI元素提交事件
{
    [SerializeField] AudioData selectSFX; // 鼠标悬停或选择时播放的音效
    [SerializeField] AudioData submitSFX; // 鼠标按下或提交时播放的音效

    // 当鼠标悬停在UI元素上时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(selectSFX); // 播放悬停音效
    }

    // 当鼠标按下UI元素时调用
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(submitSFX); // 播放按下音效
    }

    // 当UI元素被选中时调用
    public void OnSelect(BaseEventData eventData)
    {
        AudioManager.Instance.PlaySFX(selectSFX); // 播放选择音效
    }

    // 当UI元素提交时调用
    public void OnSubmit(BaseEventData eventData)
    {
        AudioManager.Instance.PlaySFX(submitSFX); // 播放提交音效
    }
}
