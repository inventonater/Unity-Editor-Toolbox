using System.Linq;
using Toolbox;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Provides examples and usage scenarios for the component enumerable capabilities.
    /// </summary>
    public class ComponentEnumerableExamples : MonoBehaviour
    {
        private void Start()
        {
            // Example 1: Get all ancestor renderers of the current component
            var ancestorRenderers = transform.GetAncestors<Renderer>();
            Debug.Log("Ancestor Renderers:");
            foreach (var renderer in ancestorRenderers)
            {
                Debug.Log(renderer.name);
            }

            // Example 2: Get descendant colliders of the current component, filtering by enabled state and limiting depth
            var descendantOptions = new ComponentEnumerableOptions { MaxDepth = 3 };
            DescendantEnumerable<Collider> descendantColliders = transform.GetDescendants<Collider>().WithOptions(descendantOptions);
            foreach (var descendantCollider in descendantColliders)
            {
                if (descendantCollider.enabled)
                {
                    Debug.Log(descendantCollider.name);
                }
            }


            Debug.Log("Descendant Colliders (Enabled, Depth <= 3):");
            foreach (var collider in descendantColliders)
            {
                Debug.Log(collider.name);
            }

            // Example 3: Get ancestor rigidbodies, including the current component and inactive ones
            var ancestorOptions = new ComponentEnumerableOptions{ IncludeSelf = true, IncludeInactiveComponents = true};
            var ancestorRigidbodies = transform.GetAncestors<Rigidbody>(ancestorOptions);
            Debug.Log("Ancestor Rigidbodies (Including Self and Inactive):");
            foreach (var rigidbody in ancestorRigidbodies)
            {
                Debug.Log(rigidbody.name);
            }

            // Example 4: Get descendant meshes using depth-first search traversal
            var descendantMeshes = transform.GetDescendants<MeshFilter>(DescendantTraversalType.DepthFirst);
            Debug.Log("Descendant Meshes (Breadth-First Search):");
            foreach (var meshFilter in descendantMeshes)
            {
                Debug.Log(meshFilter.name);
            }

            // Example 5: Get the first ancestor component of type AudioSource
            var ancestorAudioSource = transform.GetAncestors<AudioSource>().FirstOrDefault();
            if (ancestorAudioSource != null)
            {
                Debug.Log("First Ancestor AudioSource: " + ancestorAudioSource.name);
            }
            else
            {
                Debug.Log("No ancestor AudioSource found.");
            }

            // Example 6: Get the count of descendant components of type Light
            var descendantLightCount = transform.GetDescendants<Light>().Count();
            Debug.Log("Descendant Light Count: " + descendantLightCount);

            // Example 7: Check if any ancestor component of type ParticleSystem exists
            var hasAncestorParticleSystem = transform.GetAncestors<ParticleSystem>().Any();
            Debug.Log("Has Ancestor ParticleSystem: " + hasAncestorParticleSystem);

            // Example 8: Get the closest ancestor component of type Camera
            var closestAncestorCamera = transform.GetAncestors<Camera>().FirstOrDefault();
            if (closestAncestorCamera != null)
            {
                Debug.Log("Closest Ancestor Camera (Depth <= 5): " + closestAncestorCamera.name);
            }
            else
            {
                Debug.Log("No ancestor Camera found within depth 5.");
            }

            // Example 9: Get all descendant components of type Renderer and sort them by name
            var sortedDescendantRenderers = transform.GetDescendants<Renderer>().OrderBy(renderer => renderer.name);
            Debug.Log("Sorted Descendant Renderers:");
            foreach (var renderer in sortedDescendantRenderers)
            {
                Debug.Log(renderer.name);
            }

            // Example 10: Get the total bounding box of all descendant MeshRenderer components
            var descendantMeshRenderers = transform.GetDescendants<MeshRenderer>();
            if (descendantMeshRenderers.Any())
            {
                var totalBounds = descendantMeshRenderers.First().bounds;
                foreach (var meshRenderer in descendantMeshRenderers.Skip(1))
                {
                    totalBounds.Encapsulate(meshRenderer.bounds);
                }
                Debug.Log("Total Bounding Box of Descendant MeshRenderers: " + totalBounds);
            }
            else
            {
                Debug.Log("No descendant MeshRenderer found.");
            }
        }
    }
}