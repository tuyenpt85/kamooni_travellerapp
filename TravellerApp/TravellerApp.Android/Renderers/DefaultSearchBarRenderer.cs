﻿using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using TravellerApp.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SearchBar), typeof(DefaultSearchBarRenderer))]

namespace TravellerApp.Droid.Renderers
{
    public class DefaultSearchBarRenderer : SearchBarRenderer
    {
        public DefaultSearchBarRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
                // WorkAround to searchBar not appearing in newer android versions
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    Element.HeightRequest = 40;
            }

            if (Control != null && Element != null)
            {
                var listener = new OnQueryTextListener(Element);
                Control.SetOnQueryTextListener(listener);

                EditText searchPlate = Control.FindViewById<EditText>(base.Resources.GetIdentifier("android:id/search_src_text", null, null));
                searchPlate.SetOnEditorActionListener(new OnEditorActionListener(listener));
            }
        }

        public class OnEditorActionListener : Java.Lang.Object, TextView.IOnEditorActionListener
        {
            private readonly OnQueryTextListener listener;

            public OnEditorActionListener(OnQueryTextListener listener)
            {
                this.listener = listener;
            }

            public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
            {
                if (actionId == ImeAction.Search)
                {
                    listener?.OnQueryTextSubmit(v.Text);
                }
                return true;
            }
        }

        public class OnQueryTextListener : Java.Lang.Object, SearchView.IOnQueryTextListener
        {
            private SearchBar searchBar;

            public OnQueryTextListener(SearchBar searchBar)
            {
                this.searchBar = searchBar;
            }

            public bool OnQueryTextChange(string newText)
            {
                searchBar.Text = newText;
                return true;
            }

            public bool OnQueryTextSubmit(string query)
            {
                if (searchBar.SearchCommandParameter != null)
                    searchBar.SearchCommand?.Execute(searchBar.SearchCommandParameter);
                else
                    searchBar.SearchCommand?.Execute(query);
                searchBar.Unfocus();
                return true;
            }
        }
    }
}