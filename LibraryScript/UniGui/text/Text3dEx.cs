using UnityEngine;
[AddComponentMenu("UniGui/Text/Text3dEx")]
class Text3dEx : MonoBehaviourIgnoreGui
{
    public TextMesh textMesh = null;
    public TextMesh[] backtextMesh = null;
    void Start()
    {
        color = Color.white;
        backcolor = Color.black;
    }

    public string text
    {
        get
        {
            return textMesh.text;
        }
        set
        {
            textMesh.text = value;
            for (int i = 0; i < backtextMesh.Length; i ++ )
            {
                backtextMesh[i].text = value;
            }
        }
    }
    public Color color
    {
        get
        {
            return textMesh.GetComponent<Renderer>().material.color;
        }
        set
        {
            textMesh.GetComponent<Renderer>().material.color = value;
        }
    }
    public Color backcolor
    {
        get
        {
            return backtextMesh[0].GetComponent<Renderer>().material.color;
        }
        set
        {
            for (int i = 0; i < backtextMesh.Length; i ++ )
            {
                backtextMesh[i].GetComponent<Renderer>().material.color = value;
            }
        }
    }

    /*
    public void ClonebacktextMesh()
    {

        foreach (TextMesh t in backtextMesh)
        {

            t.renderer.material = new Material(t.renderer.material);

        }

    }
    
   */
}
