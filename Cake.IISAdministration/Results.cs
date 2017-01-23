using System.Collections.Generic;

namespace Cake.IISAdministration
{

    public class Results<T>
    {
        public bool Success { set; get; }
        public string Message { set; get; }
        public IEnumerable<T> Data { set; get; }
    }
}