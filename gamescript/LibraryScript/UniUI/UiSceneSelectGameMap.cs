using System;
using System.Collections.Generic;
using UnityEngine;

class UiSceneSelectGameMap : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameMap; } }

    public GameObject processObject;
    public GameObject lockObject;
    public GuiPlaneAnimationText curLevelNumber;
    public GuiPlaneAnimationText maxLevelNumber;
    public GuiPlaneAnimationProgressBar levelProgressBar;
    public GameObject[] selectPosition;

    public GuiPlaneAnimationPlayer moveAni;
    public Transform[] maplist;


    public GameObject arrowsright;
    public GameObject arrowsleft;

    private int currentSelectMapIndex = -1;

    private GuiExtendButtonGroup buttonGroup = null;
    protected override void OnInitializationUI()
    {
        buttonGroup = GetComponent<GuiExtendButtonGroup>();
        buttonGroup.selectFuntion += OnButtonSelectOk;
        //先停止工作
        buttonGroup.IsWorkDo = false;
        //定位到最后解锁的地图上
        SetCurrentSelectMapIndex(IGamerProfile.Instance.getLastLockedMap);
    }
    private void OnButtonSelectOk(int index)
    {
        SoundEffectPlayer.Play("buttonok.wav");
        //获取最后解锁的地图
        int lastLockMapIndex = IGamerProfile.Instance.getLastLockedMap;
        if (lastLockMapIndex < currentSelectMapIndex)
        {
            SetCurrentSelectMapIndex(lastLockMapIndex);
            return;
        }
        
        //设置当前地图
        IGamerProfile.Instance.gameEviroment.mapIndex = currentSelectMapIndex;
        IGamerProfile.Instance.gameEviroment.mapLevelIndex = IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex];
        if (IGamerProfile.Instance.gameEviroment.mapLevelIndex >= IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex])
        {
            IGamerProfile.Instance.gameEviroment.mapLevelIndex = IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex] - 1;
        }


        if ((GameCenterEviroment.platformChargeIntensity >= GameCenterEviroment.PlatformChargeIntensity.Intensity_VeryHigh) &&
                IGameCenterEviroment.effectCharacterLevelSale)
        {
            //让本身停止工作
            buttonGroup.IsWorkDo = false;
            LoadResource_UIPrefabs("characterlevelsale.prefab");
        }
        else
        {
            IntoGame();
        }
    }
    private GuiPlaneAnimationPlayer tempMoveAni = null;
    private void SetCurrentSelectMapIndex(int index)
    {
        if (currentSelectMapIndex == index)
            return;
        buttonGroup.IsWorkDo = false;

        tempMoveAni = ((GameObject)UnityEngine.Object.Instantiate(moveAni.gameObject)).GetComponent<GuiPlaneAnimationPlayer>();
        tempMoveAni.transform.parent = this.transform;
        tempMoveAni.IsAutoDel = false;
        tempMoveAni.playMode = GuiPlaneAnimationPlayer.PlayMode.Mode_PlayOnec;
        tempMoveAni.DelegateOnPlayEndEvent += OnMoveAniPlayEnd;
        //增加关键帧
        GuiPlaneAnimationCurvePosition curvePosition = tempMoveAni.gameObject.GetComponentInChildren<GuiPlaneAnimationCurvePosition>();
        curvePosition.xCurve = UnityEngine.AnimationCurve.EaseInOut(0.0f, maplist[0].position.x,
                                1.0f, maplist[0].position.x - maplist[index].position.x);
        //将对象定位到开始坐标对象
        curvePosition.gameObject.transform.position = maplist[0].position;
        maplist[0].transform.parent.parent = curvePosition.gameObject.transform;
        //重新标记索引
        currentSelectMapIndex = index;
        //开始播放
        tempMoveAni.Play();
    }
    private void OnMoveAniPlayEnd()
    {
        //将地图对象移出来
        maplist[0].transform.parent.parent = tempMoveAni.transform.parent;
        UnityEngine.Object.DestroyObject(tempMoveAni.gameObject);
        tempMoveAni = null;
        //刷新当前选择属性
        UpdateCurrentSelectMapData(currentSelectMapIndex);
    }
    private void UpdateCurrentSelectMapData(int index)
    {
        //获取最后解锁的地图
        int lastLockMapIndex = IGamerProfile.Instance.getLastLockedMap;
        //这个地图是锁定的
        if (lastLockMapIndex < index)
        {
            processObject.SetActive(false);
            lockObject.SetActive(true);
        }
        else
        {
            lockObject.SetActive(false);
            processObject.SetActive(true);
            //刷新数据
            curLevelNumber.Text = IGamerProfile.Instance.playerdata.levelProcess[index].ToString();
            maxLevelNumber.Text = IGamerProfile.gameLevel.mapMaxLevel[index].ToString();
            levelProgressBar.SetProgressBar((float)IGamerProfile.Instance.playerdata.levelProcess[index] / (float)IGamerProfile.gameLevel.mapMaxLevel[index]);
        }
        for (int i=0;i<selectPosition.Length;i++)
        {
            selectPosition[i].SetActive(false);
        }
        selectPosition[index].SetActive(true);

        if (index == 0)
        {
            arrowsright.SetActive(true);
            arrowsleft.SetActive(false);
        }
        else if (index == IGamerProfile.gameLevel.mapData.Length - 1)
        {
            arrowsright.SetActive(false);
            arrowsleft.SetActive(true);
        }
        else
        {
            arrowsright.SetActive(true);
            arrowsleft.SetActive(true);
        }

        buttonGroup.IsWorkDo = true;
    }

    //需要重载Input刷新函数
    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public override bool OnInputUpdate() 
    {
        if (InputDevice.ButtonLeft && buttonGroup.IsWorkDo)
        {
            SoundEffectPlayer.Play("scroll.wav");
            if (currentSelectMapIndex > 0)
            {
                SetCurrentSelectMapIndex(currentSelectMapIndex - 1);
            }
            return false;
        }
        else if (InputDevice.ButtonRight && buttonGroup.IsWorkDo)
        {
            SoundEffectPlayer.Play("scroll.wav");
            if (currentSelectMapIndex < maplist.Length - 1)
            {
                SetCurrentSelectMapIndex(currentSelectMapIndex + 1);
            }
            return false;
        }
        else if (InputDevice.ButtonBack)
        {
            SoundEffectPlayer.Play("buttonok.wav");
            //闪白
            ((UiSceneUICamera)UIManager).FadeScreen();
            //进入角色选择界面
            ((UiSceneUICamera)UIManager).CreateAloneScene(UiSceneUICamera.UISceneId.Id_UIGameCharacter);
            return false;
        }
        return true; 
    }

    //一个子UI删除
    public override void OnChildUIRelease(GuiUiSceneBase ui)
    {
        if (ui is UiSceneCharacterLevelSale)
        {
            IntoGame();
        }
    }


    public void IntoGame()
    {
        //闪白
        //((UiSceneUICamera)UIManager).FadeScreen();
        ////设置为进入游戏模式
        //UiSceneSelectGameCharacter.selectCharacterMode = UiSceneSelectGameCharacter.SelectCharacterMode.Mode_IntoGame;
        ////进入角色选择界面
        //((UiSceneUICamera)UIManager).CreateAloneScene(UiSceneUICamera.UISceneId.Id_UIGameCharacter);
        //进入加载界面
        ((UiSceneUICamera)UIManager).CreatePoolingScene(UiSceneUICamera.UISceneId.Id_UIGameLoading, UiSceneGameLoading.LoadingType.Type_LoadingGameNew);
        //删除主UI界面
        ((UiSceneUICamera)UIManager).ReleaseAloneScene();
    }
}
