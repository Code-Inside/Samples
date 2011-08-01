using System;
using System.ServiceModel;

namespace BlogPosts.Buzzwords.ServiceContracts
{
    [ServiceContract(Namespace = "http://BlogPosts.Buzzwords.ServiceContracts/2007/10", Name = "IBuzzwordService", SessionMode = SessionMode.Allowed)]
    public interface IBuzzwordService
    {
        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = true, AsyncPattern = false, Action = "Insert")]
        void Insert(BlogPosts.Buzzwords.DataContracts.Buzzword request);
        [OperationContract(IsTerminating = false, IsInitiating = true, IsOneWay = false, AsyncPattern = false, Action = "Load")]
        BlogPosts.Buzzwords.DataContracts.Buzzword Load(System.Int32 request);
    }
}
