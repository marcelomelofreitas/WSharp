﻿using System;

namespace WSharp.Compiler.Binding
{
	[Flags]
	public enum ConversionFlags
	{
		None = 0, 
		Exists = 1, 
		IsIdentity = 2, 
		IsImplicit = 4
	}
}