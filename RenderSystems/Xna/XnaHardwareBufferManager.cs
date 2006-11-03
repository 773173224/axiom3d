#region LGPL License
/*
Axiom Game Engine Library
Copyright (C) 2003  Axiom Project Team

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

#region Namespace Declarations

using System;

using Axiom;
using Axiom.Graphics;
using VertexDeclaration = Axiom.Graphics.VertexDeclaration;

using DX = Microsoft.Xna.Framework;
using D3D = Microsoft.Xna.Framework;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.XNA
{
    /// <summary>
    /// 	Summary description for D3DHardwareBufferManager.
    /// </summary>
    public class XNAHardwareBufferManager : HardwareBufferManager
    {
        #region Member variables

        protected D3D.Graphics.GraphicsDevice device;

        #endregion

        #region Constructors

        /// <summary>
        ///		
        /// </summary>
        /// <param name="device"></param>
        public XNAHardwareBufferManager( D3D.Graphics.GraphicsDevice device )
        {
            this.device = device;
        }

        #endregion

        #region Methods

        public override HardwareIndexBuffer CreateIndexBuffer( IndexType type, int numIndices, BufferUsage usage )
        {
            // call overloaded method with no shadow buffer
            return CreateIndexBuffer( type, numIndices, usage, false );
        }

        public override HardwareIndexBuffer CreateIndexBuffer( IndexType type, int numIndices, BufferUsage usage, bool useShadowBuffer )
        {
            XNAHardwareIndexBuffer buffer = new XNAHardwareIndexBuffer( type, numIndices, usage, device, false, useShadowBuffer );
            indexBuffers.Add( buffer );
            return buffer;
        }

        public override HardwareVertexBuffer CreateVertexBuffer( int vertexSize, int numVerts, BufferUsage usage )
        {
            // call overloaded method with no shadow buffer
            return CreateVertexBuffer( vertexSize, numVerts, usage, false );
        }

        public override HardwareVertexBuffer CreateVertexBuffer( int vertexSize, int numVerts, BufferUsage usage, bool useShadowBuffer )
        {
            XNAHardwareVertexBuffer buffer = new XNAHardwareVertexBuffer( vertexSize, numVerts, usage, device, false, useShadowBuffer );
            vertexBuffers.Add( buffer );
            return buffer;
        }

        public override VertexDeclaration CreateVertexDeclaration()
        {
            VertexDeclaration decl = new XNAVertexDeclaration( device );
            vertexDeclarations.Add( decl );
            return decl;
        }

        // TODO: Disposal

        #endregion

        #region Properties

        #endregion

    }
}
