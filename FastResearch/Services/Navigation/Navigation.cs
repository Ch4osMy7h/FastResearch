using System;
using System.Threading.Tasks;

namespace BuildCast.Services.Navigation
{
    public interface INavigationService
    {
        event EventHandler<bool> IsNavigatingChanged;

        event EventHandler Navigated;

        bool CanGoBack { get; }

        bool IsNavigating { get; }

        Task NavigateToPapersPageAsync();

        Task NavigateToToolsPageAsync();

        Task GoBackAsync();
    }
}