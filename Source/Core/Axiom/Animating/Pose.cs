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
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id$"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Axiom.Collections;
using Axiom.Graphics;
using Axiom.Core;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Animating
{
	/// <summary>
	/// A pose is a linked set of vertex offsets applying to one set of vertex data. 
	/// </summary>
	/// <remarks>
	///		The target index referred to by the pose has a meaning set by the user
	///		of this class; but for example when used by Mesh it refers to either the
	///		Mesh shared geometry (0) or a SubMesh dedicated geometry (1+).
	///		Pose instances can be referred to by keyframes in VertexAnimationTrack in
	///		order to animate based on blending poses together.
	/// </remarks>
	public class Pose
	{
		#region Protected Members

		/// <summary>Target geometry index</summary>
		private readonly ushort target;

		/// Optional name
		private readonly string name;

		/// <summary>Primary storage, sparse vertex use</summary>
		private readonly Dictionary<int, Vector3> vertexOffsetMap = new Dictionary<int, Vector3>();

		/// <summary>Derived hardware buffer, covers all vertices</summary>
		private HardwareVertexBuffer vertexBuffer;

		#endregion Protected Members

		#region Constructors

		/// <summary>Constructor</summary>
		///	<param name="target">The target vertexdata index (0 for shared, 1+ for 
		///		dedicated at the submesh index + 1</param>
		///	<param name="name"></param>
		public Pose( ushort target, string name )
		{
			this.target = target;
			this.name = name;
		}

		#endregion Constructors

		#region Properties

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ushort Target
		{
			get
			{
				return this.target;
			}
		}

		public Dictionary<int, Vector3> VertexOffsetMap
		{
			get
			{
				return this.vertexOffsetMap;
			}
		}

		public HardwareVertexBuffer VertexBuffer
		{
			get
			{
				return this.vertexBuffer;
			}
		}

		#endregion Properties

		#region Public Methods

		/// <summary>Adds an offset to a vertex for this pose.</summary>
		/// <param name="index"> The vertex index</param>
		/// <param name="offset"> The position offset for this pose</param>
		public void AddVertex( int index, Vector3 offset )
		{
			this.vertexOffsetMap[ index ] = offset;
			DisposeVertexBuffer();
		}

		/// <summary>Remove a vertex offset.</summary>
		public void RemoveVertex( int index )
		{
			if ( this.vertexOffsetMap.ContainsKey( index ) )
			{
				this.vertexOffsetMap.Remove( index );
			}
			DisposeVertexBuffer();
		}

		/// <summary>Clear all vertex offsets.</summary>
		public void ClearVertexOffsets()
		{
			this.vertexOffsetMap.Clear();
			DisposeVertexBuffer();
		}

		protected void DisposeVertexBuffer()
		{
			if ( this.vertexBuffer != null )
			{
				this.vertexBuffer.Dispose();
				this.vertexBuffer = null;
			}
		}

		/// <summary>Get a hardware vertex buffer version of the vertex offsets.</summary>
		public HardwareVertexBuffer GetHardwareVertexBuffer( int numVertices )
		{
			if ( this.vertexBuffer == null )
			{
				// Create buffer
				var decl = HardwareBufferManager.Instance.CreateVertexDeclaration();
				decl.AddElement( 0, 0, VertexElementType.Float3, VertexElementSemantic.Position );

				this.vertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer( decl, numVertices,
				                                                                       BufferUsage.StaticWriteOnly,
				                                                                       false );

				// lock the vertex buffer
				var ipBuf = this.vertexBuffer.Lock( BufferLocking.Discard );

#if !AXIOM_SAFE_ONLY
				unsafe
#endif
				{
					var buffer = ipBuf.ToFloatPointer();
					for ( var i = 0; i < numVertices*3; i++ )
					{
						buffer[ i ] = 0f;
					}

					// Set each vertex
					foreach ( var pair in this.vertexOffsetMap )
					{
						var offset = 3*pair.Key;
						var v = pair.Value;
						buffer[ offset++ ] = v.x;
						buffer[ offset++ ] = v.y;
						buffer[ offset ] = v.z;
					}
					this.vertexBuffer.Unlock();
				}
			}
			return this.vertexBuffer;
		}

		#endregion Public Methods
	}
}