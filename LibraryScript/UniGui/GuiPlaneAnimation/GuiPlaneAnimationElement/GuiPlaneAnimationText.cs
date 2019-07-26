using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
/*
 * 这个类的实现方法：
 * 子对象代表一个个字符，动画控制在父对象上进行
 * */

[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationText")]
class GuiPlaneAnimationText : GuiPlaneAnimationElement
{
    //字符转换表。数组的索引代表贴图帧所有。字符代表需要显示的字符
    //特殊字符空格
    private const char SpecialCharacters_Space = ' ';
    private const char SpecialCharacters_Point = '.';
    public enum CharactersTableType
    {
        Type_Number,                //数字的字符转换表,格式:[0~9](数字)[10](小数点)
        Type_LowerCharacters,       //小写字母,格式:[0~25](字母)
        Type_UpperCharacters,       //大写字母,格式:[0~25](字母)
        Type_Characters,            //大小写都有，格式:[0~25](小写字母)[26~51](大写字母)
        Type_NumberAndCharacters,   //贴图帧0,格式:[0~9](数字)[10](小数点)[11~36](小写字母)[37~62](大写字母)
        Type_ASCII128,              //前128字符，格式:[0~97]（从ASCII 30开始到127）
        Type_ASCII256,               //全部字符，格式:[0~224]（从ASCII 30开始到254）
        Type_Other                  //其他方式，自定义格式
    }
    public CharactersTableType charactersTableType = CharactersTableType.Type_Number;
    public string charactersTable = null;

    public enum CharactersAlignAt
    {
        Align_Left,
        Align_Centre,
        Align_Right
    }
    public CharactersAlignAt alignAt = CharactersAlignAt.Align_Left;

    protected string m_Text=" ";
    public string Text
    {
        get { return m_Text; }
        set
        {
            if (String.Equals(m_Text,value))
                return;
            m_Text = value;
            if (TextAnimationList == null)
                return;
            //计算填充位置
            int Index = AccountCharactersAlignAt();
            //首先索引之前的需要隐藏
            for (int i = 0; i < Index;i++ )
            {
                if (TextAnimationList[i].gameObject.activeSelf)
                {
                    TextAnimationList[i].gameObject.SetActive(false);
                }
            }
            //从这个索引开始填充
            for (int i = Index; i < TextAnimationList.Length;i++ )
            {
                int textIndex = i - Index;
                if (textIndex >= m_Text.Length)
                {
                    if (TextAnimationList[i].gameObject.activeSelf)
                    {
                        TextAnimationList[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    int frame = AccountCharactersFrame(m_Text[textIndex]);
                    if (frame == -1)
                    {
                        if (TextAnimationList[i].gameObject.activeSelf)
                        {
                            TextAnimationList[i].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (!TextAnimationList[i].gameObject.activeSelf)
                        {
                            TextAnimationList[i].gameObject.SetActive(true);
                        }
                        TextAnimationList[i].frame = frame;
                    }
                }
            }



            //for (int i = 0; i < TextAnimationList.Length;i++ )
            //{
            //    if (i < m_Text.Length)
            //    {
            //        int frame = AccountCharactersFrame(m_Text[i]);
            //        if (frame == -1)
            //        {
            //            if (TextAnimationList[i].gameObject.activeSelf)
            //            {
            //                TextAnimationList[i].gameObject.SetActive(false);
            //            }
            //            return;
            //        }
            //        if (!TextAnimationList[i].gameObject.activeSelf)
            //        {
            //            TextAnimationList[i].gameObject.SetActive(true);
            //        }
            //        TextAnimationList[i].frame = frame;
            //    }
            //    else
            //    {
            //        if (TextAnimationList[i].gameObject.activeSelf)
            //        {
            //            TextAnimationList[i].gameObject.SetActive(false);
            //        }
                    
            //    }
            //}
        }
    }

    //程序尽量别改颜色，否则每个对象都会克隆一份材质球
    protected Color m_TextColor = Color.white;
    public Color TextColor
    {
        get { return m_TextColor; }
        set
        {
            if (m_TextColor == value)
                return;
            m_TextColor = value;
            if (TextAnimationList == null)
                return;
            for (int i = 0; i < TextAnimationList.Length;i++ )
            {
                TextAnimationList[i].GetComponent<Renderer>().material.color = m_TextColor;
            }
        }
    }

/// <summary>
/// 独立场景时使用，及不切换场景的
/// </summary>
#if Independent_Scene
 
#else
    public override void OnDestroy()
    {
        base.OnDestroy();
        Renderer renderer;
        for (int i = 0; i < TextAnimationList.Length; i++)
        {
            renderer = TextAnimationList[i].GetComponent<Renderer>();
            if (renderer.material != null)
        	{
                UnityEngine.GameObject.Destroy(renderer.material);
        	}
            renderer.material = null;
            //UnityEngine.GameObject.Destroy(TextAnimationList[i].renderer);
        }
    }

#endif
   

    //这里的顺序自己拖，否则很可能保证不了顺序
    public GuiPlaneAnimationUVAnimation[] TextAnimationList = null;
    private int AccountCharactersFrame(char c)
    {
        if (c == SpecialCharacters_Space)
            return -1;
        switch(charactersTableType)
        {
            case CharactersTableType.Type_Number:
                {
                    if (c == SpecialCharacters_Point)
                        return 10;
                    int charValue=(int)c;
                    if (charValue < 48 || charValue > 57)
                        return -1;
                    return charValue - 48;
                }
            case CharactersTableType.Type_LowerCharacters:
                {
                    int charValue = (int)c;
                    if (charValue < 97 || charValue > 122)
                        return -1;
                    return charValue - 97;
                }
            case CharactersTableType.Type_UpperCharacters:
                {
                    int charValue = (int)c;
                    if (charValue < 65 || charValue > 90)
                        return -1;
                    return charValue - 65;
                }
            case CharactersTableType.Type_Characters:
                {
                    int charValue = (int)c;
                    if (charValue >= 97 && charValue <= 122)
                    {
                        return charValue - 97;
                    }
                    else if (charValue >= 65 && charValue <= 90)
                    {
                        return 26 + charValue - 65;
                    }
                    return -1; 
                }
            case CharactersTableType.Type_NumberAndCharacters:
                {
                    if (c == SpecialCharacters_Point)
                        return 10;
                    int charValue = (int)c;
                    if (charValue >= 48 && charValue <= 57)
                    {
                        return charValue - 48;
                    }
                    else if (charValue >= 97 && charValue <= 122)
                    {
                        return 11+charValue - 97;
                    }
                    else if (charValue >= 65 && charValue <= 90)
                    {
                        return 11 + 26 + charValue - 65;
                    }
                    return -1; 
                }
            case CharactersTableType.Type_ASCII128:
                {
                    int charValue = (int)c;
                    if (charValue < 30 || charValue > 127)
                        return -1;
                    return 97 - (charValue - 30);
                }
            case CharactersTableType.Type_ASCII256:
                {
                    int charValue = (int)c;
                    if (charValue < 30 || charValue > 254)
                        return -1;
                    return 224 - (charValue - 30);
                }
            case CharactersTableType.Type_Other:
                {
                    if (charactersTable == null)
                        return -1;
                    for (int i = 0; i < charactersTable.Length;i++ )
                    {
                        if (charactersTable[i] == c)
                            return i;
                    }
                    return -1;
                }
                
        }
        return -1;
    }
    //计算字符串的对齐方式返回应该从那一个索引开始填充
    private int AccountCharactersAlignAt()
    {
        if (alignAt == CharactersAlignAt.Align_Left)
        {
            return 0;
        }
        else if (alignAt == CharactersAlignAt.Align_Right)
        {
            int ret = TextAnimationList.Length - m_Text.Length;
            if (ret < 0)
            {
                ret = 0;
            }
            return ret;
        }
        else if (alignAt == CharactersAlignAt.Align_Centre)
        {
            int ret;
            int showCharacterCount;
            if (m_Text.Length < TextAnimationList.Length)
            {
                ret = (TextAnimationList.Length - m_Text.Length) / 2;
                showCharacterCount = m_Text.Length;
            }
            else
            {
                ret = 0;
                showCharacterCount = TextAnimationList.Length;
            }
            //1位的时候没有办法进行对齐，无法计算出宽高
            if (TextAnimationList.Length == 1)
                return ret;
            //需要进行一次对位
            //首先计算一个字符的宽高
            //高没有用
            float characterWidth = TextAnimationList[1].transform.localPosition.x - TextAnimationList[0].transform.localPosition.x;
            //首先都使用奇数计算方法
            //这里的索引是指向中间的对象
            //先计算奇数的原因是面片不是右上顶点对齐的，是中心对齐的
            int CenterIndex = showCharacterCount / 2;
            //把这个索引转换到位置索引上去
            CenterIndex += ret;
            for (int i = 0; i < TextAnimationList.Length; i++)
            {
                Vector3 v = TextAnimationList[i].transform.localPosition;
                v.x = (i - CenterIndex) * characterWidth;
                TextAnimationList[i].transform.localPosition = v;
            }
            ////如果显示的字符是奇数会发生错半个字符，这时候需要将字符串整体移动半个字符
            //如果是偶数会由于是中心对象会错多错半个字符，所以这里要加回来
            if (showCharacterCount % 2 == 0)
            {
                float halfCharacterWidth = characterWidth / 2.0f;
                for (int i = 0; i < TextAnimationList.Length; i++)
                {
                    Vector3 v = TextAnimationList[i].transform.localPosition;
                    v.x += halfCharacterWidth;
                    TextAnimationList[i].transform.localPosition = v;
                }
            }
            return ret;
                
        }
        else
        {
            return 0;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        Text = "";
    }
}
