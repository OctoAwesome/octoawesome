using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using engenious.Graphics;
using engenious;
namespace OctoAwesome.Entities
{
    public interface IGraphicsDevice
    {
        // GraphicsDevice GraphicsDevice { get; } // ich würde das lieber wegkappseln...
        // Nur gekappselte Methoden, weil dadurch instancing ermöglicht wird... 
        // möglicherweise auf vertex und indexbuffer verzichten und nur über geometrie + texture daten
        // wenn aus dem model die vertecies rausgezogen werden kann auch für model LOD behandelt werden.
        void Draw(int rotx, int roty, int rotz, VertexBuffer vb, IndexBuffer ib, PrimitiveType primitiveType, int verteciesCount, int primitiveCont);
        void Draw(int rotz, VertexBuffer vb, IndexBuffer ib, PrimitiveType primitiveType, int verteciesCount, int primitiveCont);
        void Draw(int rotx, int roty, int rotz);
        void Draw(int rotz);
        Texture2D LoadTexture(string name);
        Model LoadModel(string name);
    }
    public interface IDrawable
    {
        #region Temporary
        string Name { get; }
        string ModelName { get; }
        string TextureName { get; }
        float BaseRotationZ { get; }
        #endregion

        float Azimuth { get; }
        Vector3 Body { get; }
        Coordinate Position { get; }
        bool NeedUpdate { get; }
        void Initialize(IGraphicsDevice device);
        void Draw(IGraphicsDevice graphicsDevice, GameTime gameTime);
    }

    public interface IControllable
    {
        Coordinate Position { get; }
        IController Controller { get; }
        void Register(IController controller);
        void Reset();
    }

    public interface IController
    {
        float HeadTilt { get; }
        float HeadYaw { get; }
        Vector2 MoveValue { get; }
        Vector2 HeadValue { get; }
        Index3? InteractBlock { get; }
        Index3? ApplyBlock { get; }
        OrientationFlags? ApplySide { get; }
        InputTrigger<bool> JumpInput { get; }
        InputTrigger<bool> ApplyInput { get; }
        InputTrigger<bool> InteractInput { get; }
    }

    public interface IInteractable
    {
    }

    public class InputTrigger<T>
    {
        public int Setter { get; private set; }
        public T Value { get; private set; }
        public void Set(T value)
        {
            Setter++;
            Value = value;
        }
        public void Validate()
        {
            Setter = 0;
            Value = default;
        }
    }



}
