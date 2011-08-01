using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using System.Collections.Generic;


[ServiceContract(Namespace = "ServiceNamespace")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Service
{
    // Add [WebGet] attribute to use HTTP GET
    [OperationContract]
    public DateTime GetDateTime()
    {
        return DateTime.Now;
    }

    [OperationContract]
    public ComplexType GetComplexOne()
    {
        return ComplexType.GetOne();
    }


    [OperationContract]
    public List<ComplexType> GetComplexList()
    {
        return ComplexType.GetList();
    }
    // Add more operations here and mark them with [OperationContract]
}
