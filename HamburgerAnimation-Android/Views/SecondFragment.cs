using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace HamburgerAnimation_Android {
    public class SecondFragment : Fragment {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            return inflater.Inflate(Resource.Layout.Fragment_Second, container, false);
        }
    }
}