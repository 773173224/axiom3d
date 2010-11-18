using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Views;
using Android.Content;
using Axiom.Core;
using System.Reflection;
using Android.App;
using Axiom.Platform.Android;

namespace Droid
{
	class DemoView : AndroidGameView
	{

		private Root _engine;
		private bool _initialized = false;
		private Demos.Tutorial1 demo;

		public DemoView( Context handle )
			: base( handle )
		{

		}

		// This gets called when the drawing surface is ready
		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			// this.GLContextVersion = GLContextVersion.Gles1_1;
			// Run the render loop
			Run();
		}

		void Initialize()
		{
			try
			{
				//new AndroidAssetArchive( this.Context.Assets );

				// instantiate the Root singleton
				_engine = new Root( "AxiomDemos.log" );

				( new Axiom.RenderSystems.OpenGLES.GLESPlugin() ).Initialize();
				( new Axiom.Platform.Android.Plugin() ).Initialize();

				Root.Instance.RenderSystem = Root.Instance.RenderSystems[ "OpenGLES" ];

				_loadPlugins();

				_setupResources();

				demo = new Demos.Tutorial1();

				demo.Setup( this.GraphicsContext, this.Width, this.Height );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "An exception has occurred. See below for details:" );
				Console.WriteLine( BuildExceptionString( ex ) );
			}
		}

		protected override void OnRenderFrame( OpenTK.FrameEventArgs e )
		{
			base.OnRenderFrame( e );
			base.MakeCurrent();
			if ( !_initialized )
			{
				Initialize();
				_initialized = true;
			}

			try
			{

				if ( _engine != null )
				{
					_engine.RenderOneFrame();
				}
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "An exception has occurred. See below for details:" );
				Console.WriteLine( BuildExceptionString( ex ) );
			}
			//base.SwapBuffers();
		}

		protected override void OnUpdateFrame( OpenTK.FrameEventArgs e )
		{
			base.OnUpdateFrame( e );

		}

		public override void Close()
		{
			_engine = null;
			demo.Dispose();
			base.Close();
		}

		private void _setupResources()
		{
		}

		private void _loadPlugins()
		{
		}

		private static string BuildExceptionString( Exception exception )
		{
			string errMessage = string.Empty;

			errMessage += exception.Message + Environment.NewLine + exception.StackTrace;

			while ( exception.InnerException != null )
			{
				errMessage += BuildInnerExceptionString( exception.InnerException );
				exception = exception.InnerException;
			}

			return errMessage;
		}

		private static string BuildInnerExceptionString( Exception innerException )
		{
			string errMessage = string.Empty;

			errMessage += Environment.NewLine + " InnerException ";
			errMessage += Environment.NewLine + innerException.Message + Environment.NewLine + innerException.StackTrace;

			return errMessage;
		}
	}
}