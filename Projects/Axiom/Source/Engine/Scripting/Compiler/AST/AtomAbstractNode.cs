#region LGPL License
/*
Axiom Graphics Engine Library
Copyright (C) 2003-2007  Axiom Project Team

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
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id$"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.AST
{
	/// <summary>
	///  This is an abstract node which cannot be broken down further
	/// </summary>
	class AtomAbstractNode : AbstractNode
	{
		#region Fields and Properties

		private bool _parsed = false;
		private bool _isNumber = false;
		private float _number;

		public String value;
		public uint id;

		public bool IsNumber
		{
			get
			{
				if ( !_parsed )
					_parse();
				return _isNumber;
			}
		}

		public float Number
		{
			get
			{
				if ( !_parsed )
					_parse();
				return _number;
			}
		}
		#endregion Fields and Properties

		public AtomAbstractNode( AbstractNode parent )
			: base( parent )
		{
			type = AbstractNodeType.Atom;
		}

		private void _parse()
		{
			_isNumber = float.TryParse( value, out _number);
			_parsed = true;
		}

		#region AbstractNode Implementation

		public override AbstractNode Clone()
		{
			AtomAbstractNode node = new AtomAbstractNode( parent );
			node.file = file;
			node.line = line;
			node.id = id;
			node.type = type;
			node.value = value;
			return node;
		}

		#endregion AbstractNode Implementation
	}
}