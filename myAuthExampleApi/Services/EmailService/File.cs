using System.Collections.Generic;

namespace Services.EmailServ
{
    public class File
    {
        private byte[] _FileData;

        public string FileName { get; set; }
        public IReadOnlyCollection<byte> FileData
        {
            get
            {
                return _FileData;
            }
            set
            {
                _FileData = value as byte[];
            }
        }
    }
}
