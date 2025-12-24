using GLTech.Scripting;
using GLTech.Space;
using System.Diagnostics;

namespace wolf3d;

public class TrajectoryRecorder : Script
{
    const int INITIAL_CAPACITY = 400;

    List<Vector>? bestRun = null;
    List<Vector>? playbackRun = null;
    List<Vector> currentRun = new(INITIAL_CAPACITY);
    Stopwatch sw = new();
    float? bestRunTime = null;
    bool recording = false;
    float keyframeInterval;

    float Time { get => sw.ElapsedMilliseconds / 1000f; }

    public TrajectoryRecorder(float keyframeInterval = 1f)
    {
        this.keyframeInterval = keyframeInterval;
    }

    public void StartRecording()
    {
        recording = true;
        currentRun.Clear();
        playbackRun = bestRun;
        sw.Restart();
    }

    public void FinishRecording()
    {
        sw.Stop();
        recording = false;

        var currentRunTime = Time;
        if (bestRun == null || currentRunTime < bestRunTime)
        {
            bestRun = currentRun;
            bestRunTime = currentRunTime;
        }
        currentRun = new List<Vector>(INITIAL_CAPACITY);
    }


    void Start()
    {
        Entity.WorldPosition = (float.PositiveInfinity, float.PositiveInfinity);
    }

    void Update()
    {
        if (!recording) return;
        Record();
        PlayBack();
    }

    void Record()
    {
        int keyframesRecorded = currentRun.Count;
        float keyframeIntervals = Time / keyframeInterval;
        if (keyframesRecorded < keyframeIntervals)
            currentRun.Add(Scene.Camera.WorldPosition);
    }

    void PlayBack()
    {
        if (playbackRun == null) return;
        float currentFrame = Time / keyframeInterval;
        if (currentFrame >= playbackRun.Count - 1)
        {
            Entity.WorldPosition = playbackRun[playbackRun.Count - 1];
            return;
        }

        int floorKeyframe = (int)currentFrame;
        int ceilingKeyFrame = floorKeyframe + 1;

        float progression = currentFrame - floorKeyframe;
        Vector previous = playbackRun[floorKeyframe];
        Vector next = playbackRun[ceilingKeyFrame];
        Vector currentPos = progression * next + (1 - progression) * previous;
        Entity.WorldPosition = currentPos;
    }
}