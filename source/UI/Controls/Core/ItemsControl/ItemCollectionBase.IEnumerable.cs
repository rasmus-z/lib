﻿// <copyright file="ItemCollectionBase.IEnumerable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		#region Interface Implementations

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#endregion
	}
}