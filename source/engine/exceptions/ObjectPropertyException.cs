using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cevEngine2D.source.engine.exceptions {
    internal class ObjectPropertyException : Exception {

        public object CheckObject { get; }
        public ObjectPropertyException(string message, object checkObject) : base(message) {
             CheckObject = checkObject;
        }
    }
}
