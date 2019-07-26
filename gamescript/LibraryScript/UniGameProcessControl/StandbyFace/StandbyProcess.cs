using System;
using System.Collections.Generic;
using UnityEngine;


class StandbyProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_Standby; } }
    public StandbyProcess()
        : base()
    {

    }
    //初始化函数
    public override void Initialization()
    {
        base.Initialization();
    }
}
