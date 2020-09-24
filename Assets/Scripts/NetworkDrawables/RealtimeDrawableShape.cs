using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Normal.Realtime;

public abstract class RealtimeDrawableShape : MonoBehaviour
{
    public GameObject m_ShapeToClone;
    protected GameObject m_Shape;
    public Transform m_ParentTransform;

    public static float m_SubdivisionScale { get; private set; }

    protected Vector3? drawingStartPosition = null;
    protected Vector3 drawingStartPositionNonNull { get { return drawingStartPosition ?? Vector3.zero; } }
    protected Vector3? drawingCurrentPosition = null;
    protected Vector3 drawingCurrentPositionNonNull { get { return drawingCurrentPosition ?? Vector3.zero; } }
    protected Vector3? drawingEndPosition = null;
    protected Vector3 drawingEndPositionNonNull { get { return drawingEndPosition ?? Vector3.zero; } }

    // Realtime variables
    protected string shapeName;
    protected RealtimeTransform realtimeTransform;

    public RealtimeDrawableShape(string shapeName)
    {
        this.shapeName = shapeName;
    }

    // TODO: there will be 3+ instances of m_SubdivisionScale. how do we get this to 1?
    void Awake()
    {
        UpdateGridScale();
        GlobalGridScale.AddScaleListener(UpdateGridScale);

        AddListener(PostFinish, ListenerType.PostFinish);

        RunOnAwake();
    }

    private void UpdateGridScale()
    {
        m_SubdivisionScale = (float)GlobalGridScale.Instance.GridScale;
    }


    protected virtual void RunOnAwake() { }

    public virtual void StartDrawing(Vector3 startPosition)
    {
        drawingStartPosition = startPosition;
        // m_Shape = UnityEngine.Object.Instantiate(m_ShapeToClone, drawingStartPositionNonNull, Quaternion.identity, m_ParentTransform);
        m_Shape = Realtime.Instantiate(shapeName);
        realtimeTransform = m_Shape.GetComponent<RealtimeTransform>();

        print("Instatiated " + m_Shape.name);
        print(realtimeTransform);
        realtimeTransform.RequestOwnership();

        m_Shape.transform.position = drawingEndPositionNonNull;
        m_Shape.transform.rotation = Quaternion.identity;
        m_Shape.transform.SetParent(m_ParentTransform);
    }
    public virtual void Drawing(Vector3 currentPosition)
    {
        drawingCurrentPosition = currentPosition;
    }
    public virtual void StopDrawing(bool snapToGrid) { }

    public float SnapToGrid(float dimension, float subdivisionScale)
    {
        dimension *= subdivisionScale;
        if (dimension < 1)
        {
            dimension = 1;
        }
        else
        {
            dimension = Mathf.Round(dimension);
        }

        return dimension / subdivisionScale;
    }

    private void PostFinish()
    {
        m_Shape = null;
        drawingStartPosition = null;
        drawingCurrentPosition = null;
    }



    // Events
    private UnityEvent _onPreFinish = new UnityEvent();
    private UnityEvent _onPostFinish = new UnityEvent();
    private UnityEvent _onPreStart = new UnityEvent();
    private UnityEvent _onPostStart = new UnityEvent();
    private UnityEvent _onPreDraw = new UnityEvent();
    private UnityEvent _onPostDraw = new UnityEvent();

    public enum ListenerType
    {
        PreStart, PostStart, PreDraw, PostDraw, PreFinish, PostFinish
    }
    public void AddListener(UnityAction listener, ListenerType listenerType)
    {
        switch (listenerType)
        {
            case ListenerType.PreStart:
                _onPreStart.AddListener(listener);
                break;
            case ListenerType.PostStart:
                _onPostStart.AddListener(listener);
                break;
            case ListenerType.PreDraw:
                _onPreDraw.AddListener(listener);
                break;
            case ListenerType.PostDraw:
                _onPostDraw.AddListener(listener);
                break;
            case ListenerType.PreFinish:
                _onPreFinish.AddListener(listener);
                break;
            case ListenerType.PostFinish:
                _onPostFinish.AddListener(listener);
                break;
        }
    }
    public void RemoveListener(UnityAction listener, ListenerType listenerType)
    {
        switch (listenerType)
        {
            case ListenerType.PreStart:
                _onPreStart.RemoveListener(listener);
                break;
            case ListenerType.PostStart:
                _onPostStart.RemoveListener(listener);
                break;
            case ListenerType.PreDraw:
                _onPreDraw.RemoveListener(listener);
                break;
            case ListenerType.PostDraw:
                _onPostDraw.RemoveListener(listener);
                break;
            case ListenerType.PreFinish:
                _onPreFinish.RemoveListener(listener);
                break;
            case ListenerType.PostFinish:
                _onPostFinish.RemoveListener(listener);
                break;
        }
    }
    public void InvokeListener(ListenerType listenerType)
    {
        switch (listenerType)
        {
            case ListenerType.PreStart:
                _onPreStart.Invoke();
                break;
            case ListenerType.PostStart:
                _onPostStart.Invoke();
                break;
            case ListenerType.PreDraw:
                _onPreDraw.Invoke();
                break;
            case ListenerType.PostDraw:
                _onPostDraw.Invoke();
                break;
            case ListenerType.PreFinish:
                _onPreFinish.Invoke();
                break;
            case ListenerType.PostFinish:
                _onPostFinish.Invoke();
                break;
        }
    }

}