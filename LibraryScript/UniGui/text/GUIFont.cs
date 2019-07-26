using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/Text/GUIFont")]
class GUIFont : MonoBehaviourIgnoreGui
{
    public enum ShadeType
    {
        Shade_NULL = 0,
        Shade_Right = 1,
        Shade_Full = 2
    }
    //是否使用阴影
    public ShadeType shadeType = ShadeType.Shade_NULL;
    //设置字体大小
    //设置偏移
    public int m_OffsetSize = 1;
    public GUIText m_MainFont = null;
    //阴影的材质球
    public Material m_ShadeMaterial = null;
    //阴影的字体
    protected GUIText[] m_pShadeFont = null;
    //设置的字符
    public string srcText = "";
    public string Text
    {
        get
        {
            return m_MainFont.text;
        }
        set
        {
            if (m_MainFont.text == value)
                return;
            m_MainFont.text = value;
            if (m_pShadeFont != null)
            {
                for (int i = 0; i < m_pShadeFont.Length; i++)
                {
                    m_pShadeFont[i].text = value;
                }
            }
        }
    }

    
    protected override void Awake()
    {
        if (m_MainFont == null)
            return;
        switch (shadeType)
        {
            case ShadeType.Shade_NULL:
                m_pShadeFont = null;
                break;
            case ShadeType.Shade_Right:
                {
                    if (m_pShadeFont != null)
                    {
                        m_pShadeFont = new GUIText[1];
                        GameObject obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                        obj.transform.parent = transform;
                        m_pShadeFont[0] = obj.GetComponent<GUIText>();
                        m_pShadeFont[0].pixelOffset = new Vector2(m_MainFont.pixelOffset.x + m_OffsetSize, m_MainFont.pixelOffset.y - m_OffsetSize);
                        m_pShadeFont[0].material = m_ShadeMaterial;
                    }
                }
                break;
            case ShadeType.Shade_Full:
                if (m_pShadeFont != null)
                {
                    m_pShadeFont = new GUIText[8];
                    GameObject obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[0] = obj.GetComponent<GUIText>();
					m_pShadeFont[0].pixelOffset = new Vector2(m_MainFont.pixelOffset.x - m_OffsetSize, m_MainFont.pixelOffset.y + m_OffsetSize);
                    m_pShadeFont[0].material = m_ShadeMaterial;

                    obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[1] = obj.GetComponent<GUIText>();
                    m_pShadeFont[1].pixelOffset = new Vector2(m_MainFont.pixelOffset.x + m_OffsetSize, m_MainFont.pixelOffset.y - m_OffsetSize);
                    m_pShadeFont[1].material = m_ShadeMaterial;

                    obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[2] = obj.GetComponent<GUIText>();
                    m_pShadeFont[2].pixelOffset = new Vector2(m_MainFont.pixelOffset.x - m_OffsetSize, m_MainFont.pixelOffset.y - m_OffsetSize);
                    m_pShadeFont[2].material = m_ShadeMaterial;

                    obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[3] = obj.GetComponent<GUIText>();
                    m_pShadeFont[3].pixelOffset = new Vector2(m_MainFont.pixelOffset.x + m_OffsetSize, m_MainFont.pixelOffset.y + m_OffsetSize);
                    m_pShadeFont[3].material = m_ShadeMaterial;

                    obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[4] = obj.GetComponent<GUIText>();
                    m_pShadeFont[4].pixelOffset = new Vector2(m_MainFont.pixelOffset.x + m_OffsetSize, m_MainFont.pixelOffset.y);
                    m_pShadeFont[4].material = m_ShadeMaterial;

                    obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[5] = obj.GetComponent<GUIText>();
                    m_pShadeFont[5].pixelOffset = new Vector2(m_MainFont.pixelOffset.x - m_OffsetSize, m_MainFont.pixelOffset.y);
                    m_pShadeFont[5].material = m_ShadeMaterial;

                    obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[6] = obj.GetComponent<GUIText>();
                    m_pShadeFont[6].pixelOffset = new Vector2(m_MainFont.pixelOffset.x, m_MainFont.pixelOffset.y + m_OffsetSize);
                    m_pShadeFont[6].material = m_ShadeMaterial;

                    obj = (GameObject)GameObject.Instantiate(m_MainFont.gameObject, m_MainFont.gameObject.transform.position, m_MainFont.gameObject.transform.rotation);
                    obj.transform.parent = transform;
                    m_pShadeFont[7] = obj.GetComponent<GUIText>();
                    m_pShadeFont[7].pixelOffset = new Vector2(m_MainFont.pixelOffset.x, m_MainFont.pixelOffset.y - m_OffsetSize);
                    m_pShadeFont[7].material = m_ShadeMaterial;
                }
                break;
        }
    }
    protected virtual void Start()
    {
        Text = srcText;
    }
}

