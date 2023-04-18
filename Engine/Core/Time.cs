using System.Diagnostics;

namespace GraphicsProgramming.Engine.Core;

public static class Time
{
    public static double deltaTime { get; private set; }
    public static double time { get; private set; }
    
    
    private static double lastTime = 0f;
    
    public static void Tick()
    {
        if (lastTime == 0) lastTime = NanoTime();
        deltaTime = (NanoTime() - lastTime) / 1000d / 1000d / 1000d;
        lastTime = NanoTime();
        time += deltaTime;
    }
    
    private static long NanoTime() {
        long nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;
        return nano;
    }

    public static void Reset()
    {
        time = 0f;
        deltaTime = 0f;
        lastTime = 0f;
    }
}