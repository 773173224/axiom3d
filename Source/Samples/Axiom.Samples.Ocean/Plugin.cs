﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Axiom.Core;

namespace Axiom.Samples.Ocean
{
	[Export( typeof ( IPlugin ) )]
	public class Plugin : SamplePlugin
	{
		private OceanSample sample;

		public override void Initialize()
		{
			this.sample = new OceanSample();
			Name = this.sample.Metadata[ "Title" ] + " Sample";
			AddSample( this.sample );
		}
	}
}