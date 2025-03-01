using Demo_Common.Entity;
using Demo_Common.Service;
using Demo_WPF.Helper;
using Demo_WPF.Model;
using System.Collections.ObjectModel;
using System.Windows;

namespace Demo_WPF.ViewModel
{
    public class DebugViewModel : ViewModelBase
    {
        private DebugInfo info = new();
        private ObservableCollection<DebugItem> items = [];
        /// <summary>
        /// Debug infor
        /// </summary>
        public DebugInfo Info { get  => info; set { info = value; NotifyPropertyChanged(nameof(Info)); } }
        /// <summary>
        /// Items
        /// </summary>
        public ObservableCollection<DebugItem> Items { get => items; set { items = value; NotifyPropertyChanged(nameof(Items)); } }

        /// <summary>
        /// Command clear
        /// </summary>
        public MyCommand CmdClear { get; set; }

        /// <summary>
        /// Command copy
        /// </summary>
        public MyCommand CmdCopy { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DebugViewModel()
        {
            CmdClear = new MyCommand(DoClear, CanClear);
            CmdCopy = new MyCommand(DoCopy, CanCopy);
        }

        /// <summary>
        /// Set infor
        /// </summary>
        /// <param name="debug">Debug infor</param>
        public void SetInfo(DebugInfo debug)
        {
            Info = debug;
            SendService.Instance.DebugHandler += Instance_DebugItemHandler;
        }

        /// <summary>
        /// Debug item handler
        /// </summary>
        /// <param name="item">Debug item</param>
        private void Instance_DebugItemHandler(bool request, DebugItem item)
        {
            if (request == Info.Request)
            {
                Application.Current.Dispatcher.Invoke(() => { Items.Insert(0, item); });
            }
        }

        /// <summary>
        /// Can clear
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CanClear(object parameter) => true;

        /// <summary>
        /// Can clear
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool CanCopy(object parameter) => true;

        /// <summary>
        /// Do clear
        /// </summary>
        /// <param name="parameter"></param>
        private void DoClear(object parameter)
        {
            Items.Clear();
        }

        /// <summary>
        /// Do copy
        /// </summary>
        /// <param name="parameter"></param>
        private void DoCopy(object parameter)
        {
            if (Items.Count == 0) return;
            if (Info.Index == -1)
            {
                MsgHelper.Infor("Please select an item to copy");
                return;
            }
            if (Items.Count <= Info.Index)
            {
                return;
            }
            Clipboard.SetDataObject(Items[Info.Index].Data);
        }
    }
}
