using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 * 注意这个类不要常驻内存，在使用的模块内使用
 * 退出后删除，昵称数组较大
 * */
class UniNameCreate
{
    private string[] nameList = null;
    //构造的时候读取昵称数据
    public UniNameCreate()
    {
        string text = UniGameResources.currentUniGameResources.LoadLanguageResource_TextFile("namedata.txt", Encoding.UTF8);
        nameList = text.Split('\n');
    }
    //随机产生昵称的时候，可以提供一个忽略索引，比如我的昵称索引，这样，就不会产生和我一样的昵称了
    //如果不需要填-1
    public int RandomName(int avoidIndex)
    {
        int index;
        do 
        {
            index = FTLibrary.Command.FTRandom.Next(nameList.Length - 1);
        } while (index == avoidIndex);
        return index;
    }
    public string GetName(int index)
    {
        string ret = nameList[index];
        return ret.Substring(0, ret.Length - 1);
    }

    public List<int> CreateBatchRandomList()
    {
        List<int> ret = new List<int>(16);
        for (int i = 0;i<nameList.Length;i++)
        {
            ret.Add(i);
        }
        return ret;
    }
    public void RemoveBatchRandomListIndex(List<int> list,int index)
    {
        for (int i = 0;i<list.Count;i++)
        {
            if (list[i] == index)
            {
                list.RemoveAt(i);
                return;
            }
        }
    }
    public int RandomBatchRandomListIndex(List<int> list)
    {
        int index = FTLibrary.Command.FTRandom.Next(list.Count);
        int ret = list[index];
        list.RemoveAt(index);
        return ret;
    }
}
