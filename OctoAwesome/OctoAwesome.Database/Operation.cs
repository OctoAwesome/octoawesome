using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database
{
    [Flags]
    public enum Operation
    {
        /// <summary>
        /// Indicates that this lock is for this read process
        /// </summary>
        Read = 1 << 0,
        /// <summary>
        /// Indicates that this lock is for this write process
        /// </summary>
        Write = 1 << 1,
        /// <summary>
        /// Indicates that lock is exclusive for this operation
        /// </summary>
        Exclusive = 1 << 2 
    }
}
