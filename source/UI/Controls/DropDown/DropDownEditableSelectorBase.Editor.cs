// <copyright file="DropDownEditableSelectorBase.Editor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Editors.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem>
	{
		public event EventHandler EditingStarted;
		public event EventHandler<EditingEndedEventArgs> EditingEnded;
		private DateTime _lastTextInputTime = DateTime.MinValue;

		protected virtual bool ActualOpenDropDownOnEditing => OpenDropDownOnEditing;

		protected virtual bool ActualShouldPreserveText
		{
			get
			{
				return PreserveText switch
				{
					PreserveTextMode.False => false,
					PreserveTextMode.True => true,
					PreserveTextMode.Auto => AutoPreserveText,
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		protected virtual bool AutoPreserveText => true;

		protected abstract FrameworkElement Editor { get; }

		private void BeginEditDropDownSuspended()
		{
			try
			{
				SuspendOpenDropDown = true;

				BeginEdit();
			}
			finally
			{
				SuspendOpenDropDown = false;
			}
		}

		public bool BeginEdit()
		{
			if (IsEditing || IsEditable == false)
				return false;

			IsEditing = true;

			OnBeginEdit();

			if (IsEditing == false)
				return false;

			EnterEditState();

			return IsEditing;
		}

		public bool CancelEdit()
		{
			if (IsEditing == false)
				return false;

			var cancelResult = OnCancelEdit();

			if (cancelResult == false)
				return false;

			IsEditing = false;

			OnEndEdit(EditingEndedEventArgs.CancelArgs);

			if (IsEditing)
				return false;

			LeaveEditState();

			return IsEditing == false;
		}

		private void CancelEditingPrivate()
		{
			CancelEdit();
		}

		private object CoerceIsEditing(object isEditing)
		{
			var boolIsEditing = (bool) isEditing;

			return boolIsEditing && IsEditable;
		}

		private static string CoerceText(string text)
		{
			return text ?? string.Empty;
		}

		public bool CommitEdit()
		{
			if (IsEditing == false)
				return false;

			var commitResult = OnCommitEdit();

			if (commitResult == false)
				return false;

			IsEditing = false;

			OnEndEdit(EditingEndedEventArgs.CommitArgs);

			if (IsEditing)
				return false;

			LeaveEditState();

			return IsEditing == false;
		}

		private protected virtual void EnterEditState()
		{
			if (IsTemplateAttached == false)
				ApplyTemplate();

			if (Editor != null)
				Editor.Visibility = Visibility.Visible;

			if (Editor is TemplateContractControl templateContractControl && templateContractControl.IsTemplateAttached == false)
				templateContractControl.ApplyTemplate();

			if (IsEditorFocused == false)
				FocusEditor(false);

			if (SuspendOpenDropDown == false && ActualOpenDropDownOnEditing && IsEditing && IsDropDownOpen != true)
				OpenDropDown();
		}

		private protected virtual void LeaveEditState()
		{
			if (Editor != null)
				Editor.Visibility = Visibility.Collapsed;

			if (ActualShouldPreserveText == false)
				ResetText();

			EffectiveKeyboard = null;

			try
			{
				SuspendFocusHandler();
				SuspendDropDownHandler();

				var focus = false;

				if (IsDescendant(FocusHelper.GetKeyboardFocusedElement()))
				{
					Keyboard.ClearFocus();

					focus = true;
				}

				if (focus)
					Keyboard.Focus(this);
			}
			finally
			{
				ResumeFocusHandler();
				ResumeDropDownHandler();
			}
		}

		protected virtual void OnBeginEdit()
		{
			EditingStarted?.Invoke(this, EventArgs.Empty);
		}

		protected virtual bool OnCancelEdit()
		{
			return SafeCompleteEditing(base.CancelSelectionCore);
		}

		protected virtual bool OnCommitEdit()
		{
			return SafeCompleteEditing(base.CommitSelectionCore);
		}

		protected virtual void OnEndEdit(EditingEndedEventArgs args)
		{
			EditingEnded?.Invoke(this, args);
		}

		protected virtual void OnIsEditingChanged()
		{
		}

		private void OnIsEditingPropertyChangedPrivate()
		{
			OnIsEditingChanged();

			UpdateVisualState(true);
		}

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			if (string.Equals(e.Text, "\u001b") && IsEditing == false)
				return;

			if (IsEditing == false) 
				BeginEditDropDownSuspended();
			else if (IsDropDownOpen == false && OpenDropDownOnEditing)
				OpenDropDownOnTextInput(false);

			_lastTextInputTime = DateTime.Now;

			base.OnPreviewTextInput(e);
		}

		private void OnTextPropertyChangedPrivate(string oldText, string newText)
		{
			if (IsEditing == false || IsDropDownOpen)
				return;

			OpenDropDownOnTextInput(DateTime.Now - _lastTextInputTime > GetFilterDelay());
		}
		
		private protected virtual TimeSpan GetFilterDelay()
		{
			return TimeSpan.Zero;
		}

		private protected virtual void ForceFilterUpdate()
		{
		}

		protected virtual void ResetText()
		{
			this.SetCurrentValueInternal(TextProperty, string.Empty);
		}

		private bool SafeCompleteEditing(Func<bool> action)
		{
			try
			{
				SuspendFocusHandler();
				SuspendDropDownHandler();

				if (action?.Invoke() == false)
					return false;
			}
			finally
			{
				ResumeFocusHandler();
				ResumeDropDownHandler();
			}

			return true;
		}
	}
}