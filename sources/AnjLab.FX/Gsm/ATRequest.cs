using System;

namespace AnjLab.FX.Gsm
{
    public class ATRequest
    {
        readonly static string _standartErrorIndicator = "ERROR:";
        readonly static object[][] _standartResponseEndFlags = new object[][]
            {
                new object[] { "OK", ATComandResult.Ok }, 
                new object[] { "ERROR", ATComandResult.Error },
            };

        private readonly string _request;
        private readonly string _expectedSpecialResponseEndFlags;

        public ATRequest(string request)
        {
            _request = request;
        }

        public ATRequest(string request, string expectedResponseEndFlag)
        {
            _request = request;
            _expectedSpecialResponseEndFlags = expectedResponseEndFlag;
        }

        public ATResponse ParseResponse(string response)
        {
            string trimed = response.Trim();
            foreach (object[] flag in _standartResponseEndFlags)
                if (trimed.EndsWith(flag[0] as string, StringComparison.InvariantCultureIgnoreCase))
                    return Response(response, (ATComandResult)flag[1]);

            if (!String.IsNullOrEmpty(_expectedSpecialResponseEndFlags) && 
                response.EndsWith(_expectedSpecialResponseEndFlags, StringComparison.InvariantCultureIgnoreCase))
                return Response(response, ATComandResult.Special);

            if (response.IndexOf(_standartErrorIndicator) != -1)
                return Response(response, ATComandResult.Error);

            return null;
        }

        private ATResponse Response(string response, ATComandResult result)
        {
            if (response.Length > _request.Length)
                response = response.Remove(0, _request.Length).Trim();
            return new ATResponse(response, result);
        }

        public string Request
        {
            get { return _request; }
        }
    }
}
