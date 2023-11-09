using UnityEditor;
using UnityEngine;




public class Test : MonoBehaviour
{
    public UiBaseScroll scroll;

    [InitializeOnLoadMethod]
    static void EditorInit()
    {
        TEXT.Init("CN");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
