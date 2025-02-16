using System.Buffers;

namespace Demo_Common.Entity
{
    public struct ApData
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id;
        /// <summary>
        /// Topic
        /// </summary>
        public string Topic;
        /// <summary>
        /// Topic alias
        /// </summary>
        public ushort TopicAlias;
        /// <summary>
        /// Data
        /// </summary>
        public ReadOnlySequence<byte> Data;
    }
}
