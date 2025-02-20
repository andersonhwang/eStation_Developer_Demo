using Demo_Common.Enum;
using Demo_Common.Service;
using Demo_WPF.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    internal class ApListViewModel : ViewModelBase
    {
        private object _locker = new();
        private ApModel? selectAP = null;
        private ObservableCollection<ApModel> aps = [];
        /// <summary>
        /// Select AP
        /// </summary>
        public ApModel? SelectAP
        {
            get => selectAP;
            set
            {
                selectAP = value; NotifyPropertyChanged(nameof(SelectAP));
                if (value != null) SendService.Instance.SelectAp(value.IDM);
            }
        }
        /// <summary>
        /// AP list
        /// </summary>
        public ObservableCollection<ApModel> APs { get => aps; set { aps = value; NotifyPropertyChanged(nameof(APs)); } }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApListViewModel()
        {
            SendService.Instance.Register(APsStatusHandler);
        }

        /// <summary>
        /// AP status handler
        /// </summary>
        /// <param name="id">AP ID</param>
        /// <param name="ip">AP IP</param>
        /// <param name="status">AP status</param>
        private void APsStatusHandler(string id, string ip, ApStatus status)
        {
            lock (_locker)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var ap = APs.FirstOrDefault(x => x.IDM == id);
                    if (ap == null)
                    {
                        ap = new ApModel(id, ip, status);
                        APs.Add(ap);
                    }
                    else
                    {
                        ap.StatusM = status;
                    }
                });
            }
        }
    }
}
