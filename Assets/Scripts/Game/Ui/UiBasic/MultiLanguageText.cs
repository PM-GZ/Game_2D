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

    private string mOriginStr;

    public MultiLanguageText()
    {
        textPreprocessor = new MultiLanguagePreprocessor(OnOriginStr);
    }

    private void OnOriginStr(string str)
    {
        mOriginStr = str;
    }

    public void ChangedLanguage()
    {
        textPreprocessor = new MultiLanguagePreprocessor(null);
        this.text = textPreprocessor.PreprocessText(mOriginStr);
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
            if(string.IsNullOrEmpty(text)) return text;

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
