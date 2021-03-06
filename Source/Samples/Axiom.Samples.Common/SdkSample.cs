#region MIT/X11 License

//Copyright � 2003-2012 Axiom 3D Rendering Engine Project
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

#endregion License

using System;
using System.Collections.Generic;
using Axiom.Collections;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using SIS = SharpInputSystem;

namespace Axiom.Samples
{
	/// <summary>
	/// Base SDK sample class. Includes default player camera and SDK trays.
	/// </summary>
	public class SdkSample : Sample
	{
		#region Fields and Properties

		private bool dragLook;

		/// <summary>
		/// click and drag to free-look
		/// </summary>
		protected virtual bool DragLook
		{
			get
			{
				return this.dragLook;
			}

			[OgreVersion( 1, 7, 2 )]
			set
			{
				if ( value )
				{
					this.CameraManager.setStyle( CameraStyle.Manual );
					this.TrayManager.ShowCursor();
					this.dragLook = true;
				}
				else
				{
					this.CameraManager.setStyle( CameraStyle.FreeLook );
					this.TrayManager.HideCursor();
					this.dragLook = false;
				}
			}
		}

		/// <summary>
		/// Main Viewport
		/// </summary>
		protected Viewport Viewport;

		/// <summary>
		/// Main Camera
		/// </summary>
		protected Camera Camera;

		/// <summary>
		/// Tray Interface Manager
		/// </summary>
		protected SdkTrayManager TrayManager;

		/// <summary>
		/// Basic Camera Manager
		/// </summary>
		protected SdkCameraManager CameraManager;

		/// <summary>
		/// Simple Details Panel
		/// </summary>
		protected ParamsPanel DetailsPanel;

		/// <summary>
		/// Was the cursor visible before dialog appeared
		/// </summary>
		protected bool CursorWasVisible;

		#endregion Fields and Properties

		#region Construction and Destruction

		public SdkSample()
		{
			// so we don't have to worry about checking if these keys exist later
			Metadata[ "Title" ] = "Untitled";
			Metadata[ "Description" ] = String.Empty;
			Metadata[ "Category" ] = "Unsorted";
			Metadata[ "Thumbnail" ] = String.Empty;
			Metadata[ "Help" ] = String.Empty;

			this.TrayManager = null;
			this.CameraManager = null;
		}

		~SdkSample()
		{
		}

		#endregion Construction and Destruction

		#region Sample Implementation

		/// <summary>
		/// Manually update the cursor position after being unpaused.
		/// </summary>
		[OgreVersion( 1, 7, 2 )]
		public override void Unpaused()
		{
			this.TrayManager.RefreshCursor();
		}

		/// <summary>
		/// Automatically saves position and orientation for free-look cameras.
		/// </summary>
		[OgreVersion( 1, 7, 2 )]
		public override void SaveState( NameValuePairList state )
		{
			if ( this.CameraManager.getStyle() == CameraStyle.FreeLook )
			{
				state[ "CameraPosition" ] = this.Camera.Position.ToString();
				state[ "CameraOrientation" ] = this.Camera.Orientation.ToString();
			}
		}

		/// <summary>
		/// Automatically restores position and orientation for free-look cameras.
		/// </summary>
		[OgreVersion( 1, 7, 2 )]
		public override void RestoreState( NameValuePairList state )
		{
			if ( state.ContainsKey( "CameraPosition" ) && state.ContainsKey( "CameraOrientation" ) )
			{
				this.CameraManager.setStyle( CameraStyle.FreeLook );
				this.Camera.Position = StringConverter.ParseVector3( state[ "CameraPosition" ] );
				this.Camera.Orientation = StringConverter.ParseQuaternion( state[ "CameraOrientation" ] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>true if we should stop rendering!</returns>
		[OgreVersion( 1, 7, 2 )]
		public override bool FrameRenderingQueued( FrameEventArgs evt )
		{
			this.TrayManager.FrameRenderingQueued( evt );

			if ( !this.TrayManager.IsDialogVisible )
			{
				this.CameraManager.frameRenderingQueued( evt ); // if dialog isn't up, then update the camera

				if ( this.DetailsPanel.IsVisible ) // if details panel is visible, then update its contents
				{
					this.DetailsPanel.SetParamValue( 0, this.Camera.DerivedPosition.x.ToString() );
					this.DetailsPanel.SetParamValue( 1, this.Camera.DerivedPosition.y.ToString() );
					this.DetailsPanel.SetParamValue( 2, this.Camera.DerivedPosition.z.ToString() );
					this.DetailsPanel.SetParamValue( 4, this.Camera.DerivedOrientation.w.ToString() );
					this.DetailsPanel.SetParamValue( 5, this.Camera.DerivedOrientation.x.ToString() );
					this.DetailsPanel.SetParamValue( 6, this.Camera.DerivedOrientation.y.ToString() );
					this.DetailsPanel.SetParamValue( 7, this.Camera.DerivedOrientation.z.ToString() );
				}
			}

			return false;
		}

		[OgreVersion( 1, 7, 2 )]
		public override void WindowResized( RenderWindow rw )
		{
			this.Camera.AspectRatio = (Real)this.Viewport.ActualWidth/(Real)this.Viewport.ActualHeight;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public override bool KeyPressed( SIS.KeyEventArgs evt )
		{
			if ( evt.Key == SIS.KeyCode.Key_H || evt.Key == SIS.KeyCode.Key_F1 ) // toggle visibility of help dialog
			{
				if ( !this.TrayManager.IsDialogVisible && Metadata[ "Help" ] != "" )
				{
					this.TrayManager.ShowOkDialog( "Help", Metadata[ "Help" ] );
				}
				else
				{
					this.TrayManager.CloseDialog();
				}
			}

			if ( this.TrayManager.IsDialogVisible )
			{
				return true; // don't process any more keys if dialog is up
			}

			if ( evt.Key == SIS.KeyCode.Key_F ) // toggle visibility of advanced frame stats
			{
				this.TrayManager.ToggleAdvancedFrameStats();
			}
			else if ( evt.Key == SIS.KeyCode.Key_G ) // toggle visibility of even rarer debugging details
			{
				if ( this.DetailsPanel.TrayLocation == TrayLocation.None )
				{
					this.TrayManager.MoveWidgetToTray( this.DetailsPanel, TrayLocation.TopRight, 0 );
					this.DetailsPanel.Show();
				}
				else
				{
					this.TrayManager.RemoveWidgetFromTray( this.DetailsPanel );
					this.DetailsPanel.Hide();
				}
			}
			else if ( evt.Key == SIS.KeyCode.Key_T ) // cycle polygon rendering mode
			{
				String newVal;
				TextureFiltering tfo;
				int aniso;

				switch ( this.DetailsPanel.GetParamValue( 9 )[ 0 ] )
				{
					case 'B':
						newVal = "Trilinear";
						tfo = TextureFiltering.Trilinear;
						aniso = 1;
						break;
					case 'T':
						newVal = "Anisotropic";
						tfo = TextureFiltering.Anisotropic;
						aniso = 8;
						break;
					case 'A':
						newVal = "None";
						tfo = TextureFiltering.None;
						aniso = 1;
						break;
					default:
						newVal = "Bilinear";
						tfo = TextureFiltering.Bilinear;
						aniso = 1;
						break;
				}

				MaterialManager.Instance.SetDefaultTextureFiltering( tfo );
				MaterialManager.Instance.DefaultAnisotropy = aniso;
				this.DetailsPanel.SetParamValue( 9, newVal );
			}
			else if ( evt.Key == SIS.KeyCode.Key_R ) // cycle polygon rendering mode
			{
				String newVal;
				PolygonMode pm;

				switch ( this.Camera.PolygonMode )
				{
					case PolygonMode.Solid:
						newVal = "Wireframe";
						pm = PolygonMode.Wireframe;
						break;
					case PolygonMode.Wireframe:
						newVal = "Points";
						pm = PolygonMode.Points;
						break;
					default:
						newVal = "Solid";
						pm = PolygonMode.Solid;
						break;
				}

				this.Camera.PolygonMode = pm;
				this.DetailsPanel.SetParamValue( 10, newVal );
			}
			else if ( evt.Key == SIS.KeyCode.Key_F5 ) // refresh all textures
			{
				TextureManager.Instance.ReloadAll();
			}
			else if ( evt.Key == SIS.KeyCode.Key_F9 ) // take a screenshot
			{
				Window.WriteContentsToTimestampedFile( "screenshot", ".png" );
			}

			this.CameraManager.injectKeyDown( evt );

			return true;
		}

		[OgreVersion( 1, 7, 2 )]
		public override bool KeyReleased( SIS.KeyEventArgs evt )
		{
			this.CameraManager.injectKeyUp( evt );

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// IMPORTANT: When overriding this handler, remember to allow the tray manager
		/// to filter out any interface-related mouse events before processing them in your scene.
		/// If the tray manager handler returns true, the event was meant for the trays, not you.
		/// </remarks>
		[OgreVersion( 1, 7, 2 )]
		public override bool MouseMoved( SIS.MouseEventArgs evt )
		{
			if ( this.TrayManager.InjectMouseMove( evt ) )
			{
				return true;
			}

			this.CameraManager.injectMouseMove( evt );

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// IMPORTANT: When overriding this handler, remember to allow the tray manager
		/// to filter out any interface-related mouse events before processing them in your scene.
		/// If the tray manager handler returns true, the event was meant for the trays, not you.
		/// </remarks>
		/// <param name="evt"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[OgreVersion( 1, 7, 2 )]
		public override bool MousePressed( SIS.MouseEventArgs evt, SIS.MouseButtonID id )
		{
			if ( this.TrayManager.InjectMouseDown( evt, id ) )
			{
				return true;
			}

			if ( this.dragLook && id == SIS.MouseButtonID.Left )
			{
				this.CameraManager.setStyle( CameraStyle.FreeLook );
				this.TrayManager.HideCursor();
			}

			this.CameraManager.injectMouseDown( evt, id );

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// IMPORTANT: When overriding this handler, remember to allow the tray manager
		/// to filter out any interface-related mouse events before processing them in your scene.
		/// If the tray manager handler returns true, the event was meant for the trays, not you.
		/// </remarks>
		/// <param name="evt"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[OgreVersion( 1, 7, 2 )]
		public override bool MouseReleased( SIS.MouseEventArgs evt, SIS.MouseButtonID id )
		{
			if ( this.TrayManager.InjectMouseUp( evt, id ) )
			{
				return true;
			}

			if ( this.dragLook && id == SIS.MouseButtonID.Left )
			{
				this.CameraManager.setStyle( CameraStyle.Manual );
				this.TrayManager.ShowCursor();
			}

			this.CameraManager.injectMouseUp( evt, id );

			return true;
		}

		/// <summary>
		/// Extendeded to setup a default tray interface and camera controller.
		/// </summary>
		/// <param name="window"></param>
		/// <param name="keyboard"></param>
		/// <param name="mouse"></param>
		protected internal override void Setup( RenderWindow window, SIS.Keyboard keyboard, SIS.Mouse mouse )
		{
			Window = window;
			Keyboard = keyboard;
			Mouse = mouse;

			LocateResources();
			CreateSceneManager();
			SetupView();

			this.TrayManager = new SdkTrayManager( "SampleControls", window, mouse, this as ISdkTrayListener );
			// create a tray interface

			LoadResources();
			ResourcesLoaded = true;

			// Show stats and logo and Hide the cursor
			this.TrayManager.ShowFrameStats( TrayLocation.BottomLeft );
			this.TrayManager.ShowLogo( TrayLocation.BottomRight );
			this.TrayManager.HideCursor();

			// create a params panel for displaying sample details
			var items = new List<string>
			            {
			            	"cam.pX",
			            	"cam.pY",
			            	"cam.pZ",
			            	String.Empty,
			            	"cam.oW",
			            	"cam.oX",
			            	"cam.oY",
			            	"cam.oZ",
			            	String.Empty,
			            	"Filtering",
			            	"Poly Mode"
			            };
			this.DetailsPanel = this.TrayManager.CreateParamsPanel( TrayLocation.None, "DetailsPanel", 180, items );
			this.DetailsPanel.SetParamValue( 9, "Bilinear" );
			this.DetailsPanel.SetParamValue( 10, "Solid" );
			this.DetailsPanel.Hide();

			SetupContent();
			ContentSetup = true;

			IsDone = false;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Shutdown()
		{
			base.Shutdown();

			if ( this.TrayManager != null )
			{
				this.TrayManager.Dispose();
			}
			if ( this.CameraManager != null )
			{
				this.CameraManager = null;
			}

			// restore settings we may have changed, so as not to affect other samples
			MaterialManager.Instance.SetDefaultTextureFiltering( TextureFiltering.Bilinear );
			MaterialManager.Instance.DefaultAnisotropy = 1;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void SetupView()
		{
			// setup default viewport layout and camera
			this.Camera = SceneManager.CreateCamera( "MainCamera" );
			this.Viewport = Window.AddViewport( this.Camera );
			this.Camera.AspectRatio = (Real)this.Viewport.ActualWidth/(Real)this.Viewport.ActualHeight;
			this.Camera.Near = 5;

			this.CameraManager = new SdkCameraManager( this.Camera ); // create a default camera controller
		}

		#endregion Sample Implementation
	}
}