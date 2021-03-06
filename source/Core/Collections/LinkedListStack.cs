﻿// <copyright file="LinkedListStack.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Pools;

namespace Zaaml.Core.Collections
{
	internal sealed class LinkedListStack<T> : PoolSharedObject<LinkedListStack<T>>
	{
		#region Fields

		private readonly LinkedListStackNodePool<T> _nodePool;

		private LinkedListStackNode<T> _tail;

		#endregion

		#region Ctors

		public LinkedListStack(LinkedListStackNodePool<T> nodePool, IPool<LinkedListStack<T>> listPool) : base(listPool)
		{
			_nodePool = nodePool;
			NodeSize = nodePool.NodeSize;
		}

		#endregion

		#region Properties

		public int Count { get; private set; }

		public int NodeSize { get; }

		#endregion

		#region Methods

		public ref T PeekRef()
		{
			return ref _tail.Array[_tail.Count - 1];
		}

		public T Pop()
		{
			if (Count == 0)
				throw new InvalidOperationException();

			if (_tail.Count == 0)
			{
				var prev = _tail.Prev;

				_tail.Prev = null;

				var releaseNode = _tail;

				Array.Clear(releaseNode.Array, 0, releaseNode.Array.Length);

				_nodePool.Release(releaseNode);

				_tail = prev;
			}

			Count--;

			return _tail.Array[--_tail.Count];
		}

		public void Push(T thread)
		{
			if (_tail == null || _tail.Count == NodeSize)
			{
				var node = _nodePool.Get();

				node.Prev = _tail;
				_tail = node;
			}

			_tail.Array[_tail.Count++] = thread;

			Count++;
		}

		public void PushRef(ref T thread)
		{
			if (_tail == null || _tail.Count == NodeSize)
			{
				var node = _nodePool.Get();

				node.Prev = _tail;
				_tail = node;
			}

			_tail.Array[_tail.Count++] = thread;

			Count++;
		}

		#endregion
	}

	internal sealed class LinkedListStackNode<T>
	{
		#region Fields

		public readonly T[] Array;
		public int Count;
		public LinkedListStackNode<T> Prev;

		#endregion

		#region Ctors

		public LinkedListStackNode(int nodeSize)
		{
			Array = new T[nodeSize];
		}

		#endregion
	}

	internal sealed class LinkedListStackNodePool<T> : IPool<LinkedListStackNode<T>>
	{
		#region Fields

		private readonly Stack<LinkedListStackNode<T>> _stackPool = new Stack<LinkedListStackNode<T>>();

		#endregion

		#region Ctors

		public LinkedListStackNodePool(int nodeSize)
		{
			NodeSize = nodeSize;
		}

		#endregion

		#region Properties

		public int NodeSize { get; }

		#endregion

		#region Interface Implementations

		#region IPool<LinkedListStackNode<T>>

		public LinkedListStackNode<T> Get()
		{
			return _stackPool.Count > 0 ? _stackPool.Pop() : new LinkedListStackNode<T>(NodeSize);
		}

		public void Release(LinkedListStackNode<T> item)
		{
			if (NodeSize != item.Array.Length)
				throw new InvalidOperationException();

			_stackPool.Push(item);
		}

		#endregion

		#endregion
	}
}