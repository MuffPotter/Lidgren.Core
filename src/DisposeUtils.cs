﻿#nullable enable
using System;

namespace Lidgren.Core
{
	public static class DisposeUtils
	{
		/// <summary>
		/// Only use if Dispose(ref ob) cannot be used
		/// </summary>
		public static void Dispose(object ob)
		{
			var copy = ob as IDisposable;
			if (copy != null)
				copy.Dispose();
		}

		/// <summary>
		/// If non-null; calls dispose and nulls this reference. Locks on 'this' if no lock object provided, concurrent safe, will dispose exactly one time.
		/// </summary>
		public static bool Dispose<T>(ref T? ob, object? lockObject = null) where T : class, IDisposable
		{
			var myRef = ob;

			if (lockObject == null)
				lockObject = myRef;

			if (myRef != null)
			{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				lock (lockObject)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
				{
					if (ob != myRef)
						return false; // someone beat us to it
					try
					{
						ob.Dispose();
						return true;
					}
					catch
					{
						return false;
					}
					finally
					{
						ob = null;
					}
				}
			}
			return false;
		}
	}
}

