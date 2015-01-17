using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MebiusLib
{
    class AutoScrollBehavior : Behavior<ListBox>
    {
        private ObservableCollection<string> oc;
        public AutoScrollBehavior() { }//do nothing
        protected override void OnAttached()
        {
            base.OnAttached();
            this.oc = (ObservableCollection<string>)this.AssociatedObject.ItemsSource;
            oc.CollectionChanged += this.scroll;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.oc.CollectionChanged -= this.scroll;
        }
        private void scroll(object sender, EventArgs e)
        {
            this.AssociatedObject.ScrollIntoView(this.oc[this.oc.Count - 1]);
        }
    }
}
