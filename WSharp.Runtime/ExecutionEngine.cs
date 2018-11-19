﻿using Spackle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;

namespace WSharp.Runtime
{
	public sealed class ExecutionEngine
		: IExecutionEngineActions
	{
		private readonly ImmutableDictionary<ulong, Line> lines;
		private readonly Random random;
		private bool shouldStatementBeDeferred;

		public ExecutionEngine(ImmutableList<Line> lines, Random random)
		{
			this.random = random ?? throw new ArgumentNullException(nameof(random));

			if (lines == null)
			{
				throw new ArgumentNullException(nameof(lines));
			}
			else if(lines.Count == 0)
			{
				throw new ArgumentException("Must pass in at least one line.", nameof(lines));
			}
			else
			{
				var messages = new List<string>();

				for(var i = 0; i < lines.Count; i++)
				{
					if(lines[i] == null)
					{
						messages.Add($"The line at index {i} is null.");
					}
				}

				if(messages.Count > 0)
				{
					throw new ExecutionEngineLinesException(messages.ToImmutableList());
				}
			}

			var indexedLines = ImmutableDictionary.CreateBuilder<ulong, Line>();

			foreach(var line in lines)
			{
				indexedLines.Add(line.Identifier, line);
			}

			this.lines = indexedLines.ToImmutable();
		}

		public BigInteger GetCurrentLineCount()
		{
			var lineCount = BigInteger.Zero;

			foreach(var line in this.lines.Values)
			{
				lineCount = lineCount + line.Count;
			}

			return lineCount;
		}

		public bool Defer(bool shouldDefer)
		{
			this.shouldStatementBeDeferred = shouldDefer;
			return shouldDefer;
		}

		public bool DoesLineExist(ulong identifier)
		{
			if(this.lines.TryGetValue(identifier, out var line))
			{
				return line.Count > 0;
			}
			else
			{
				return false;
			}
		}

		public bool Execute()
		{
			this.shouldStatementBeDeferred = false;
			var currentLineCount = this.GetCurrentLineCount();

			if(currentLineCount > 0)
			{
				var buffer = currentLineCount.ToByteArray();
				this.random.NextBytes(buffer);

				var generated = new BigInteger(buffer) % currentLineCount;
				var currentLowerBound = BigInteger.Zero;

				// TODO: We should never come out of this foreach
				// without executing a line.
				foreach (var line in this.lines.Values)
				{
					var range = new Range<BigInteger>(currentLowerBound, line.Count + currentLowerBound - 1);
					if (range.Contains(generated))
					{
						line.Code(this);

						if (!this.shouldStatementBeDeferred)
						{
							var newLine = line.UpdateCount(-1);
						}

						break;
					}
					else
					{
						currentLowerBound += line.Count;
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public ulong N(ulong identifier) => 
			throw new NotImplementedException();

		public string U(long number) => number.ToString();
	}
}
