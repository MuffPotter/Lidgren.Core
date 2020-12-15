﻿#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace Lidgren.Core
{
	/// <summary>
	/// Replacement for StringBuilder with differences:
	/// 1. It's a value type
	/// 2. It takes spans
	/// 3. Has indentation support
	/// 4. Newlines are just \n
	/// </summary>
	public struct ValueStringBuilder
	{
		private static readonly string s_tabs = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
		private char[] m_buffer;
		private int m_length;
		private int m_column;
		private int m_indentionLevel;

		public readonly int Length => m_length;
		public readonly Span<char> Span => m_buffer.AsSpan(0, m_length);
		public readonly ReadOnlySpan<char> ReadOnlySpan => m_buffer.AsSpan(0, m_length);

		public ValueStringBuilder(int initialCapacity)
		{
			m_buffer = new char[initialCapacity];
			m_length = 0;
			m_column = 0;
			m_indentionLevel = 0;
		}

		public void Clear()
		{
			m_length = 0;
			m_column = 0;
			m_indentionLevel = 0;
		}

		public int Capacity
		{
			readonly get { return m_buffer.Length; }
			set
			{
				var newBuffer = new char[value];
				m_buffer.AsSpan(0, m_length).CopyTo(newBuffer);
				m_buffer = newBuffer;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacity(int len)
		{
			if (len > m_buffer.Length - m_length)
				Grow(len);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grow(int len)
		{
			int newSize = Math.Max(m_length + len, m_buffer.Length * 2);
			Capacity = newSize;
		}

		public void AppendLine()
		{
			EnsureCapacity(1);
			m_buffer[m_length++] = '\n';
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Indent(int add)
		{
			m_indentionLevel += add;
		}

		// remaining MUST have room for m_indentionLevel characters, and span will be modified
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MaybeIndent(ref Span<char> span)
		{
			if (m_column != 0)
				return;
			var lvl = m_indentionLevel;
			if (lvl == 0)
				return;
			s_tabs.AsSpan(0, lvl).CopyTo(m_buffer.AsSpan(m_length));
			span = span.Slice(lvl);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(string str)
		{
			AppendLine(str.AsSpan());
		}

		public void AppendLine(ReadOnlySpan<char> str)
		{
			if (str.Length == 0)
			{
				AppendLine();
				return;
			}

			var len = str.Length + m_indentionLevel + 1;

			EnsureCapacity(len);
			var span = m_buffer.AsSpan(m_length, len);

			MaybeIndent(ref span); // add indention
			str.CopyTo(span); // add str
			span = span.Slice(str.Length);
			span[0] = '\n'; // add newline
			m_length += len;
			m_column = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(string str)
		{
			Append(str.AsSpan());
		}

		public void Append(ReadOnlySpan<char> str)
		{
			if (str.Length == 0)
				return;
			var len = str.Length + m_indentionLevel;
			EnsureCapacity(len);
			var span = m_buffer.AsSpan(m_length, len);
			MaybeIndent(ref span); // add indention
			str.CopyTo(span); // add str
			m_length += len;
			m_column = str.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(char c)
		{
			var len = 1 + m_indentionLevel;
			EnsureCapacity(len);
			var span = m_buffer.AsSpan(m_length, len);
			MaybeIndent(ref span); // add indention
			span[0] = c;
			m_length += len;
			m_column += 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(bool value)
		{
			var maxLen = 5 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(int value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(uint value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(byte value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(short value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ushort value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(long value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ulong value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(float value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(double value)
		{
			var maxLen = 12 + m_indentionLevel;
			EnsureCapacity(maxLen);

			var span = m_buffer.AsSpan(m_length, maxLen);
			MaybeIndent(ref span); // add indention

			bool ok = value.TryFormat(span, out int written);
			CoreException.Assert(ok);

			var actualLen = m_indentionLevel + written;
			m_length += actualLen;
			m_column += actualLen;
		}

		public override int GetHashCode()
		{
			return (int)HashUtil.Hash32(ReadOnlySpan);
		}

		public override string ToString()
		{
			return ReadOnlySpan.ToString();
		}
	}
}
