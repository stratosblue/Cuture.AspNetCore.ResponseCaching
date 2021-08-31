using System.IO;

namespace Cuture.AspNetCore.ResponseCaching.Internal
{
    internal class ResponseDumpContext
    {
        #region Public 字段

        /// <summary>
        /// DumpStream
        /// </summary>
        public MemoryStream DumpStream { get; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 原始流
        /// </summary>
        public Stream OriginalStream { get; }

        #endregion Public 字段

        #region Public 构造函数

        public ResponseDumpContext(string key, MemoryStream dumpStream, Stream originalStream)
        {
            Key = key;
            DumpStream = dumpStream;
            OriginalStream = originalStream;
        }

        #endregion Public 构造函数
    }
}