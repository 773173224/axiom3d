#region LGPL License

/*
Axiom Graphics Engine Library
Copyright � 2003-2011 Axiom Project Team

The overall design, and a majority of the core engine and rendering code 
contained within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.  
Many thanks to the OGRE team for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/

#endregion

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: HLSLIncludeHandler.cs 2856 2011-09-26 00:25:49Z ncardosoyd $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.IO;
using Axiom.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.Xna.HLSL
{
#if !XNA4
	class HLSLIncludeHandler : CompilerIncludeHandler
	{      
		public override Stream Open( CompilerIncludeHandlerType includeType, string filename )
		{
			return ResourceGroupManager.Instance.OpenResource( filename );
		}
	}
#endif
}