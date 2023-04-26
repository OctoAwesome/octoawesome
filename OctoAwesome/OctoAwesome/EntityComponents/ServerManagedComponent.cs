using OctoAwesome.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents;
[Nooson]
public partial class ServerManagedComponent : Component, IEntityComponent
{
    [NoosonIgnore]
    public bool OnServer { get; set; } = false;
}
