using System;

namespace BlogPosts.Buzzwords.FaultContracts
{
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://BlogPosts.Buzzwords.FaultContracts/2007/10", Name = "DefaultFaultContract")]
    public class DefaultFaultContract
    {
        int errorId;
        string errorMessage;
        Guid correlationId;

        public DefaultFaultContract()
            : this(-1, String.Empty, Guid.Empty)
        {
        }

        public DefaultFaultContract(int errorId, string errorMessage, Guid correlationId)
        {
            this.errorId = errorId;
            this.errorMessage = errorMessage;
            this.correlationId = correlationId;
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Name = "ErrorId", Order = 1)]
        public int ErrorId
        {
            get { return errorId; }
            set { errorId = value; }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Name = "ErrorMessage", Order = 2)]
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true, Name = "CorrelationId", Order = 3)]
        public Guid CorrelationId
        {
            get { return correlationId; }
            set { correlationId = value; }
        }
    }
}