using UnityEngine;
using System.Collections;

struct UniTimeLocker
{
    private float time;
    private float nexttime;
    public UniTimeLocker(float t)
    {
        time = t;
        nexttime = 0.0f;
    }
    public bool IsLocked
    {
        get
        {
            return nexttime > Time.time;
        }
        set
        {
            if (value)
            {
                nexttime = Time.time + time;
            }
            else
            {
                nexttime = 0.0f;
            }
        }
    }
}
