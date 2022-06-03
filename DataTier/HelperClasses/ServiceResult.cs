//
//  2018-08-13  Mark Stega
//              Created
//
//  2019-06-24  Mark Stega
//              Changed property 'sets' from private to public as Json.NET will otherwise not deserialize
//              properly.
//
//  2019-09-27  Mark Stega
//              Moved from Newtonsoft.Json to System.Text.Json
//

namespace ICEBG.DataTier.HelperClasses;

public class ServiceResult<ServiceResultType>
{
    public bool pSuccess { get; set; }

    public string pError { get; set; }

    public ServiceResultType pResult { get; set; }

    // public only to support System.Text.Json serialization on WASM
    public ServiceResult() { }

    public ServiceResult(
        ServiceResultType result,
        bool success = true,
        string error = "")
    {
        pSuccess = success;
        pError = error;
        pResult = result;
    }

}

