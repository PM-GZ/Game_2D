using System;
using System.Text;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
public class MultiLanguageText : TextMeshProUGUI
{
    private const string START_MULTI = "<multi>";
    private const string END_MULTI = "</multi>";


    public new string text { get => base.text; }

    private string mOriginStr;
    private string mKey;
    private object[] mArgs;

    public MultiLanguageText()
    {
        textPreprocessor = new MultiLanguagePreprocessor(OnOriginStr);
    }

    private void OnOriginStr(string str)
    {
        mOriginStr = str;
        SetFieldAndArgs(string.Empty, str, null);
    }

    public void SetText(string keyField)
    {
        base.text = TEXT.GetText(keyField);
        SetFieldAndArgs(keyField, string.Empty, null);
    }

    public void SetText(string keyField, params object[] args)
    {
        base.text = string.Format(TEXT.GetText(keyField), args);
        SetFieldAndArgs(keyField, string.Empty, args);
    }

    private void SetFieldAndArgs(string keyField, string origin, params object[] args)
    {
        mKey = keyField;
        mOriginStr = origin;
        mArgs = args;
    }

    public void ChangedLanguage()
    {
        if (string.IsNullOrEmpty(mOriginStr)) //使用 SetText(keyField) 方式
        {
            SetText(mKey);
        }
        else if (mArgs == null) //在组件中使用 富文本 方式
        {
            textPreprocessor = new MultiLanguagePreprocessor(OnOriginStr);
            base.text = textPreprocessor.PreprocessText(mOriginStr);
        }
        else //使用 SetText(keyField, args) 方式
        {
            SetText(mKey, mArgs);
        }
    }



    private class MultiLanguagePreprocessor : ITextPreprocessor
    {
        private StringBuilder mBuilder = new StringBuilder();
        private Action<string> mOriginStr;

        public MultiLanguagePreprocessor(Action<string> originStr)
        {
            mOriginStr = originStr;
        }

        public string PreprocessText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            mBuilder.Clear();
            int startIndex = text.IndexOf(START_MULTI);
            int endIndex = text.IndexOf(END_MULTI);
            int keyLength = endIndex - (startIndex + START_MULTI.Length);

            if (startIndex == -1 || endIndex == -1) return text;

            mBuilder.Append(text.Substring(0, startIndex));

            string keyStr = text.Substring(startIndex + START_MULTI.Length, keyLength);
            string key = TEXT.GetText(keyStr);
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"找不到对应KEY：“{keyStr}” ");
                return text;
            }
            mBuilder.Append(key);

            string endStr = text.Substring(endIndex + END_MULTI.Length, text.Length - (endIndex + END_MULTI.Length));
            mBuilder.Append(endStr);

            mOriginStr?.Invoke(text);
            return mBuilder.ToString();
        }
    }
}
