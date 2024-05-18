using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;
using R3;

/*

 Show me how to use Cysharp/R3 to implement a Recoverable Action Tree pattern for a user interface component. It should support a hierarchical structure of actions, where each action is recoverable and can be undone if certain criteria are met. Actions are evaluated continuously over many iterations of the Update loop while the user performs their gestures. Actions should be triggered by these continuous user events and processed recursively based on their associated criteria. The pattern should allow for flexible layering and branching of actions. Provide a code example in C# that demonstrates the implementation of the Recoverable Action Tree pattern.
As an example application of the pattern, create a system which allows the user to "pluck" an entity out of a list, drag it around in 3d space, place it on a destination 2d surface, manipulate it within the plane of that surface, and then perform a quick fling down gesture to commit the placement. This sequence should be defined step by step by the Recoverable Action Tree. If any intermediate step fails - the whole stack should reverse and clean up to the original state. It should be possible for designers to inject alternative branches to the Recoverable Action Tree.
We should also be able to provide View Observers with continuous updates for rendering specific visual feedback which is appropriate for each node in the Tree. Visual feedback might require additional meta details such as DragMagnitude, DragVelocity, and so forth.
Show me the implementation of this system in detail.

 */


namespace Runtime.MechanicFluid4
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;


    public class Entity
    {
        public Transform transform { get; }
    }

    public class EntityList
    {
        public async Task<Entity> OnEntitySelected()
        {
            throw new NotImplementedException();
        }

        public void ReturnEntity(Entity selectedEntity)
        {
            throw new NotImplementedException();
        }
    }

    public class EntityManager
    {
        public static void CommitPlacement()
        {
            throw new NotImplementedException();
        }

        public static void UndoPlacement()
        {
            throw new NotImplementedException();
        }

        public static Entity GetDraggedEntity()
        {
            throw new NotImplementedException();
        }

        public static Entity GetPlacedEntity()
        {
            throw new NotImplementedException();
        }
    }

    public class ViewManager
    {
        public static void UpdateDragFeedback(float dragMagnitude, Vector3 dragVelocity)
        {
            throw new NotImplementedException();
        }
    }


    public class RecoverableActionTree : MonoBehaviour
    {
        public EntityList entityList;
        public Transform destinationSurface;

        private ActionNode rootNode;

        private void Start()
        {
            rootNode = new SequenceNode(
                new PluckEntityNode(entityList),
                new DragEntityNode(),
                new PlaceEntityNode(destinationSurface),
                new ManipulateEntityNode(),
                new CommitPlacementNode()
            );

            rootNode.Execute().Forget();
        }
    }

    public abstract class ActionNode
    {
        public abstract UniTask Execute();
        public abstract UniTask Undo();
    }

    public class SequenceNode : ActionNode
    {
        private readonly ActionNode[] children;

        public SequenceNode(params ActionNode[] children)
        {
            this.children = children;
        }

        public override async UniTask Execute()
        {
            foreach (var child in children)
            {
                await child.Execute();
            }
        }

        public override async UniTask Undo()
        {
            for (int i = children.Length - 1; i >= 0; i--)
            {
                await children[i].Undo();
            }
        }
    }

    public class PluckEntityNode : ActionNode
    {
        private readonly EntityList entityList;
        private Entity selectedEntity;

        public PluckEntityNode(EntityList entityList)
        {
            this.entityList = entityList;
        }

        public override async UniTask Execute()
        {
            // Wait for user to select an entity from the list
            selectedEntity = await entityList.OnEntitySelected();
        }

        public override UniTask Undo()
        {
            // Return the entity to its original state in the list
            entityList.ReturnEntity(selectedEntity);
            return UniTask.CompletedTask;
        }
    }

    public class DragEntityNode : ActionNode
    {
        private Entity draggedEntity;
        private Vector3 initialPosition;

        public override async UniTask Execute()
        {
            draggedEntity = EntityManager.GetDraggedEntity();
            initialPosition = draggedEntity.transform.position;

            // Continuously update the entity's position based on user input
            await Observable.EveryUpdate()
                .TakeWhile(_ => Input.GetMouseButton(0))
                .ForEachAsync(_ =>
                {
                    Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    draggedEntity.transform.position = newPosition;

                    // Provide visual feedback based on drag magnitude and velocity
                    float dragMagnitude = Vector3.Distance(initialPosition, newPosition);
                    Vector3 dragVelocity = (newPosition - draggedEntity.transform.position) / Time.deltaTime;
                    ViewManager.UpdateDragFeedback(dragMagnitude, dragVelocity);
                });
        }

        public override UniTask Undo()
        {
            // Return the entity to its initial position
            draggedEntity.transform.position = initialPosition;
            return UniTask.CompletedTask;
        }
    }

    public class PlaceEntityNode : ActionNode
    {
        private readonly Transform destinationSurface;
        private Entity placedEntity;

        public PlaceEntityNode(Transform destinationSurface)
        {
            this.destinationSurface = destinationSurface;
        }

        public override async UniTask Execute()
        {
            placedEntity = EntityManager.GetDraggedEntity();

            // Wait for the user to place the entity on the destination surface
            await UniTask.WaitUntil(() => IsEntityOnSurface(placedEntity, destinationSurface));

            // Snap the entity to the surface
            placedEntity.transform.SetParent(destinationSurface, true);
        }

        public override UniTask Undo()
        {
            // Remove the entity from the destination surface
            placedEntity.transform.SetParent(null);
            return UniTask.CompletedTask;
        }

        private bool IsEntityOnSurface(Entity entity, Transform surface)
        {
            // Check if the entity is within the bounds of the surface
            // Implement your own logic here based on your specific requirements
            return true;
        }
    }

    public class ManipulateEntityNode : ActionNode
    {
        private Entity manipulatedEntity;
        private Quaternion initialRotation;
        private Vector3 initialScale;

        public override async UniTask Execute()
        {
            manipulatedEntity = EntityManager.GetPlacedEntity();
            initialRotation = manipulatedEntity.transform.rotation;
            initialScale = manipulatedEntity.transform.localScale;

            // Continuously update the entity's rotation and scale based on user input
            await UniTaskAsyncEnumerable.EveryUpdate()
                .TakeWhile(_ => Input.GetMouseButton(0))
                .ForEachAsync(_ =>
                {
                    // Implement your own manipulation logic here based on user input
                    // Example: Rotate the entity based on mouse movement
                    float rotationSpeed = 100f;
                    float rotationAmount = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                    manipulatedEntity.transform.Rotate(Vector3.up, rotationAmount);

                    // Example: Scale the entity based on mouse scroll wheel
                    float scaleSpeed = 0.1f;
                    float scaleAmount = Input.GetAxis("Mouse ScrollWheel") * scaleSpeed;
                    manipulatedEntity.transform.localScale += Vector3.one * scaleAmount;
                });
        }

        public override UniTask Undo()
        {
            // Reset the entity's rotation and scale to initial values
            manipulatedEntity.transform.rotation = initialRotation;
            manipulatedEntity.transform.localScale = initialScale;
            return UniTask.CompletedTask;
        }
    }

    public class CommitPlacementNode : ActionNode
    {
        public override async UniTask Execute()
        {
            // Wait for the user to perform a quick fling down gesture
            await UniTask.WaitUntil(() => IsFlingDownGesture());

            // Commit the placement
            EntityManager.CommitPlacement();
        }

        public override UniTask Undo()
        {
            // Undo the placement commitment
            EntityManager.UndoPlacement();
            return UniTask.CompletedTask;
        }

        private bool IsFlingDownGesture()
        {
            // Implement your own logic to detect a quick fling down gesture
            // Example: Check if the mouse movement speed exceeds a threshold in the downward direction
            return Input.GetMouseButtonUp(0) && Input.GetAxis("Mouse Y") < -5f;
        }
    }
}
