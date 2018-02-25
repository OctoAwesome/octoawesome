using engenious.Graphics;

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
}
