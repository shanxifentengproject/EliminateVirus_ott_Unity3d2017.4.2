using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;
using UnityEngine;
class UniCameraPerformanceKindControl
{
    //支持的最大摄像机数
    private const int SupportMaxCameraCount = 4;
    public delegate void HandlePerformanceProc(Camera[] camera,Transform[] cameraTransform, ref PerformanceCheckData performanceCheckData);
    public struct PerformanceCheckData
    {
        //归属类型
        public UniCameraPerformanceCookieType cookieType;
        //检测区域
        public float checkRange;
        //摄像机前检测区域
        public float forntRange;
        //摄像机后检测区域
        public float backRange;
        public float sqrcheckRange;
        public float sqrforntRange;
        public float sqrbackRange;
        //处理函数
        public HandlePerformanceProc handleproc;
        //处理间隔
        public UniTimeLocker lockTime;
        public PerformanceCheckData(UniCameraPerformanceCookieType type, HandlePerformanceProc proc,
                    float checkrange,float forntrange,float backrange,float delayTime)
        {
            cookieType = type;
            checkRange = checkrange;
            forntRange = forntrange;
            backRange = backrange;
            sqrcheckRange = checkRange * checkRange;
            sqrforntRange = forntRange * forntRange;
            sqrbackRange = backRange * backRange;
            handleproc = proc;
            lockTime = new UniTimeLocker(delayTime);
        }
    }
    public List<PerformanceCheckData> performanceCheckDataList = new List<PerformanceCheckData>(8);
    public void AddPerformanceCheckData(ref PerformanceCheckData performanceCheckData)
    {
        performanceCheckDataList.Add(performanceCheckData);
    }
    public void HandlePerformance(Camera[] camera, Transform[] cameraTransform)
    {
        for (int i = 0; i < performanceCheckDataList.Count;i++ )
        {
            PerformanceCheckData checkData = performanceCheckDataList[i];
            if (checkData.lockTime.IsLocked)
                continue;
            checkData.handleproc(camera, cameraTransform, ref checkData);
            checkData.lockTime.IsLocked = true;
            performanceCheckDataList[i] = checkData;
        }
    }


    private Vector3[] cameraLookAtVector = new Vector3[SupportMaxCameraCount];
    private Vector3[] cameraPosition = new Vector3[SupportMaxCameraCount];
    public  void HandlePerformanceCommonProc(Camera[] camera, Transform[] cameraTransform, ref PerformanceCheckData performanceCheckData)
    {
        List<UniCameraPerformanceCookie> list = UniCameraPerformanceCookie.GetCookieList(performanceCheckData.cookieType);
        if (list == null)
            return;
        Vector3 cameraViewportPoint = new Vector3(0.5f, 0.5f, 0.0f);
        int cameraCount = camera.Length;

        for (int i = 0;i< cameraCount; i++)
        {
            cameraLookAtVector[i] = camera[i].ViewportPointToRay(cameraViewportPoint).direction;
            cameraPosition[i] = cameraTransform[i].position;
        }
        for (int i = 0; i < list.Count;i++ )
        {
            UniCameraPerformanceCookie cookie = list[i];
            //如果这个灯处于关闭状态则不处理了
            if (!cookie.enabled)
                continue;
            Vector3 cookiePosition = cookie.transform.position;

            UniCameraPerformanceCookie.PerformanceType checkType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
            for (int j = 0;j<cameraCount;j++)
            {
                //计算摄像机到这个灯光的向量
                if (Mathf.Abs(cookiePosition.x - cameraPosition[j].x) > performanceCheckData.checkRange)
                {
                    checkType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
                }
                else if (Mathf.Abs(cookiePosition.z - cameraPosition[j].z) > performanceCheckData.checkRange)
                {
                    checkType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
                }
                else
                {
                    Vector3 direction = cookiePosition - cameraPosition[j];
                    ////计算出向量的长度
                    float distance = direction.sqrMagnitude;
                    if (distance > performanceCheckData.sqrcheckRange)
                    {
                        checkType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
                        
                    }
                    else
                    {
                        //需要计算这个灯光在摄像机的前面还是背面
                        float dot = Vector3.Dot(direction, cameraLookAtVector[j]);
                        if (dot >= 0.0f)//在前面
                        {
                            if (distance > performanceCheckData.sqrforntRange)
                            {
                                checkType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
                                
                            }
                        }
                        else//在后面
                        {
                            if (distance > performanceCheckData.sqrbackRange)
                            {
                                checkType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
                                
                            }
                        }
                        checkType = UniCameraPerformanceCookie.PerformanceType.Performance_Hight;
                        //已经有一个检测需要打开对象，则不再检测后面的了
                        break;
                    }   
                }
            }
            //根据最好计算出来的检测类型赋值
            cookie.performanceType = checkType;

            ////计算摄像机到这个灯光的向量
            //if (Mathf.Abs(cookiePosition.x - cameraPosition.x) > performanceCheckData.checkRange)
            //{
            //    cookie.performanceType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
            //    continue;
            //}
            //else if (Mathf.Abs(cookiePosition.z - cameraPosition.z) > performanceCheckData.checkRange)
            //{
            //    cookie.performanceType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
            //    continue;
            //}
            //Vector3 direction = cookiePosition - cameraPosition;
            //////计算出向量的长度
            //float distance = direction.sqrMagnitude;
            //if (distance > performanceCheckData.sqrcheckRange)
            //{
            //    cookie.performanceType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
            //    continue;
            //}
            ////需要计算这个灯光在摄像机的前面还是背面
            //float dot = Vector3.Dot(direction, cameraLookAtVector);
            //if (dot >= 0.0f)//在前面
            //{
            //    if (distance > performanceCheckData.sqrforntRange)
            //    {
            //        cookie.performanceType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
            //        continue;
            //    }
            //}
            //else//在后面
            //{
            //    if (distance > performanceCheckData.sqrbackRange)
            //    {
            //        cookie.performanceType = UniCameraPerformanceCookie.PerformanceType.Performance_Stop;
            //        continue;
            //    }
            //}
            //cookie.performanceType = UniCameraPerformanceCookie.PerformanceType.Performance_Hight;
        }
    }
}
