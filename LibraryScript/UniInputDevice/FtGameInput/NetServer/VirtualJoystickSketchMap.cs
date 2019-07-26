using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class VirtualJoystickSketchMap : MonoBehaviour
{

    public GameObject[] select_key = null;
    // Use this for initialization
    void Start ()
    {
        Reset();
    }
    public void Reset()
    {
        if (select_key != null)
        {
            for (int i = 0; i < select_key.Length; i++)
            {
                if (select_key[i] != null)
                {
                    select_key[i].SetActive(false);
                }
            }
        }
    }
    public void SetKeyStatus(FtGameInput.NetInputKeyCode key, FtGameInput.NetInputKeyStatus status)
    {
        if (select_key[(int)key] == null)
            return;
        switch(status)
        {
            case FtGameInput.NetInputKeyStatus.Status_Down:
            case FtGameInput.NetInputKeyStatus.Status_Press:
                select_key[(int)key].SetActive(true);
                break;
            case FtGameInput.NetInputKeyStatus.Status_Up:
            case FtGameInput.NetInputKeyStatus.Status_Nothing:
                select_key[(int)key].SetActive(false);
                break;
            
        }
    }
	
}
