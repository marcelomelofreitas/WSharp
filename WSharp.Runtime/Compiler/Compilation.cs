﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using WSharp.Runtime.Compiler.Binding;
using WSharp.Runtime.Compiler.Syntax;

namespace WSharp.Runtime.Compiler
{
	public sealed class Compilation
	{
		public Compilation(SyntaxTree tree) => this.Tree = tree;

		public EvaluationResult Evaluate()
		{
			var binder = new Binder();
			var statement = binder.BindCompilationUnit(this.Tree.Root);

			var diagnostics = this.Tree.Diagnostics.Concat(binder.Diagnostics).ToArray();

			if(diagnostics.Length > 0)
			{
				return new EvaluationResult(diagnostics.ToImmutableArray(), ImmutableArray<Line>.Empty);
			}

			var evaluator = new Evaluator(new List<BoundStatement> { statement });
			var lines = evaluator.Evaluate();

			return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, lines);
		}

		public SyntaxTree Tree { get; }
	}
}