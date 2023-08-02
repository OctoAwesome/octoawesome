using OctoAwesome.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents;
/// <summary>
/// Restricts an entity to be the only one at a given position component
/// </summary>
[SerializationId(1, 17)]
public class UniquePositionComponent : Component, IEntityComponent
{
}
