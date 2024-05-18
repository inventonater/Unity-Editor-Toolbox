using R3;
using UnityEngine;

namespace Toolbox.ReactiveMechanics
{
    public class EventHandler : MonoBehaviour
    {
        private ClickMechanic[] clickMechanics;
        private ReactiveMechanic<DoubleClickMechanic.Event, MouseInputFrame>[] doubleClickMechanics;
        private ReactiveMechanic<LongPressMechanic.Event, MouseInputFrame>[] longPressMechanics;
        private ReactiveMechanic<DragMechanic.Event, MouseInputFrame>[] dragMechanics;

        private void Start()
        {
            clickMechanics = GetComponents<ClickMechanic>();
            doubleClickMechanics = GetComponents<ReactiveMechanic<DoubleClickMechanic.Event, MouseInputFrame>>();
            longPressMechanics = GetComponents<ReactiveMechanic<LongPressMechanic.Event, MouseInputFrame>>();
            dragMechanics = GetComponents<ReactiveMechanic<DragMechanic.Event, MouseInputFrame>>();

            foreach (var mechanic in clickMechanics)
            {
                mechanic.Observable.Subscribe(HandleClickEvent);
            }

            foreach (var mechanic in doubleClickMechanics)
            {
                mechanic.Observable.Subscribe(HandleDoubleClickEvent);
            }

            foreach (var mechanic in longPressMechanics)
            {
                mechanic.Observable.Subscribe(HandleLongPressEvent);
            }

            foreach (var mechanic in dragMechanics)
            {
                mechanic.Observable.Subscribe(HandleDragEvent);
            }
        }

        private void HandleClickEvent(ClickMechanic.Event clickEvent)
        {
            Debug.Log("Click event triggered at position: " + clickEvent.ClickPosition);
        }

        private void HandleDoubleClickEvent(DoubleClickMechanic.Event doubleClickEvent)
        {
            Debug.Log("Double-click event triggered at position: " + doubleClickEvent.DoubleClickPosition);
        }

        private void HandleLongPressEvent(LongPressMechanic.Event longPressEvent)
        {
            Debug.Log("Long press event triggered at position: " + longPressEvent.LongPressPosition);
        }

        private void HandleDragEvent(DragMechanic.Event dragEvent)
        {
            Debug.Log($"Drag event triggered from {dragEvent.DragEndPosition} to {dragEvent.DragEndPosition}");
        }
    }
}