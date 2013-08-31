using System;
using System.Collections.Generic;
using WPAppStudio.Entities;
using WPAppStudio.Entities.Base;
using WPAppStudio.Controls;

namespace WPAppStudio.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<TDestinationViewModel>();
        void NavigateTo<TDestinationViewModel>(object navigationContext);
        void NavigateBack();
        void NavigateBack(object navigationContext);
		void NavigateTo(MenuItemData menuItem);
		void NavigateTo(Uri uri);
        void ClearNavigationHistory();
        string GetUri(Type viewModelType);
    }
}
