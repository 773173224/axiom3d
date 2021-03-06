#region LGPL License

/*
Axiom Graphics Engine Library
Copyright (C) 2003-2010 Axiom Project Team
This file is part of Axiom.RenderSystems.OpenGLES
C# version developed by bostich.

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

#endregion LGPL License

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id$"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.IO;

using Android.Content.Res;

using Axiom.FileSystem;

using Ionic.Zip;

#endregion Namespace Declarations

namespace Axiom.Platform.Android
{
	public class AndroidZipAssetArchive : ZipArchive
	{
		private string _type;
		private readonly AssetManager _assets;
		private MemoryStream ms;

		public AndroidZipAssetArchive( AssetManager assets, string name, string type )
			: base( name, type )
		{
			this._assets = assets;
		}

		#region Archive Implementation

		public override void Load()
		{
			//if ( string.IsNullOrEmpty( _zipFile ) )
			//{
			_zipFile = Name;

			// read the open the zip archive
			var stream = this._assets.Open( Name );
			this.ms = new MemoryStream();
            stream.CopyTo( this.ms );
            stream.Close();

            this.ms.Position = 0;

			// get a input stream from the zip file
			_zipStream = ZipFile.Read( this.ms );
			//}
		}

		#endregion Archive Implementation
	}

	public class AndroidZipAssetArchiveFactory : ArchiveFactory
	{
		private const string _type = "AndroidZipAsset";
		private readonly AssetManager _assets;

		public AndroidZipAssetArchiveFactory( AssetManager assets )
		{
			this._assets = assets;
		}

		#region ArchiveFactory Implementation

		public override string Type
		{
			get { return _type; }
		}

		public override Archive CreateInstance( string name )
		{
			return new AndroidZipAssetArchive( this._assets, name, _type );
		}

		public override void DestroyInstance( ref Archive obj )
		{
			if ( obj is AndroidZipAssetArchive )
			{
				obj.Dispose();
				obj = null;
			}
		}

		#endregion ArchiveFactory Implementation
	}
}
