using System.Runtime.InteropServices;

public static class LogitechDriver
{
    private static bool initialized = false;

    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern bool LogiSteeringInitialize(bool ignoreXInput);
    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern void LogiSteeringShutdown();
    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern bool LogiUpdate();
    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern bool LogiIsConnected(int index);

    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern void LogiPlaySpringForce(int index, int offset, int saturation, int coef);
    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern void LogiStopSpringForce(int index);

    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern void LogiPlayDirtRoadEffect(int index, int magnitude);
    [DllImport("LogitechSteeringWheelEnginesWrapper")] private static extern void LogiStopDirtRoadEffect(int index);

    public static void Update()
    {
        if (!initialized)
        {
            LogiSteeringInitialize(false);
            initialized = true;

            UnityEngine.Application.quitting += Shutdown;
        }

        LogiUpdate();
    }

    public static void Shutdown()
    {
        if (initialized)
        {
            LogiSteeringShutdown();
            initialized = false;
        }
    }

    public static bool IsConnected() => LogiIsConnected(0);
    public static void PlaySpring(int o, int s, int c) => LogiPlaySpringForce(0, o, s, c);
    public static void StopSpring() => LogiStopSpringForce(0);
    public static void PlayDirt(int m) => LogiPlayDirtRoadEffect(0, m);
    public static void StopDirt() => LogiStopDirtRoadEffect(0);
}
