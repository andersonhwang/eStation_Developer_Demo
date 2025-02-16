using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo_Common.Entity
{
    public class DebugInfo
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="alias">Topic alias</param>
    /// <param name="topic">Topic</param>
    /// <param name="data">Data</param>
    public class DebugItem(ushort alias, string topic, string data)
    {
        private DateTime _time = DateTime.Now;
        private ushort _id = alias;
        private string _topic = topic;
        private string _data = data;

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get => _time; }
        /// <summary>
        /// Name
        /// </summary>
        public ushort Id { get => _id; }
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
