using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace HamburgerAnimation_Android {
    public class FirstFragment : Fragment {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            return inflater.Inflate(Resource.Layout.Fragment_First, container, false);
        }
    }
}