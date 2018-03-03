using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Common
{
    public interface IDefinitionResolverHelper
    {
        IParsedDefinition GetDefinitionBody(string definitionname);
    }
}
