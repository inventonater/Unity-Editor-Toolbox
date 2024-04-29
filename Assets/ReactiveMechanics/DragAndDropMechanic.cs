using R3;
using UnityEngine;

public class DragAndDropMechanic : ReactiveMechanic<DragAndDropMechanic.Event, IInputFrame>
{
    public readonly struct Event : IMechanicEvent
    {
        public Event(DragAndDropMechanic mechanic, Vector2 dragStartPosition, Vector2 dragEndPosition, GameObject droppedObject)
        {
            Mechanic = mechanic;
            DragStartPosition = dragStartPosition;
            DragEndPosition = dragEndPosition;
            DroppedObject = droppedObject;
        }

        public IMechanic Mechanic { get; }
        public Vector2 DragStartPosition { get; }
        public Vector2 DragEndPosition { get; }
        public GameObject DroppedObject { get; }
    }

    private DragMechanic dragMechanic;

    protected override void Awake()
    {
        base.Awake();
        dragMechanic = GetComponent<DragMechanic>();
    }

    protected override void SetupMechanic(IInputSource<IInputFrame> inputSource)
    {
        dragMechanic.Observable
            .WithLatestFrom(inputSource.InputObservable, (dragEvent, inputFrame) => new { DragEvent = dragEvent, InputFrame = inputFrame })
            .Subscribe(data =>
            {
                GameObject droppedObject = GetDroppedObject(data.DragEvent.DragEndPosition);
                if (droppedObject != null)
                {
                    FireEvent(new Event(this, data.DragEvent.DragStartPosition, data.DragEvent.DragEndPosition, droppedObject));
                }
            });
    }

    private GameObject GetDroppedObject(Vector2 position)
    {
        // Implement logic to determine the dropped object based on the drag end position
        // This could involve raycasting, collider checks, or any other game-specific logic
        // Return the dropped GameObject if a valid drop target is found, otherwise return null
        // Example:
        // RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        // return hit.collider != null ? hit.collider.gameObject : null;
        return null;
    }
}