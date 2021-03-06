// <copyright file="VisualStateMap.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	internal static class VisualStateMap
	{
		#region Static Fields and Constants

		private static readonly List<string> StateList = new List<string>(2048) {"Unset"};
		private static readonly Dictionary<string, uint> StateIndexMap = new Dictionary<string, uint>();

		#endregion

		#region  Methods

		public static uint GetStateIndex(string stateName)
		{
			if (stateName.IsNullOrEmpty())
				return 0u;

			var currentIndex = StateIndexMap.GetValueOrDefault(stateName);

			if (currentIndex != 0u)
				return currentIndex;

			StateIndexMap[stateName] = currentIndex = (uint) StateList.Count;
			StateList.Add(stateName);

			return currentIndex;
		}

		public static string GetStateName(uint index)
		{
			return StateList[(int) index];
		}

		#endregion
	}
}