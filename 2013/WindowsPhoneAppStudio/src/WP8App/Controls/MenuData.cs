using System.Collections.ObjectModel;

namespace WPAppStudio.Controls
{
    public class MenuData : ObservableCollection<MenuItemData>
    {

    }

    public class MenuItemData
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Target { get; set; }
        public string TargetUrl { get; set; }
        public string Image { get; set; }
    }
}
