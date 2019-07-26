using UnityEngine;
using System.Collections;

class UiSceneCharacterLevelSale : GuiUiSceneBase
{

    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UICharacterLevelSale; } }
    public GuiPlaneAnimationText payMoney;
    protected override void OnInitializationUI()
    {
        GuiExtendDialog dlg = GetComponent<GuiExtendDialog>();
        if (dlg != null)
        {
            dlg.callbackFuntion += OnDialogReback;
            dlg.buttonSelectStatus = ( GuiExtendDialog.DialogFlag ) IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_CharacterLevelSale_BtnIndex;
        }

        int currentSelectCharacterIndex = IGamerProfile.Instance.gameEviroment.characterIndex;
        int money = IGamerProfile.gameCharacter.characterDataList[currentSelectCharacterIndex].LevelToMoney.GetValue(
                                                    IGamerProfile.Instance.playerdata.characterData[currentSelectCharacterIndex].level);
        payMoney.Text = money.ToString();
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
                    int currentSelectCharacterIndex = IGamerProfile.Instance.gameEviroment.characterIndex;
                    IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_LevelCharacter,
                                           IGamerProfile.gameCharacter.characterDataList[currentSelectCharacterIndex].LevelToMoney.GetValue(
                                                    IGamerProfile.Instance.playerdata.characterData[currentSelectCharacterIndex].level),
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
            case IGamerProfile.PayMoneyItem.PayMoneyItem_LevelCharacter:
                {
                    if (!isSucceed)
                    {
                        UnityEngine.Object.DestroyObject(this.gameObject);
                        return;
                    }
                    //处理档案
                    //增加角色级别
                    int currentSelectCharacterIndex = IGamerProfile.Instance.gameEviroment.characterIndex;
                    IGamerProfile.Instance.playerdata.characterData[currentSelectCharacterIndex].level += 1;
                    if (IGamerProfile.Instance.playerdata.characterData[currentSelectCharacterIndex].level > IGamerProfile.gameCharacter.characterDataList[currentSelectCharacterIndex].maxlevel)
                    {
                        IGamerProfile.Instance.playerdata.characterData[currentSelectCharacterIndex].level = IGamerProfile.gameCharacter.characterDataList[currentSelectCharacterIndex].maxlevel;
                    }
                    //存储档案
                    IGamerProfile.Instance.SaveGamerProfileToServer();
                    UnityEngine.Object.DestroyObject(this.gameObject);
                }
                break;

        }
    }
}
