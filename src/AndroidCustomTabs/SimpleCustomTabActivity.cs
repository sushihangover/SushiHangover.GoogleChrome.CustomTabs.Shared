using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.CustomTabs;
using Android.Support.V7.App;
using Android.Views;
using System;
using Uri = Android.Net.Uri;

namespace AndroidCustomTabs
{
	[Activity(Label = "@string/title_activity_simple_chrome_tab")]
	[MetaData("android.support.PARENT_ACTIVITY", Value=".DemoListActivity")]
	public class SimpleCustomTabActivity : AppCompatActivity, View.IOnClickListener
	{
		private EditText mUrlEditText;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_simple_custom_tab);

			FindViewById(Resource.Id.start_custom_tab).SetOnClickListener(this);

			mUrlEditText = (EditText)FindViewById(Resource.Id.url);
		}

		public void OnClick(View v)
		{
			int viewId = v.Id;

			switch (viewId)
			{
				case Resource.Id.start_custom_tab:
					var url = mUrlEditText.Text;
					var customTabsIntent = new CustomTabsIntent.Builder().Build();
					CustomTabActivityHelper.OpenCustomTab(this, customTabsIntent, Uri.Parse(url), (CustomTabActivityHelper.ICustomTabFallback)new WebviewFallback());
					break;
				default:
					throw new Exception("Unknown View Clicked");
			}
		}
	}
}
