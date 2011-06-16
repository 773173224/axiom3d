﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using SlimDX.Direct3D9;

namespace Axiom.RenderSystems.DirectX9
{
    public class D3D9DriverList : DisposableObject, IEnumerable<Driver>
    {
        [OgreVersion(1, 7)]
        private readonly List<Driver> _driverList = new List<Driver>();

        [OgreVersion(1, 7)]
        public D3D9DriverList()
        {
            Enumerate();
        }

        [OgreVersion(1, 7)]
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
                _driverList.Clear();
        }

        [OgreVersion(1, 7, "Update D3DDriver constructor")]
        public bool Enumerate()
        {
            var d3D = D3DRenderSystem.Direct3D9;

		    LogManager.Instance.Write( "D3D9: Driver Detection Starts" );
            for (var iAdaptor = 0; iAdaptor < d3D.AdapterCount; iAdaptor++)
            {
                /*
                var adapterIdentifier = d3D.GetAdapterIdentifier( iAdaptor );
                var d3ddm = d3D.GetAdapterDisplayMode( iAdaptor );
                var d3dcaps9 = d3D.GetAdapterDisplayMode( iAdaptor );
                */

                _driverList.Add( new Driver( d3D.Adapters[ iAdaptor ] ) );
            }

            LogManager.Instance.Write( "D3D9: Driver Detection Ends" );
		    return true;
	    }

        [OgreVersion(1, 7)]
        public int Count
        {
            get
            {
                return _driverList.Count;
            }
        }

        [OgreVersion(1, 7)]
        public Driver this[int index]
        {
            get
            {
                return _driverList[ index ];
            }
        }

        [OgreVersion(1, 7)]
        public Driver this[string name]
        {
            get
            {
                return _driverList.FirstOrDefault(d => d.Description == name);
            }
        }


        [AxiomHelper(0, 8)]
        public IEnumerator<Driver> GetEnumerator()
        {
            return _driverList.GetEnumerator();
        }

        [AxiomHelper(0, 8)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
