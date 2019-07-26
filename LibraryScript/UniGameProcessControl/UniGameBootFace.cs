using System;
using System.Collections.Generic;
using System.Text;
class UniGameBootFace : MonoBehaviourIgnoreGui
{
    public GuiPlaneAnimationPlayer bootFacePlayer = null;
    protected virtual void Start()
    {
        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
    }
    public virtual void CloseGameBootFace()
    {
        bootFacePlayer.DelegateOnPlayEndEvent = GameBootFacePlayEnd;
        bootFacePlayer.Play();
    }
    private void GameBootFacePlayEnd()
    {
        bootFacePlayer.DelegateOnPlayEndEvent = null;
        UnityEngine.Object.DestroyObject(this.gameObject);
    }
}
