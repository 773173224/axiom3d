#region LGPL License
/*
Axiom Graphics Engine Library
Copyright (C) 2003-2006 Axiom Project Team

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
//     <license see="http://axiomengine.sf.net/wiki/index.php/license.txt"/>
//     <id value="$Id$"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations

using System;

using Axiom.Core;
using Axiom.ParticleSystems;
using Axiom.Math;
using Axiom.Scripting;

#endregion Namespace Declarations

namespace Axiom.ParticleFX
{
    /// <summary>
    /// Summary description for BoxEmitter.
    /// </summary>
    public class BoxEmitter : AreaEmitter
    {
		public BoxEmitter( ParticleSystem ps )
            : base(ps)
        {
            InitDefaults( "Box" );
        }

        public override void InitParticle( Particle particle )
        {
            Vector3 xOff, yOff, zOff;

            xOff = Utility.SymmetricRandom() * xRange;
            yOff = Utility.SymmetricRandom() * yRange;
            zOff = Utility.SymmetricRandom() * zRange;

            particle.Position = position + xOff + yOff + zOff;

            // Generate complex data by reference
            GenerateEmissionColor( ref particle.Color );
            GenerateEmissionDirection( ref particle.Direction );
            GenerateEmissionVelocity( ref particle.Direction );

            // Generate simpler data
            particle.timeToLive = particle.totalTimeToLive = GenerateEmissionTTL();
        }

        #region Command definition classes

        /// <summary>
        ///    
        /// </summary>
        [Command( "width", "Width of the box emitter.", typeof( ParticleEmitter ) )]
		class WidthCommand : IPropertyCommand
        {
            public void Set( object target, string val )
            {
                BoxEmitter emitter = target as BoxEmitter;
                emitter.Width = StringConverter.ParseFloat( val );
            }
            public string Get( object target )
            {
                BoxEmitter emitter = target as BoxEmitter;
                return StringConverter.ToString( emitter.Width );
            }
        }

        /// <summary>
        ///    
        /// </summary>
        [Command( "height", "Height of the box emitter.", typeof( ParticleEmitter ) )]
		class HeightCommand : IPropertyCommand
        {
            public void Set( object target, string val )
            {
                BoxEmitter emitter = target as BoxEmitter;
                emitter.Height = StringConverter.ParseFloat( val );
            }
            public string Get( object target )
            {
                BoxEmitter emitter = target as BoxEmitter;
                return StringConverter.ToString( emitter.Height );
            }
        }

        /// <summary>
        ///    
        /// </summary>
        [Command( "depth", "Depth of the box emitter.", typeof( ParticleEmitter ) )]
		class DepthCommand : IPropertyCommand
        {
            public void Set( object target, string val )
            {
                BoxEmitter emitter = target as BoxEmitter;
                emitter.Depth = StringConverter.ParseFloat( val );
            }
            public string Get( object target )
            {
                BoxEmitter emitter = target as BoxEmitter;
                return StringConverter.ToString( emitter.Depth );
            }
        }

        #endregion Command definition classes
    }
}