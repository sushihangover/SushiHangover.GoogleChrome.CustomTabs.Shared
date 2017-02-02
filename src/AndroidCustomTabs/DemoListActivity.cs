using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace AndroidCustomTabs
{
	[Activity(Label = "DemoListActivity", Name="com.sushhangover.androidcustomtabs.DemoListActivity", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
	[IntentFilter(new[] { "android.intent.action.MAIN" })]
	public class DemoListActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			List<ActivityDesc> activityDescList = new List<ActivityDesc>();

			ActivityListAdapter listAdapter = new ActivityListAdapter(this, activityDescList);

			var activityDesc = CreateActivityDesc(
				Resource.String.title_activity_simple_chrome_tab,
				Resource.String.description_activity_simple_chrome_tab,
				Class.FromType(typeof(SimpleCustomTabActivity)));
			activityDescList.Add(activityDesc);

			activityDesc = CreateActivityDesc(
				Resource.String.title_activity_service_connection,
				Resource.String.description_activity_service_connection,
				Class.FromType(typeof(ServiceConnectionActivity)));
			activityDescList.Add(activityDesc);

			activityDesc = CreateActivityDesc(
				Resource.String.title_activity_customized_chrome_tab,
				Resource.String.description_activity_customized_chrome_tab,
				Class.FromType(typeof(CustomUIActivity)));
			activityDescList.Add(activityDesc);

			activityDesc = CreateActivityDesc(
				Resource.String.title_activity_notification_parent,
				Resource.String.title_activity_notification_parent,
				Class.FromType(typeof(NotificationParentActivity)));
			activityDescList.Add(activityDesc);

			var recyclerView = (RecyclerView)FindViewById(Android.Resource.Id.List);
			recyclerView.SetAdapter(listAdapter);
			recyclerView.SetLayoutManager(new LinearLayoutManager(this));
		}

		ActivityDesc CreateActivityDesc(int titleId, int descriptionId, Java.Lang.Class activity)
		{
			ActivityDesc activityDesc = new ActivityDesc();
			activityDesc.mTitle = GetString(titleId);
			activityDesc.mDescription = GetString(descriptionId);
			activityDesc.mActivity = activity;
			return activityDesc;
		}

		class ActivityDesc
		{
			public string mTitle;
			public string mDescription;
			public Class mActivity;
		}

		class CustomViewHolder : RecyclerView.ViewHolder
		{
			/* package */
			public TextView mTitleTextView;
			/* package */
			public TextView mDescriptionTextView;
			/* package */
			public int mPosition;

			public CustomViewHolder(View itemView) : base(itemView)
			{
				mTitleTextView = (TextView)itemView.FindViewById(Resource.Id.title);
				mDescriptionTextView = (TextView)itemView.FindViewById(Resource.Id.description);
			}
		}

		class ActivityListAdapter : RecyclerView.Adapter, View.IOnClickListener
		{

			readonly Context mContext;
			readonly LayoutInflater mLayoutInflater;
			readonly List<ActivityDesc> mActivityDescs;

			public override int ItemCount
			{
				get
				{
					return mActivityDescs.Count;
				}
			}

			public ActivityListAdapter(Context context, List<ActivityDesc> activityDescs)
			{
				mActivityDescs = activityDescs;
				mContext = context;
				mLayoutInflater = LayoutInflater.From(context);
			}

			public void OnClick(View v)
			{
				int position = ((CustomViewHolder)v.Tag).mPosition;
				ActivityDesc activityDesc = mActivityDescs[position];
				Intent intent = new Intent(mContext, activityDesc.mActivity);
				mContext.StartActivity(intent);
			}

			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				View v = mLayoutInflater.Inflate(Resource.Layout.item_example_description, parent, false);
				CustomViewHolder viewHolder = new CustomViewHolder(v);
				v.SetOnClickListener(this);
				v.Tag = viewHolder;
				return viewHolder;
			}

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				var viewHolder = holder as CustomViewHolder;
				ActivityDesc activityDesc = mActivityDescs[position];
				var title = activityDesc.mTitle;
				var description = activityDesc.mDescription;

				viewHolder.mTitleTextView.SetText(title, TextView.BufferType.Normal);
				viewHolder.mDescriptionTextView.SetText(description, TextView.BufferType.Normal);
				viewHolder.mPosition = position;
			}
		}
	}
}