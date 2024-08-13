using System.Collections.Generic;
using System.Collections.ObjectModel;
using eStationDemo.Model;
using eStationDemo.Service;
using ReactiveUI;

namespace eStationDemo.ViewModel
{
    public class PublishESLTaskViewModel : ViewModelBase
    {
        string ClientID = string.Empty;
        ObservableCollection<ESLTask> tasks = new();
        public ObservableCollection<ESLTask> Tasks { get => tasks; set => this.RaiseAndSetIfChanged(ref tasks, value); }
        /// <summary>
        /// Set client ID
        /// </summary>
        /// <param name="clientID">Client ID</param>
        public void SetClientID(string clientID)
        {
            ClientID = clientID;
        }

        /// <summary>
        /// Publish esl tasks
        /// </summary>
        public void PublishESLTasks()
        {
            var list = new List<ESLEntity>();
            MQTTService.Instance.PublishMessage<List<ESLEntity>>(ClientID, string.Empty, list).Wait();
        }
    }
}