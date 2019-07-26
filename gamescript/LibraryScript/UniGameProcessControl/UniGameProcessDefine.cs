using System;
using System.Collections.Generic;
using System.Text;
//这个类型后面会被项目正式的定义替换掉
enum ModalProcessType
{
    Process_Unknow,

    Process_SelfChecking,
    Process_GameOptions,        //选项过程
    Process_CompanyLogo,    //公司LOGO

    Process_Standby,        //待机状态
    Process_SelectDrago,
    Process_GameRace,       //进行游戏
    Process_GameOver,       //结束游戏
}
