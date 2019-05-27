using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace MagicMirror
{
    public class ItemToShow : ObservableCollection<string>, ISupportIncrementalLoading
    {
        public int lastItem = 1;
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            ProgressBar progressBar = ((Window.Current.Content as Frame).Content as MainPage).ProgressBar;
            CoreDispatcher coreDispatcher = Window.Current.Dispatcher;

            return Task.Run<LoadMoreItemsResult>(async () =>
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    progressBar.IsIndeterminate = true;
                    progressBar.Visibility = Visibility.Visible;
                });

                List<string> items = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    items.Add(string.Format("Item {0}", lastItem));
                    lastItem++;
                    if (lastItem == 10000)
                        break;
                }

                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (string item in items)
                        this.Add(item);

                    progressBar.Visibility = Visibility.Collapsed;
                    progressBar.IsIndeterminate = false;
                });

                return new LoadMoreItemsResult() { Count = count };
            }).AsAsyncOperation<LoadMoreItemsResult>();
        }

        public bool HasMoreItems
        {
            get
            {
                if (lastItem == 10000)
                    return false;
                else
                    return true;
            }
        }
    }
}
