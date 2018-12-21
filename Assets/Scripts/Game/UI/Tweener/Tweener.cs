﻿using UnityEngine;
using UnityEngine.Events;

public enum LoopType
{
    Loop = 0,
    PingPang
}

[System.Serializable]
public abstract class Tweener : ITimerObject
{
    public bool enabled;
    public AnimationCurve animationCurve;
    public int startDelay;
    public int duration;
    public bool isLoop;
    public int loops;
    public LoopType loopType;
    public bool ignoreTimeScale;
    public GameObject target;

    public UnityEvent onFinished;

    protected int _duration;
    protected int _loops;

    int _direction;

    bool _isPlaying;

    TimerEvent _timerEvent;

    public bool IsPlaying
    {
        get { return _isPlaying; }
    }

    public float TotalDuration
    {
        get
        {
            return enabled ? startDelay + duration : 0;
        }
    }

    public Tweener()
    {
        animationCurve = new AnimationCurve();
        loops = -1;
        loopType = LoopType.Loop;

        _direction = 1;
    }

    public void Stop()
    {
        if (!_isPlaying)
        {
            return;
        }

        _isPlaying = false;

        _timerEvent.Clear();
        _timerEvent = null;
    }
    public void ForcePlay()
    {
        Stop();

        Play();
    }

    public void Play()
    {
        if (_isPlaying)
        {
            return;
        }

        _isPlaying = true;

        _duration = 0;
        _loops = 0;

        _timerEvent = WorldManager.Instance.TimerMgr.AddEndLess(startDelay, 0, this);
    }

    void AnimationFinished()
    {
        Stop();

        if (onFinished != null)
        {
            onFinished.Invoke();
            onFinished.RemoveAllListeners();
        }
    }

    public void Tick()
    {
        var gameSystemData = WorldManager.Instance.GameCore.GetData<Data.GameSystemData>();
        var deltaTime = gameSystemData.unscaleDeltaTime;

        Update(deltaTime);

        _duration += deltaTime * _direction;

        if (isLoop)
        {
            if (_duration > duration || _duration < 0)
            {

                if (loopType == LoopType.Loop)
                {
                    _duration = 0;
                    _direction = 1;
                }
                else
                {
                    if (_duration > duration)
                    {
                        _duration = duration;
                        _direction = -1;
                    }
                    else
                    {
                        _duration = 0;
                        _direction = 1;
                    }
                }

                if (loops > 0)
                {
                    _loops++;

                    if (_loops >= loops)
                    {
                        AnimationFinished();
                    }
                }
            }
        }
        else
        {
            if (_duration >= duration)
            {
                AnimationFinished();
            }
        }
    }

    public abstract void Update(int deltaTime);
}


[System.Serializable]
public class Move : Tweener
{
    public Vector3 startPosition;
    public Vector3 targetPosition;

    public Move()
    {
        startPosition = Vector3.zero;
        targetPosition = Vector3.zero;
    }

    public override void Update(int deltaTime)
    {
        var tmp = Mathf.Clamp(_duration, 0, duration);
        var curveValue = animationCurve.Evaluate((float)tmp / 1000);

        target.transform.localPosition = startPosition + (targetPosition - startPosition) * curveValue;
    }
}

[System.Serializable]
public class Rotate : Tweener
{
    public Vector3 from;
    public Vector3 to;

    public Rotate()
    {
        from = Vector3.zero;
        to = Vector3.zero;
    }

    public override void Update(int deltaTime)
    {
        var tmp = Mathf.Clamp(_duration, 0, duration);
        var curveValue = animationCurve.Evaluate((float)tmp / 1000);
        target.transform.localRotation = Quaternion.Euler(from + (to - from) * curveValue);
    }
}

[System.Serializable]
public class Scale : Tweener
{
    public Vector3 from;
    public Vector3 to;

    public Scale()
    {
        from = Vector3.zero;
        to = Vector3.zero;
    }

    public override void Update(int deltaTime)
    {
        var tmp = Mathf.Clamp(_duration, 0, duration);
        var curveValue = animationCurve.Evaluate((float)tmp / 1000);
        target.transform.localScale = from + (to - from) * curveValue;
    }
}

[System.Serializable]
public class Fade : Tweener
{
    public float from;
    public float to;

    CanvasGroup _canvas;
    Graphics _graphic;

    public Fade()
    {
        from = 0f;
        to = 0f;
    }

    public override void Update(int deltaTime)
    {
        if (_canvas == null)
        {
            _canvas = target.GetComponent<CanvasGroup>();
        }

        var tmp = Mathf.Clamp(_duration, 0, duration);
        var curveValue = animationCurve.Evaluate((float)tmp / 1000);

        if (_canvas != null)
        {
            _canvas.alpha = from + (to - from) * curveValue;
        }
    }
}