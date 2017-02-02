using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.CustomTabs;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Uri = Android.Net.Uri;

namespace AndroidCustomTabs
{
	[Activity(Label = "@string/title_activity_customized_chrome_tab")]
	[MetaData("android.support.PARENT_ACTIVITY", Value = ".DemoListActivity")]
	public class CustomUIActivity : AppCompatActivity, View.IOnClickListener
	{
		const string TAG = "CustChromeTabActivity";
		const int TOOLBAR_ITEM_ID = 1;

		EditText mUrlEditText;
		EditText mCustomTabColorEditText;
		EditText mCustomTabSecondaryColorEditText;
		CheckBox mShowActionButtonCheckbox;
		CheckBox mAddMenusCheckbox;
		CheckBox mShowTitleCheckBox;
		CheckBox mCustomBackButtonCheckBox;
		CheckBox mAutoHideAppBarCheckbox;
		CheckBox mAddDefaultShareCheckbox;
		CheckBox mToolbarItemCheckbox;
		CustomTabActivityHelper mCustomTabActivityHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_custom_ui);

			mCustomTabActivityHelper = new CustomTabActivityHelper();
			FindViewById(Resource.Id.start_custom_tab).SetOnClickListener(this);

			mUrlEditText = (EditText)FindViewById(Resource.Id.url);
			mCustomTabColorEditText = (EditText)FindViewById(Resource.Id.custom_toolbar_color);
			mCustomTabSecondaryColorEditText =
					(EditText)FindViewById(Resource.Id.custom_toolbar_secondary_color);
			mShowActionButtonCheckbox = (CheckBox)FindViewById(Resource.Id.custom_show_action_button);
			mAddMenusCheckbox = (CheckBox)FindViewById(Resource.Id.custom_add_menus);
			mShowTitleCheckBox = (CheckBox)FindViewById(Resource.Id.show_title);
			mCustomBackButtonCheckBox = (CheckBox)FindViewById(Resource.Id.custom_back_button);
			mAutoHideAppBarCheckbox = (CheckBox)FindViewById(Resource.Id.auto_hide_checkbox);
			mAddDefaultShareCheckbox = (CheckBox)FindViewById(Resource.Id.add_default_share);
			mToolbarItemCheckbox = (CheckBox)FindViewById(Resource.Id.add_toolbar_item);
		}

		protected override void OnStart()
		{
			base.OnStart();
			mCustomTabActivityHelper.BindCustomTabsService(this);
		}

		protected override void OnStop()
		{
			base.OnStop();
			mCustomTabActivityHelper.UnbindCustomTabsService(this);
		}


		public void OnClick(View v)
		{
			int viewId = v.Id;
			switch (viewId)
			{
				case Resource.Id.start_custom_tab:
					OpenCustomTab();
					break;
					//default:
					//	//Unknown View Clicked
			}
		}

		void OpenCustomTab()
		{
			string url = mUrlEditText.Text;

			int color = getColor(mCustomTabColorEditText);
			int secondaryColor = getColor(mCustomTabSecondaryColorEditText);

			CustomTabsIntent.Builder intentBuilder = new CustomTabsIntent.Builder();
			intentBuilder.SetToolbarColor(color);
			intentBuilder.SetSecondaryToolbarColor(secondaryColor);

			if (mShowActionButtonCheckbox.Checked)
			{
				//Generally you do not want to decode bitmaps in the UI thread. Decoding it in the
				//UI thread to keep the example short.
				string actionLabel = GetString(Resource.String.label_action);
				Bitmap icon = BitmapFactory.DecodeResource(Resources, Android.Resource.Drawable.IcMenuShare);
				PendingIntent pendingIntent =
						CreatePendingIntent(ActionBroadcastReceiver.ACTION_ACTION_BUTTON);
				intentBuilder.SetActionButton(icon, actionLabel, pendingIntent);
			}

			if (mAddMenusCheckbox.Checked)
			{
				string menuItemTitle = GetString(Resource.String.menu_item_title);
				PendingIntent menuItemPendingIntent =
						CreatePendingIntent(ActionBroadcastReceiver.ACTION_MENU_ITEM);
				intentBuilder.AddMenuItem(menuItemTitle, menuItemPendingIntent);
			}

			if (mAddDefaultShareCheckbox.Checked)
			{
				intentBuilder.AddDefaultShareMenuItem();
			}

			if (mToolbarItemCheckbox.Checked)
			{
				//Generally you do not want to decode bitmaps in the UI thread. Decoding it in the
				//UI thread to keep the example short.
				string actionLabel = GetString(Resource.String.label_action);
				Bitmap icon = BitmapFactory.DecodeResource(Resources, Android.Resource.Drawable.IcMenuShare);
				PendingIntent pendingIntent = CreatePendingIntent(ActionBroadcastReceiver.ACTION_TOOLBAR);
				intentBuilder.AddToolbarItem(TOOLBAR_ITEM_ID, icon, actionLabel, pendingIntent);
			}

			intentBuilder.SetShowTitle(mShowTitleCheckBox.Checked);
			if (mAutoHideAppBarCheckbox.Checked)
			{
				intentBuilder.EnableUrlBarHiding();
			}

			if (mCustomBackButtonCheckBox.Checked)
			{
				intentBuilder.SetCloseButtonIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_arrow_back));
			}

			intentBuilder.SetStartAnimations(this, Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
			intentBuilder.SetExitAnimations(this, Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);

			CustomTabActivityHelper.OpenCustomTab(this, intentBuilder.Build(), Uri.Parse(url), new WebviewFallback());
		}

		int getColor(EditText editText)
		{
			try
			{
				return Color.ParseColor(editText.Text);
			}
			catch (NumberFormatException ex)
			{
				Log.Info(TAG, $"Unable to parse Color: {editText.Text} : \n{ex.Message}");
				return Color.LightGray;
			}
		}

		PendingIntent CreatePendingIntent(int actionSourceId)
		{
			var actionIntent = new Intent(ApplicationContext, typeof(ActionBroadcastReceiver));
			actionIntent.PutExtra(ActionBroadcastReceiver.KEY_ACTION_SOURCE, actionSourceId);
			return PendingIntent.GetBroadcast(ApplicationContext, actionSourceId, actionIntent, 0);
		}
	}
}
