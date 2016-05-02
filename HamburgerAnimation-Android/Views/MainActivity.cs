using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace HamburgerAnimation_Android {
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity {

        private NavigationDrawerFragment navigationDrawerFragment;
        private DrawerLayout drawerLayout;
        private IDrawerToggle drawerToggle;

        private FrameLayout contentLayout;

        private string currentFragment;
        private FirstFragment firstFragment = new FirstFragment();
        private SecondFragment secondFragment = new SecondFragment();

        private Runnable[] runnables = new Runnable[NavigationDrawerFragment.ANIMATION_DURATION + 1];

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            SetContentView(Resource.Layout.Activity_Main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Android.Support.V7.App.ActionBar actionBar = SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(true);
            actionBar.SetHomeButtonEnabled(true);

            navigationDrawerFragment = (NavigationDrawerFragment)SupportFragmentManager.FindFragmentById(Resource.Id.navigation_drawer);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            navigationDrawerFragment.Setup(drawerLayout, toolbar);
            navigationDrawerFragment.DrawerToggle.ToolbarNavigationClickListener = new ActionBarDrawerToggle.LambdaActionListener(view => {
                if (currentFragment == typeof(FirstFragment).Name) {
                    SupportFragmentManager.BeginTransaction()
                        .SetCustomAnimations(Resource.Animation.slide_left_enter, Resource.Animation.slide_left_exit)
                        .Replace(Resource.Id.content_layout, secondFragment)
                        .Commit();

                    currentFragment = typeof(SecondFragment).Name;

                    drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
                    for (int i = 0; i < runnables.Length; i++)
                        drawerLayout.PostDelayed(runnables[i], i);
                    drawerLayout.PostDelayed(() => {
                        navigationDrawerFragment.DrawerToggle.DrawerIndicatorEnabled = false;
                    }, NavigationDrawerFragment.ANIMATION_DURATION);
                } else {
                    SupportFragmentManager.BeginTransaction()
                        .SetCustomAnimations(Resource.Animation.slide_right_enter, Resource.Animation.slide_right_exit)
                        .Replace(Resource.Id.content_layout, firstFragment)
                        .Commit();

                    currentFragment = typeof(FirstFragment).Name;

                    navigationDrawerFragment.DrawerToggle.DrawerIndicatorEnabled = true;
                    for (int i = 0; i < runnables.Length; i++)
                        drawerLayout.PostDelayed(runnables[runnables.Length - i - 1], i);
                    drawerLayout.PostDelayed(() => {
                        drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                    }, NavigationDrawerFragment.ANIMATION_DURATION);
                }
            });
            drawerToggle = navigationDrawerFragment.DrawerToggle.GetSlider();

            contentLayout = FindViewById<FrameLayout>(Resource.Id.content_layout);
            contentLayout.Post(() => {
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_layout, firstFragment).Commit();
                currentFragment = typeof(FirstFragment).Name;
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            });

            for (int i = 0; i < runnables.Length; i++) {
                float position = i / (float)NavigationDrawerFragment.ANIMATION_DURATION;
                runnables[i] = new Runnable(() => {
                    drawerToggle.SetPosition(position);
                });
            }
        }

        [Java.Interop.Export("onButtonClicked")]
        public void onButtonClicked(View v) {
            switch (v.Id) {
                case Resource.Id.go_to_second_fragment_btn:
                    navigationDrawerFragment.DrawerToggle.ToolbarNavigationClickListener.OnClick(contentLayout);
                    break;
            }
        }

        public override void OnBackPressed() {
            if (navigationDrawerFragment.DrawerToggle.DrawerOpened) {
                drawerLayout.CloseDrawers();
            } else if (currentFragment == typeof(SecondFragment).Name) {
                navigationDrawerFragment.DrawerToggle.ToolbarNavigationClickListener.OnClick(contentLayout);
            } else {
                base.OnBackPressed();
            }
        }
    }
}

