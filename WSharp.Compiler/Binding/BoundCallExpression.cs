﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using WSharp.Compiler.Symbols;

namespace WSharp.Compiler.Binding
{
	public sealed class BoundCallExpression
		: BoundExpression
	{
		internal BoundCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments) =>
			(this.Function, this.Arguments) = (function, arguments);

		public override IEnumerable<BoundNode> GetChildren()
		{
			foreach(var argument in this.Arguments)
			{
				yield return argument;
			}
		}

		public override IEnumerable<(string name, object value)> GetProperties() => 
			Enumerable.Empty<(string, object)>();

		public ImmutableArray<BoundExpression> Arguments { get; }
		public FunctionSymbol Function { get; }
		public override BoundNodeKind Kind => BoundNodeKind.CallExpression;
		public override TypeSymbol Type => this.Function.ReturnType;
	}
}