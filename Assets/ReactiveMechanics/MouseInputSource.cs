using R3;
using UnityEngine;

public struct MouseInputFrame : IInputFrame
{
    public MouseInputFrame(bool isPressed, Vector2 position) : this()
    {
        IsPressed = isPressed;
        Position = position;
    }

    public bool IsPressed { get;  }
    public Vector2 Position { get; }
    public Vector3 MouseSpecificDetails { get; }
}

public class MouseInputSource : MonoBehaviour, IInputSource<MouseInputFrame>
{
    private Subject<MouseInputFrame> _inputSubject = new();

    public Observable<MouseInputFrame> InputObservable => _inputSubject.AsObservable();

    private void Update()
    {
        _inputSubject.OnNext(new MouseInputFrame(isPressed: Input.GetMouseButton(0), position: Input.mousePosition));
    }

    private void OnDestroy() => _inputSubject.Dispose();
}