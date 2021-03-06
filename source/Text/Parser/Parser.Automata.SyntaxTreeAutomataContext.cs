﻿// <copyright file="Parser.Automata.SyntaxTreeAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class SyntaxTreeAutomataContext : ValueParserAutomataContext
			{
				#region Ctors

				public SyntaxTreeAutomataContext(ParserState state, ParserAutomata parserAutomata) : base(state, parserAutomata)
				{
				}

				#endregion

				#region Properties

				private protected override Type ILBuilderType => typeof(SyntaxTreeAutomataContext);

				#endregion

				#region Methods

				public override void Dispose()
				{
					base.Dispose();

					((ParserState) State).ReleaseSyntaxTreeContext(this);
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}