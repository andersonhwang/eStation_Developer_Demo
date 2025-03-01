namespace Demo_Common.Entity
{
    /// <summary>
    /// Debug Item
    /// </summary>
    public class DebugItem
    {
        private DateTime _time = DateTime.Now;
        private string _id = string.Empty;
        private ushort _alias = 0;
        private string _topic = string.Empty;
        private string _data = string.Empty;

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get => _time; }
        /// <summary>
        /// Client ID
        /// </summary>
        public string Id { get => _id; }
        /// <summary>
        /// Topic alias
        /// </summary>
        public ushort Alias { get => _alias; }
        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get => _topic; }
        /// <summary>
        /// Data
        /// </summary>
        public string Data { get => _data; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <param name="alias">Topic alias</param>
        /// <param name="topic">Topic</param>
        /// <param name="data">Data</param>
        public DebugItem(string id, ushort alias, string topic, string data)
        {
            _id = id;
            _alias = alias;
            _topic = topic;
            _data = data;
        }

        /// <summary>
        /// Constructor with ApData
        /// </summary>
        /// <param name="data">ApData</param>
        /// <param name="json">Json text</param>
        public DebugItem(ApData data, string json)
        {
            _id = data.Id;
            _topic = data.Topic;
            _alias = data.TopicAlias;
            _data = json;
        }
    }
}
