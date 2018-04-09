using System;
using System.ServiceModel;
//using MercuryClassLibrary.EnterpriseService;

namespace MercuryClassLibrary
{
    public static class LastError
    {
        public static ResultInfo resultInfo = new ResultInfo();

        public static bool Success() => resultInfo.Success;
        public static ResultInfo GetError() => resultInfo;

        public static void SetError(string message) => resultInfo = new ResultInfo(false, message);

        internal static void SetError(FaultException<EnterpriseService.FaultInfo> e)
        {
            string err = Common.ServiceModelExceptionToString(e);

            string message = e.Detail.message;

            if (!string.IsNullOrEmpty(err))
                message += Environment.NewLine + err;

            resultInfo = new ResultInfo(false, message);
        }

        internal static void SetError(FaultException<ApplicationManagementService.FaultInfo> e)
        {
            string err = Common.ServiceModelExceptionToString(e);

            string message = e.Detail.message;

            if (!string.IsNullOrEmpty(err))
                message += Environment.NewLine + err;

            resultInfo = new ResultInfo(false, message);
        }

    }

    public class ResultInfo
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; }

        public ResultInfo()
        {
            Success = true;
            Message = string.Empty;
        }

        public ResultInfo(bool success)
        {
            Success = success;
            Message = string.Empty;
        }

        public ResultInfo(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
