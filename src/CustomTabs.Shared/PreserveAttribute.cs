using System;
using Android;

[assembly: LinkerSafe]

namespace Android.Support.CustomTabs.Shared
{
	public sealed class PreserveAttribute : Attribute
	{
		public bool AllMembers;
		public bool Conditional;
	}
}
