using engenious;

using OctoAwesome.EntityComponents;
using OctoAwesome.Location;

namespace OctoAwesome.Client.Components;

/// <summary>
/// Interface for creating a camera view.
/// </summary>
public interface IViewCreator
{
    /// <summary>
    /// Gets a value indicating whether this creator creates a first person camera view.
    /// </summary>
    public bool IsFirstPerson { get; }

    /// <summary>
    /// Creates camera view for a entity with a head.
    /// </summary>
    /// <param name="head">The head component of the entity.</param>
    /// <param name="playerPos">The position of the entity.</param>
    /// <returns>Tuple of camera position, camera look at point and camera up vector.</returns>
    public (Vector3 cameraPosition, Vector3 lookAt, Vector3 cameraUpVector) CreateCameraUpVectorAndView(HeadComponent head, Coordinate playerPos);
}