﻿#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace Lidgren.Core
{
	/// <summary>
	/// 32 bit sequential id. Use Id32.Next() to generate. Valid ids starts at 1.
	/// </summary>
	[DebuggerDisplay("{m_id}")]
	[JsonConverter(typeof(Id32JsonConverter))]
	public readonly struct Id32 : IEquatable<Id32>, IComparable<Id32>, IComparable<int>, IEquatable<int>
	{
		public static readonly Id32 Invalid = new Id32(0);

		private static int s_lastId = 0;

		private readonly int m_id;

		public Id32(int value)
		{
			m_id = value;
		}

		public static Id32 PeekNext()
		{
			return new Id32(s_lastId + 1);
		}

		public static Id32 Next()
		{
			return new Id32(Interlocked.Increment(ref s_lastId));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator int(Id32 id)
		{
			return id.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Id32(int val)
		{
			return new Id32(val);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo(Id32 other)
		{
			if (m_id < other.m_id)
				return -1;
			if (m_id > other.m_id)
				return 1;
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int CompareTo(int other)
		{
			if (m_id < other)
				return -1;
			if (m_id > other)
				return 1;
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? other)
		{
			return ((other is null) == false) && m_id == ((Id32)other).m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Id32 other)
		{
			return m_id == other.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Id32 x, Id32 y)
		{
			return x.m_id == y.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(Id32 x, Id32 y)
		{
			return x.m_id >= y.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(Id32 x, Id32 y)
		{
			return x.m_id <= y.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Id32 x, Id32 y)
		{
			return x.m_id != y.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(Id32 x, Id32 y)
		{
			return x.m_id < y.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(Id32 x, Id32 y)
		{
			return x.m_id > y.m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return m_id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return m_id.ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(int other)
		{
			return m_id == other;
		}
	}

	public class Id32JsonConverter : JsonConverter<Id32>
	{
		public static readonly Id32JsonConverter Instance = new Id32JsonConverter();

		public override Id32 Read(ref Utf8JsonReader rdr, Type typeToConvert, JsonSerializerOptions options) => (Id32)rdr.GetInt32();

		public override void Write(Utf8JsonWriter wrt, Id32 value, JsonSerializerOptions options)
		{
			wrt.WriteNumberValue((int)value);
		}
	}
}
