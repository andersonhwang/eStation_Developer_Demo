using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Security;
using eStationDemo.Model;
using eStationDemo.Service;
using ReactiveUI;

namespace eStationDemo.ViewModel
{
    public class MainViewModel : ReactiveObject
    {
        private ConnConfig config = new();
        /// <summary>
        /// Connection configuration
        /// </summary>
        public ConnConfig Config { get => config; set => this.RaiseAndSetIfChanged(ref config, value); }
        /// <summary>
        /// Clients list
        /// </summary>
        public ObservableCollection<ClientInfor> Clients { get; set; } = [];
        /// <summary>
        /// Run MQTT service
        /// </summary>
        public void Run()
        {
            MQTTService.Instance.Init(config, ApInforHandler, ApMessageHandler, TaskResultHandler).Wait();
        }

        public void ApInforHandler(eStationInfor infor)
        {

        }

        public void ApMessageHandler(eStationMessage message)
        {

        }

        public void TaskResultHandler(TaskResult result)
        {

        }
    }
}
