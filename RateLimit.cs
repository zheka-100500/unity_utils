using System;
using System.Diagnostics;

public class RateLimit
{
    private static Stopwatch timer;

    public static RateLimit New(int count, float time)
    {
        return New(count, time, null);
    }
    public static RateLimit New(int count, float time, Action<int> onLimitHit)
    {
        if (timer != null) return new RateLimit(count, time, onLimitHit);
        timer = new Stopwatch();
        timer.Start();
        return new RateLimit(count, time, onLimitHit);
    }

    private int count;
    private float time;
    private Action<int> onHit;

    private int curCount = 0;
    private float nextActivationTime = -1;
    private int hitCount = 0;
    private RateLimit(int count, float time, Action<int> onLimitHit)
    {
        this.count = count;
        this.time = time;
        this.onHit = onLimitHit;
    }

    public bool AllowHandle()
    {
        var total = (float) timer.Elapsed.TotalSeconds;
        if (nextActivationTime <= -1)
        {
            goto activate;
        }
        if (nextActivationTime > total)
        {
            if (curCount >= count)
            {
                hitCount++;
                onHit?.Invoke(hitCount);
                return false;
            }
            curCount++;
            return true;
        }
        activate:
        hitCount = 0;
        nextActivationTime = total + time;
        curCount = 1;
        return true;
    }

}
