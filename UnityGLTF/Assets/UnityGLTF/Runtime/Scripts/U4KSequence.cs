using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EAnimationEvent
{
    None,
    OnClick,
    OnDrag,
    OnDrop,
    OnMouseEnter,
    OnMouseExit,
    OnSequenceEvent,
    OnContentUpdate,
    OnFrame,
    OnBreakPoint,
    OnPrint,
}

public enum ESequenceTiming
{
    With,
    After
}

public enum EMouseClickType
{
    Left,
    Right,
    Double
}

public enum EDragType
{
    Begin,
    End,
    Dragging
}

public enum EDropType
{
    MouseOver,
    MouseEnter,
    MouseLeave,
    Match,
    NotMatch,
}

public class AnimEventDesc
{
    public EAnimationEvent StartEvent = EAnimationEvent.None;
    public virtual void UpdateSequenceEvent(Queue<Dictionary<string, object>> eventsQueue) { }
}

public class ClickEventDesc : AnimEventDesc
{
    public EMouseClickType clickType
    { get; set; } = EMouseClickType.Left;

    public ClickEventDesc()
    {
        StartEvent = EAnimationEvent.OnClick;
    }
}

public class DragEventDesc : AnimEventDesc
{
    public EDragType condition
    { get; set; } = EDragType.Dragging;

    public DragEventDesc()
    {
        StartEvent = EAnimationEvent.OnDrag;
    }
}

public class DropEventDesc : AnimEventDesc
{
    public EDropType Condition
    { get; set; } = EDropType.MouseOver;

    public DropEventDesc()
    {
        StartEvent = EAnimationEvent.OnDrop;
    }

    public DropEventDesc(EDropType t)
    {
        StartEvent = EAnimationEvent.OnDrop;
        Condition = t;
    }
}

public class MouseEnterEventDesc : AnimEventDesc
{
    public MouseEnterEventDesc()
    {
        StartEvent = EAnimationEvent.OnMouseEnter;
    }
}

public class MouseExitEventDesc : AnimEventDesc
{
    public MouseExitEventDesc()
    {
        StartEvent = EAnimationEvent.OnMouseExit;
    }
}

public class SequenceEventDesc : AnimEventDesc
{
    public string EventName
    { get; set; } = string.Empty;

    public string Param0
    { get; set; } = string.Empty;

    public string Param1
    { get; set; } = string.Empty;

    public SequenceEventDesc()
    {
        StartEvent = EAnimationEvent.OnSequenceEvent;
    }
}

public class ContentUpdateEventDesc : AnimEventDesc
{
    public ContentUpdateEventDesc()
    {
        StartEvent = EAnimationEvent.OnContentUpdate;
    }
}

public class ReachFrameEventDesc : AnimEventDesc
{
    public int Frame
    { get; set; } = -1;
    public ReachFrameEventDesc(int frame)
    {
        StartEvent = EAnimationEvent.OnFrame;
        Frame = frame;
    }
    public ReachFrameEventDesc()
    {
        StartEvent = EAnimationEvent.OnFrame;
    }
}

public class ReachBreakPointEventDesc : AnimEventDesc
{
    public int Line
    { get; set; } = -1;

    public ReachBreakPointEventDesc(int line)
    {
        StartEvent = EAnimationEvent.OnBreakPoint;
        Line = line;
    }

    public ReachBreakPointEventDesc()
    {
        StartEvent = EAnimationEvent.OnBreakPoint;
    }
}

public class OnPrintEventDesc : AnimEventDesc
{
    private string printText;
    public System.Func<Queue<Dictionary<string, object>>, bool> UpdateSequenceEventHook = null;

    public OnPrintEventDesc()
    {
        StartEvent = EAnimationEvent.OnPrint;
        printText = string.Empty;
        UpdateSequenceEventHook = null;
    }

    public OnPrintEventDesc(string printStr)
    {
        StartEvent = EAnimationEvent.OnPrint;
        printText = printStr;
        UpdateSequenceEventHook = null;
    }

    public OnPrintEventDesc(System.Func<Queue<Dictionary<string, object>>, bool> hook)
    {
        StartEvent = EAnimationEvent.OnPrint;
        printText = string.Empty;
        UpdateSequenceEventHook = hook;
    }

    public override void UpdateSequenceEvent(Queue<Dictionary<string, object>> eventsQueue)
    {
        if (UpdateSequenceEventHook == null)
        {
            Dictionary<string, object> printEvent = new Dictionary<string, object>();
            printEvent.Add("Text", printText);
            eventsQueue.Enqueue(printEvent);
        }
        else
            UpdateSequenceEventHook(eventsQueue);
    }
}


public enum EActionType
{
    None,
    RunPython,
    CallPluginAction,
    SendEvent,
    Appear,
    Disappear,
    Flash,
    Rotate,
    Move,
    Wipe,
    TextWriterEffect,
    ChangeColor,
    ChangeBackgroundColor,
    ChangeContent,
    GotoAndPlay,
    GotoAndStop,
}

public class AnimAction
{
    public EActionType Type = EActionType.None;
    public List<string> AffectedControls
    { get; set; }
}

public class RunPythonAction : AnimAction
{
    public string Code
    { get; set; } = string.Empty;

    public RunPythonAction()
    {
        Type = EActionType.RunPython;
    }
}

public class AppearAction : AnimAction
{
    public AppearAction()
    {
        Type = EActionType.Appear;
    }
}

public class DisappearAction : AnimAction
{
    public DisappearAction()
    {
        Type = EActionType.Disappear;
    }
}

public class ChangeColorAction : AnimAction
{
    public Color color
    { get; set; }

    public ChangeColorAction()
    {
        Type = EActionType.ChangeColor;
    }
}

public class ChangebackgroundColorAction : AnimAction
{
    public Color color
    { get; set; }

    public ChangebackgroundColorAction()
    {
        Type = EActionType.ChangeBackgroundColor;
    }
}

public class ChangeContentAction : AnimAction
{
    public string Content
    { get; set; } = string.Empty;

    public ChangeContentAction()
    {
        Type = EActionType.ChangeContent;
    }

    public ChangeContentAction(ChangeFileContentAction action)
    {
        Content = action.ContentPath;
    }
}

public class ChangeFileContentAction : AnimAction
{
    public string ContentPath
    { get; set; } = string.Empty;

    public ChangeFileContentAction()
    {
        Type = EActionType.ChangeContent;
    }
}

public class CallPluginAction : AnimAction
{
    public string Action
    { get; set; } = string.Empty;

    public CallPluginAction()
    {
        Type = EActionType.CallPluginAction;
    }
}

public class SendEventAction : AnimAction
{
    public string EventName
    { get; set; } = string.Empty;

    public string Param0
    { get; set; } = string.Empty;

    public string Param1
    { get; set; } = string.Empty;

    public SendEventAction()
    {
        Type = EActionType.SendEvent;
    }
}

public class TimebasedAction : AnimAction
{
    public System.Action OnCompleteCallback = null;
    public float Duration
    { get; set; } = 1.0f;

    public void OnComplete()
    {
        if (OnCompleteCallback != null)
        {
            OnCompleteCallback();
            OnCompleteCallback = null;
        }
    }

    public virtual void Terminate() { }
}

public class RotateAction : TimebasedAction
{
    public float TotalAngle
    { get; set; } = 0.0f;

    public RotateAction()
    {
        Type = EActionType.Rotate;
    }
}

public class GotoAndPlayAction : AnimAction
{
    public int Frame
    { get; set; } = 0;

    public GotoAndPlayAction(int f)
    {
        Type = EActionType.GotoAndPlay;
        Frame = f;
    }

    public GotoAndPlayAction()
    {
        Type = EActionType.GotoAndPlay;
        Frame = 0;
    }
}

public class GotoAndStopAction : AnimAction
{
    public int Frame
    { get; set; } = 0;

    public GotoAndStopAction(int f)
    {
        Type = EActionType.GotoAndStop;
        Frame = f;
    }

    public GotoAndStopAction()
    {
        Type = EActionType.GotoAndStop;
        Frame = 0;
    }

}

public enum EFlashTarget
{
    Foreground,
    Background
}


public class U4KSequence
{
    public string Name;

    public AnimEventDesc Trigger = new ClickEventDesc();

    public List<AnimAction> Actions = new List<AnimAction>();
}
