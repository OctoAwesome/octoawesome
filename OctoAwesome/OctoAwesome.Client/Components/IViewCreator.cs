using engenious;

using OctoAwesome.EntityComponents;

namespace OctoAwesome.Client.Components;
public interface IViewCreator
{
    public bool IsFirstPerson { get; }

    public (Vector3 cameraPosition, Vector3 lookAt, Vector3 cameraUpVector) CreateCameraUpVectorAndView(HeadComponent head, Coordinate playerPos);
}