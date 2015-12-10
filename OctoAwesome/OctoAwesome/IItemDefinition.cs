using System.Drawing;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für die Definition eînes Items
    /// </summary>
    public interface IItemDefinition
    {
        /// <summary>
        /// Der Name des Items
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Bild, das das Item repräsentiert
        /// </summary>
        Bitmap Icon { get; }

        /// <summary>
        /// Gibt an, wie viele dieses Items im Inventar in einem Slot gestapelt werden können
        /// </summary>
        int StackLimit { get; }

        /// <summary>
        /// Liefert die Physikalischen Paramerter, wie härte, dichte und bruchzähigkeit
        /// </summary>
        /// <param name="item">Das Item, von dem die Parameter erhalten werden wollen</param>
        /// <returns>Die physikalischen Parameter</returns>
        //PhysicalProperties GetProperties(IItem item);

        /// <summary>
        /// Geplante Methode, mit der das Item auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="item">Der Item-Typ des interagierenden Elements</param>
        /// <param name="itemProperties">Die physikalischen Parameter des interagierenden Elements</param>
        //void Hit(IItem item, PhysicalProperties itemProperties);
    }
}
