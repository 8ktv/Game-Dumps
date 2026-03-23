using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
[Obsolete("IBaseUxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
internal interface IBaseUxmlObjectFactory : IBaseUxmlFactory
{
}
