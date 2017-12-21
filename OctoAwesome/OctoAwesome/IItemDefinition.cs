using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für die Definition eînes Items
    /// </summary>
    public interface IItemDefinition : IDefinition, IInventoryableDefinition
    {
        /*/// <summary>
        /// Liefert die Physikalischen Paramerter, wie härte, dichte und bruchzähigkeit
        /// </summary>
        /// <param name="item">Das Item, von dem die Parameter erhalten werden wollen</param>
        /// <returns>Die physikalischen Parameter</returns>
        PhysicalProperties GetProperties(IItem item);*/

        /*/// <summary>
        /// Geplante Methode, mit der das Item auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="item">Der Item-Typ des interagierenden Elements</param>
        /// <param name="itemProperties">Die physikalischen Parameter des interagierenden Elements</param>
        void Hit(IItem item, PhysicalProperties itemProperties);*/
    }
}
