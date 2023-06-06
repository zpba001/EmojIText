using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System;

public class EmojiInputField : InputField
{
    public EmojiText m_emojiText;       // 表情文本

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData); // 调用基类的方法，以保留原有功能

        // // 将光标移动到文本的末尾
        // caretPositionInternal = text.Length;
        // caretSelectPositionInternal = caretPositionInternal;
        // UpdateLabel();
        // 开始编辑
        ShowHideEmojiText(false);
    }

    public void ShowHideEmojiText(bool bShow)
    {
        m_emojiText.gameObject.SetActive(bShow);
        m_TextComponent.gameObject.SetActive(!bShow);

        m_emojiText.text = text;
    }

    public void SetText(string str)
    {
        text = str;
        m_emojiText.text = text;
    }
}

