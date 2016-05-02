using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.Graphics.Drawable;
using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace HamburgerAnimation_Android {
    public class ActionBarDrawerToggle : Java.Lang.Object, DrawerLayout.IDrawerListener {

        private readonly IDelegate mActivityImpl;
        private readonly DrawerLayout mDrawerLayout;

        private IDrawerToggle mSlider;
        private Drawable mHomeAsUpIndicator;
        private bool mDrawerIndicatorEnabled = true;
        private bool mHasCustomUpIndicator;
        private readonly int mOpenDrawerContentDescRes;
        private readonly int mCloseDrawerContentDescRes;
        private View.IOnClickListener mToolbarNavigationClickListener;
        private bool mWarnedForDisplayHomeAsUp = false;

        public ActionBarDrawerToggle(Activity activity, DrawerLayout drawerLayout, Toolbar toolbar, int openDrawerContentDescRes, int closeDrawerContentDescRes) {
            mActivityImpl = new ToolbarCompatDelegate(toolbar);
            toolbar.SetNavigationOnClickListener(new LambdaActionListener(view => {
                if (mDrawerIndicatorEnabled) {
                    Toggle();
                } else if (mToolbarNavigationClickListener != null) {
                    mToolbarNavigationClickListener.OnClick(view);
                }
            }));
            mDrawerLayout = drawerLayout;
            mOpenDrawerContentDescRes = openDrawerContentDescRes;
            mCloseDrawerContentDescRes = closeDrawerContentDescRes;
            mSlider = new DrawerArrowDrawableToggle(activity, mActivityImpl.GetActionBarThemedContext());
            mHomeAsUpIndicator = GetThemeUpIndicator();
        }

        public class LambdaActionListener : Java.Lang.Object, View.IOnClickListener {

            private Action<View> lambda;

            public LambdaActionListener(Action<View> lambda) {
                this.lambda = lambda;
            }

            public void OnClick(View v) {
                lambda(v);
            }
        }

        public void SyncState() {
            if (mDrawerLayout.IsDrawerOpen(GravityCompat.Start)) {
                mSlider.SetPosition(1f);
            } else {
                mSlider.SetPosition(0f);
            }

            if (mDrawerIndicatorEnabled) {
                SetActionBarUpIndicator((Drawable)mSlider, mDrawerLayout.IsDrawerOpen(GravityCompat.Start) ? mCloseDrawerContentDescRes : mOpenDrawerContentDescRes);
            }
        }

        public void OnConfigurationChanged(Configuration newConfig) {
            if (!mHasCustomUpIndicator) {
                mHomeAsUpIndicator = GetThemeUpIndicator();
            }
            SyncState();
        }

        public bool OnOptionsItemSelected(IMenuItem item) {
            if (item != null && item.ItemId == Android.Resource.Id.Home && mDrawerIndicatorEnabled) {
                Toggle();
                return true;
            }
            return false;
        }

        private void Toggle() {
            int drawerLockMode = mDrawerLayout.GetDrawerLockMode(GravityCompat.Start);
            if (mDrawerLayout.IsDrawerVisible(GravityCompat.Start) && (drawerLockMode != DrawerLayout.LockModeLockedOpen)) {
                mDrawerLayout.CloseDrawer(GravityCompat.Start);
            } else if (drawerLockMode != DrawerLayout.LockModeLockedClosed) {
                mDrawerLayout.OpenDrawer(GravityCompat.Start);
            }
        }

        public void SetHomeAsUpIndicator(Drawable indicator) {
            if (indicator == null) {
                mHomeAsUpIndicator = GetThemeUpIndicator();
                mHasCustomUpIndicator = false;
            } else {
                mHomeAsUpIndicator = indicator;
                mHasCustomUpIndicator = true;
            }

            if (!mDrawerIndicatorEnabled) {
                SetActionBarUpIndicator(mHomeAsUpIndicator, 0);
            }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public void SetHomeAsUpIndicator(int resId) {
            Drawable indicator = null;
            if (resId != 0) {
                indicator = mDrawerLayout.Resources.GetDrawable(resId);
            }
            SetHomeAsUpIndicator(indicator);
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public bool DrawerIndicatorEnabled
        {
            get
            {
                return mDrawerIndicatorEnabled;
            }
            set
            {
                if (value != mDrawerIndicatorEnabled) {
                    if (value) {
                        SetActionBarUpIndicator((Drawable)mSlider, mDrawerLayout.IsDrawerOpen(GravityCompat.Start) ? mCloseDrawerContentDescRes : mOpenDrawerContentDescRes);
                    } else {
                        SetActionBarUpIndicator(mHomeAsUpIndicator, 0);
                    }
                    mDrawerIndicatorEnabled = value;
                }
            }
        }

        public IDrawerToggle GetSlider() {
            return mSlider;
        }

        public virtual void OnDrawerClosed(View drawerView) {
            mSlider.SetPosition(0f);
            if (mDrawerIndicatorEnabled) {
                setActionBarDescription(mOpenDrawerContentDescRes);
            }
        }

        public virtual void OnDrawerOpened(View drawerView) {
            mSlider.SetPosition(1f);
            if (mDrawerIndicatorEnabled) {
                setActionBarDescription(mCloseDrawerContentDescRes);
            }
        }

        public virtual void OnDrawerSlide(View drawerView, float slideOffset) {
            mSlider.SetPosition(System.Math.Min(1f, System.Math.Max(0f, slideOffset)));
        }

        public virtual void OnDrawerStateChanged(int newState) {
        }

        public View.IOnClickListener ToolbarNavigationClickListener
        {
            get
            {
                return mToolbarNavigationClickListener;
            }

            set
            {
                mToolbarNavigationClickListener = value;
            }
        }

        void SetActionBarUpIndicator(Drawable upDrawable, int contentDescRes) {
            if (!mWarnedForDisplayHomeAsUp && !mActivityImpl.IsNavigationVisible()) {
                mWarnedForDisplayHomeAsUp = true;
            }
            mActivityImpl.SetActionBarUpIndicator(upDrawable, contentDescRes);
        }

        void setActionBarDescription(int contentDescRes) {
            mActivityImpl.SetActionBarDescription(contentDescRes);
        }

        Drawable GetThemeUpIndicator() {
            return mActivityImpl.GetThemeUpIndicator();
        }

        class DrawerArrowDrawableToggle : DrawerArrowDrawable, IDrawerToggle {

            private readonly Activity mActivity;

            public DrawerArrowDrawableToggle(Activity activity, Context themedContext) : base(themedContext) {
                mActivity = activity;
            }

            public float GetPosition() {
                return Progress;
            }

            public void SetPosition(float position) {
                if (GetPosition() == 1f) {
                    SetVerticalMirror(true);
                } else if (GetPosition() == 0f) {
                    SetVerticalMirror(false);
                }
                Progress = position;
            }
        }

        class ToolbarCompatDelegate : IDelegate {

            readonly Toolbar mToolbar;
            readonly Drawable mDefaultUpIndicator;
            readonly string mDefaultContentDescription;

            internal ToolbarCompatDelegate(Toolbar toolbar) {
                mToolbar = toolbar;
                mDefaultUpIndicator = toolbar.NavigationIcon;
                mDefaultContentDescription = toolbar.NavigationContentDescription;
            }

            public void SetActionBarUpIndicator(Drawable upDrawable, int contentDescRes) {
                mToolbar.NavigationIcon = upDrawable;
                SetActionBarDescription(contentDescRes);
            }

            public void SetActionBarDescription(int contentDescRes) {
                if (contentDescRes == 0) {
                    mToolbar.NavigationContentDescription = mDefaultContentDescription;
                } else {
                    mToolbar.SetNavigationContentDescription(contentDescRes);
                }
            }

            public Drawable GetThemeUpIndicator() {
                return mDefaultUpIndicator;
            }

            public Context GetActionBarThemedContext() {
                return mToolbar.Context;
            }

            public bool IsNavigationVisible() {
                return true;
            }
        }
    }

    public interface IDelegateProvider {
        IDelegate GetDrawerToggleDelegate();
    }

    public interface IDelegate {
        void SetActionBarUpIndicator(Drawable upDrawable, int contentDescRes);
        void SetActionBarDescription(int contentDescRes);
        Drawable GetThemeUpIndicator();
        Context GetActionBarThemedContext();
        bool IsNavigationVisible();
    }

    public interface IDrawerToggle {
        void SetPosition(float position);
        float GetPosition();
    }
}