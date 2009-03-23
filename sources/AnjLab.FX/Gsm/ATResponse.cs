using System;

namespace AnjLab.FX.Gsm
{
    public class ATResponse
    {
        private readonly ATComandResult _result = ATComandResult.Timeout;
        private readonly string _response = "";

        public static ATResponse Timeout
        {
            get { return new ATResponse("", ATComandResult.Timeout); }
        }

        public ATResponse(string response, ATComandResult result)
        {
            _result = result;
            _response = response;
        }

        public ATComandResult Result
        {
            get { return _result; }
        }

        public string Response
        {
            get { return _response; }
        }

        public bool IsOk { get { return Result == ATComandResult.Ok; } }
        public bool IsError { get { return Result == ATComandResult.Error; } }
        public bool IsTimeout { get { return Result == ATComandResult.Timeout; } }
        public bool IsSpecial { get { return Result == ATComandResult.Special; } }
    }

    public enum ATComandResult
    {
        Ok = 0,
        Error = 1,
        Timeout = 2,
        Special = 4
    }
}
