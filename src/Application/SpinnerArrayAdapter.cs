using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace CustomTabsClientApplication
{

	class SpinnerArrayAdapter<Pair> : ArrayAdapter<Pair>
	{
		public SpinnerArrayAdapter(IntPtr handle, Android.Runtime.JniHandleOwnership transfer) : base(handle, transfer)
		{
		}

		public SpinnerArrayAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
		{
		}

		public SpinnerArrayAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
		{
		}

		public SpinnerArrayAdapter(Context context, int textViewResourceId, Pair[] objects) : base(context, textViewResourceId, objects)
		{
		}

		public SpinnerArrayAdapter(Context context, int resource, int textViewResourceId, Pair[] objects) : base(context, resource, textViewResourceId, objects)
		{
		}

		public SpinnerArrayAdapter(Context context, int textViewResourceId, IList<Pair> objects) : base(context, textViewResourceId, objects)
		{
		}

		public SpinnerArrayAdapter(Context context, int resource, int textViewResourceId, IList<Pair> objects) : base(context, resource, textViewResourceId, objects)
		{
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView;
			if (view == null)
			{
				view = LayoutInflater.From(Context).Inflate(Android.Resource.Layout.SimpleListItem2, parent, false);
			}
			//Pair<String, String> data = getItem(position);
			//new x = Android.Util.Pair.

			var data = GetItem(position) as Android.Util.Pair;
			((TextView)view.FindViewById(Android.Resource.Id.Text1)).Text = data.First.ToString();
			((TextView)view.FindViewById(Android.Resource.Id.Text2)).Text = data.Second.ToString();
			return view;
		}

		public override View GetDropDownView(int position, View convertView, ViewGroup parent)
		{
			return GetView(position, convertView, parent);
		}
	}
}
