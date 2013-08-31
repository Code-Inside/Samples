using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;

namespace WPAppStudio.Controls
{
    public class AppBarMenu : Control
    {
        private readonly AppBarMenuAction[] _theActions;
        protected StackPanel TheStackPanel;

        public AppBarMenu(params AppBarMenuAction[] actions)
        {
            DefaultStyleKey = typeof (AppBarMenu);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Bottom;
            _theActions = actions;
        }

        /// <summary>
        ///     Gets the frame.
        /// </summary>
        /// <value>The frame.</value>
        private static PhoneApplicationFrame Frame
        {
            get { return Application.Current.RootVisual as PhoneApplicationFrame; }
        }

        /// <summary>
        ///     Gets the page.
        /// </summary>
        /// <value>The page.</value>
        private static PhoneApplicationPage Page
        {
            get { return Frame.Content as PhoneApplicationPage; }
        }

        /// <summary>
        ///     Checks for application bar.
        /// </summary>
        /// <returns></returns>
        private bool CheckForApplicationBar()
        {
            if (Page.ApplicationBar != null)
            {
                Background = CheckAppBarBackgroundColour(Page.ApplicationBar.BackgroundColor);
                Foreground = CheckAppBarForegroundColour(Page.ApplicationBar.ForegroundColor);
                return Page.ApplicationBar.IsVisible;
            }
            return false;
        }

        /// <summary>
        ///     Checks the app bar foreground colour.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Brush CheckAppBarForegroundColour(Color color)
        {
            if (color.ToString().Equals("#00000000")) // Default system theme no colour is given
            {
                bool isDark = ((Visibility) Application.Current.Resources["PhoneDarkThemeVisibility"] ==
                               Visibility.Visible);
                color = isDark ? Colors.White : Colors.Black;
            }
            return new SolidColorBrush(color);
        }

        /// <summary>
        ///     Checks the app bar background colour.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private Brush CheckAppBarBackgroundColour(Color color)
        {
            if (color.ToString().Equals("#00000000")) // Default system theme no colour is given
            {
                bool isDark = ((Visibility) Application.Current.Resources["PhoneDarkThemeVisibility"] ==
                               Visibility.Visible);
                color = isDark ? Color.FromArgb(255, 33, 32, 33) : Color.FromArgb(255, 223, 223, 223);
            }
            return new SolidColorBrush(color);
        }

        /// <summary>
        ///     Called when [apply template].
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Get the stackpanel from the template so we can populate its contents with the 
            // provided actions.
            TheStackPanel = GetTemplateChild("TheStackPanel") as StackPanel;
            foreach (AppBarMenuAction action in _theActions)
            {
                action.Parent = this;
                var menuItem = new AppBarMenuItem
                    {
                        Content = action.Content,
                        Command = action.Command,
                        Foreground = Foreground
                    };

                if (TheStackPanel != null) 
                    TheStackPanel.Children.Add(menuItem);
            }
        }
    }
}