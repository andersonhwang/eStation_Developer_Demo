using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_WPF.Model
{
    internal class DebugInfo
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="topic">Topic</param>
    /// <param name="data">Data</param>
    internal class DebugItem(string id, string topic, string data)
    {
        private DateTime _time = DateTime.Now;
        private string _topic = topic;
        private string _id = id;
        private string _data = data;

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get => _time; }
        /// <summary>
        /// Name
        /// </summary>
        public string Id { get => _id; }
        /// <summary>
        /// Code
        /// </summary>
        public string Topic { get => _topic; }
        /// <summary>
        /// Data
        /// </summary>
        public string Data { get => _data; }
    }
}
