using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace HamburgerAnimation_Android {
    public class NavigationDrawerFragment : Fragment {

        public MyActionBarDrawerToggle DrawerToggle { get; set; }

        public const int ANIMATION_DURATION = 250;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            return inflater.Inflate(Resource.Layout.Fragment_Navigation_Drawer, container, false);
        }

        public void Setup(DrawerLayout drawerLayout, Toolbar toolbar) {
            DrawerToggle = new MyActionBarDrawerToggle(Activity, drawerLayout, toolbar, Resource.String.navigation_drawer_open_string, Resource.String.navigation_drawer_close_string);
            drawerLayout.AddDrawerListener(DrawerToggle);
            drawerLayout.Post(() => {
                DrawerToggle.SyncState();
            });
        }

        public class MyActionBarDrawerToggle : ActionBarDrawerToggle {

            private Android.App.Activity activity;
            public bool DrawerOpened { get; set; }

            public MyActionBarDrawerToggle(Android.App.Activity activity, DrawerLayout drawerLayout, Toolbar toolbar, int openDrawerContentDescRes, int closeDrawerContentDescRes) : base(activity, drawerLayout, toolbar, openDrawerContentDescRes, closeDrawerContentDescRes) {
                this.activity = activity;
                DrawerOpened = false;
            }

            public override void OnDrawerOpened(View drawerView) {
                base.OnDrawerOpened(drawerView);
                DrawerOpened = true;
                activity.InvalidateOptionsMenu();
            }

            public override void OnDrawerClosed(View drawerView) {
                base.OnDrawerClosed(drawerView);
                DrawerOpened = false;
                activity.InvalidateOptionsMenu();
            }
        }
    }
}