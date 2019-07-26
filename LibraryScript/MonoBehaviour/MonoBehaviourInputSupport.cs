using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 * 由于系统刷新是从根节点向子节点刷新的，对输入控制会导致次序错误
 * 使用静态变量进行标记虽然可以解决问题，但是代码会比较混乱
 * 所以所有继承与这个类的子类在初始化的时候会挂到输入刷新周期上
 * 并且根据提供的层进行次序刷新，处理输入刷新还可以截断，其他对象不再响应输入信息
 * */
abstract class MonoBehaviourInputSupport : MonoBehaviourIgnoreGui
{
    public enum InputUpdateLevel
    {
        //最高刷新级别
        InputUpdateLevel_High = 0,
        //UI刷新级别
        InputUpdateLevel_UI_1 = 200,
        InputUpdateLevel_UI_2 = 400,
        InputUpdateLevel_UI_3 = 600,
        InputUpdateLevel_UI_4 = 800,
        InputUpdateLevel_UI_5 = 1000,
        //游戏场景刷新级别
        InputUpdateLevel_Scene_1 = 1300,
        InputUpdateLevel_Scene_2 = 1600,
        InputUpdateLevel_Scene_3 = 2000,
        //最低刷新级别
        InputUpdateLevel_Lower = 3000,
    }
    //当前刷新级别
    public InputUpdateLevel CurrentInputUpdateLevel = InputUpdateLevel.InputUpdateLevel_UI_5;

    protected virtual void Start()
    {
        if (InputDeviceBase.currentInputDevice != null)
        {
            InputDeviceBase.currentInputDevice.AddInputUpdateList(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (InputDeviceBase.currentInputDevice != null)
        {
            InputDeviceBase.currentInputDevice.DelInputUpdateList(this);
        }
    }

    //需要重载Input刷新函数
    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public virtual bool OnInputUpdate(){return true;}
}
