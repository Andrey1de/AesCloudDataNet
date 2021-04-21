using System;

namespace AesCloudDataNet.Exceptions
{
    public class StoreException : Exception
    {
        public enum EType{
            Other,
            Get,
            List,
            Insert,
            Update,
            Internal,
            Delete,
 
        }
        public readonly EType Type;

        static string getMessage(EType type, string message) {

            return (type == EType.Other) ? message : 
                    type.ToString() + "=>" + message;

        }
        public StoreException(EType type,string message = "")
            : base(getMessage(type,message))
        {
            Type = type;
        }
        public StoreException(EType type,string message, Exception innerException)
            : base(getMessage(type, message) + ":" + innerException.Message, innerException)
        {
            Type = type;
        }
        
    }
}
