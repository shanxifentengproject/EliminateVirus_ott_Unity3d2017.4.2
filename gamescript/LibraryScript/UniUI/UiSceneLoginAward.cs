using UnityEngine;
using System.Collections;
class UiSceneLoginAward : GuiUiSceneBase 
{

    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameLoginAward; } }

    public GuiPlaneAnimationText payMoney;
    public GameObject[] equipposition;
    public GuiPlaneAnimationText[] skillCount;

    private int[] equipIndexList = null;
    private GameObject[] awardIcon = null;

    protected override void OnInitializationUI()
    {
        GuiExtendDialog dlg = GetComponent<GuiExtendDialog>();
        if (dlg != null)
        {
            dlg.callbackFuntion += OnDialogReback;
            dlg.buttonSelectStatus = ( GuiExtendDialog.DialogFlag ) IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_LoginAward_BtnIndex;
        }
        payMoney.Text = IGamerProfile.gameBaseDefine.gameParameter.loginAward.paymoney.ToString();
        for (int i = 0; i < skillCount.Length; i++)
        {
            skillCount[i].Text = IGamerProfile.gameBaseDefine.gameParameter.loginAward.skill[i].ToString();
        }

        awardIcon = new GameObject[IGamerProfile.gameBaseDefine.gameParameter.loginAward.equip.Length];
        equipIndexList = new int[awardIcon.Length];
        for (int i=0;i<awardIcon.Length;i++)
        {
            int equipIndex = FTLibrary.Command.FTRandom.Next(GameEquip.EquipMaxCount);
            equipIndexList[i] = equipIndex;
            awardIcon[i] = LoadResource_UIPrefabs(string.Format("equip{0}icon.prefab", equipIndex + 1));
            GuiPlaneAnimationText num = awardIcon[i].GetComponentInChildren<GuiPlaneAnimationText>();
            num.Text = IGamerProfile.gameBaseDefine.gameParameter.loginAward.equip[i].ToString();
            awardIcon[i].transform.localPosition = equipposition[i].transform.localPosition;
        }
    }
    private void OnDialogReback(int dialogid, GuiExtendDialog.DialogFlag ret)
    {
        switch (ret)
        {
            case GuiExtendDialog.DialogFlag.Flag_Cancel:
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    UnityEngine.Object.DestroyObject(this.gameObject);
                }
                break;
            case GuiExtendDialog.DialogFlag.Flag_Ok:
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_LoginAward,
                                           IGamerProfile.gameBaseDefine.gameParameter.loginAward.paymoney,
                                           0,
                                           PayMoneyCallback), this);
                }
                break;
        }

    }
    private void PayMoneyCallback(IGamerProfile.PayMoneyData paydata, bool isSucceed)
    {
        switch (paydata.item)
        {
            case IGamerProfile.PayMoneyItem.PayMoneyItem_LoginAward:
                {
                    if (!isSucceed)
                    {
                        UnityEngine.Object.DestroyObject(this.gameObject);
                        return;
                    }
                    for (int i = 0; i < IGamerProfile.gameBaseDefine.gameParameter.loginAward.skill.Length; i++)
                    {
                        IGamerProfile.Instance.playerdata.skillCount[i] += IGamerProfile.gameBaseDefine.gameParameter.loginAward.skill[i];
                    }
                    //处理档案
                    for (int i = 0; i < equipIndexList.Length;i++ )
                    {
                        IGamerProfile.Instance.playerdata.equipCount[equipIndexList[i]] +=
                                IGamerProfile.gameBaseDefine.gameParameter.loginAward.equip[i];
                    }
                    IGamerProfile.Instance.SaveGamerProfileToServer();
                    UnityEngine.Object.DestroyObject(this.gameObject);
                }
                break;

        }
    }
}
