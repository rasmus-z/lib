﻿// <copyright file="Grammar.PrimitiveMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract class PrimitiveMatchEntry : MatchEntry
		{
			#region Methods

			public static implicit operator PrimitiveMatchEntry(char c)
			{
				return new CharEntry(c);
			}

#if NETCORE
			public static implicit operator PrimitiveMatchEntry(Range range)
			{
				return new RangeEntry((char) range.Start.Value, (char) range.End.Value);
			}
#endif

#endregion
		}

#endregion
	}
}