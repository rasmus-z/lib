﻿// <copyright file="Automata.DfaBuilder.Key.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private protected abstract partial class DfaBuilder<TDfaState> where TDfaState : DfaState<TDfaState>
		{
			#region Nested Types

			private abstract class DfaStateKey
			{
				#region Methods

				private bool Equals(DfaStateKey other)
				{
					return ReferenceEquals(GetNodes(), other.GetNodes()) &&
					       ReferenceEquals(GetLazyTransitions(), other.GetLazyTransitions()) &&
					       ReferenceEquals(GetSuccessTransition(), other.GetSuccessTransition()) &&
					       ReferenceEquals(GetPrevSuccessTransition(), other.GetPrevSuccessTransition());
				}

				public override bool Equals(object obj)
				{
					return Equals((DfaStateKey) obj);
				}

				public override int GetHashCode()
				{
					return GetKeyHashCode();
				}

				protected abstract int GetKeyHashCode();

				protected abstract DfaTransition[] GetLazyTransitions();

				protected abstract DfaNode[] GetNodes();

				protected abstract DfaTransition GetPrevSuccessTransition();

				protected abstract DfaTransition GetSuccessTransition();

				#endregion
			}

			private sealed class DfaFrozenStateKey : DfaStateKey
			{
				#region Fields

				private readonly TDfaState _state;

				#endregion

				#region Ctors

				public DfaFrozenStateKey(TDfaState state)
				{
					_state = state;
				}

				#endregion

				#region Methods

				protected override int GetKeyHashCode()
				{
					return _state.HashCode;
				}

				protected override DfaTransition[] GetLazyTransitions()
				{
					return _state.LazyTransitions;
				}

				protected override DfaNode[] GetNodes()
				{
					return _state.Nodes;
				}

				protected override DfaTransition GetPrevSuccessTransition()
				{
					return _state.PrevSuccessTransition;
				}

				protected override DfaTransition GetSuccessTransition()
				{
					return _state.SuccessTransition;
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}