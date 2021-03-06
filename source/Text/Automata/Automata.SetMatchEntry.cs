﻿// <copyright file="Automata.SetMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected class SetMatchEntry : MatchEntry
		{
			#region Ctors

			public SetMatchEntry(IEnumerable<PrimitiveMatchEntry> matches)
			{
				Matches = matches.ToArray();
			}

			#endregion

			#region Properties

			protected override string DebuggerDisplay
			{
				get
				{
					var setStr = string.Join(",", Matches.Select(m => m.ToString()));

					return $"{{{setStr}}}";
				}
			}

			public PrimitiveMatchEntry[] Matches { get; }

			#endregion

			#region Methods

			public override bool Match(TOperand operand)
			{
				for (var index = 0; index < Matches.Length; index++)
				{
					var match = Matches[index];

					if (match.Match(operand))
						return true;
				}

				return false;
			}

			public override bool Match(int operand)
			{
				for (var index = 0; index < Matches.Length; index++)
				{
					var match = Matches[index];

					if (match.Match(operand))
						return true;
				}

				return false;
			}

			#endregion
		}

		#endregion
	}
}