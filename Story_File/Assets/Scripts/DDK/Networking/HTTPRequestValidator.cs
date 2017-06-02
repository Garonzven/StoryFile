using System;
using UnityEngine;


#if USE_BEST_HTTP
using BestHTTP;
#endif


namespace DDK.Networking.ApiClient
{
    public static class HttpRequestValidator
    {
#if USE_BEST_HTTP
        public static Action onRequestError;
        public static Action onRequestConnectionTimeOut;
        public static Action onRequestTimeOut;
        /*public static delegate void onRequestError();
        public static delegate void onRequestConnectionTimeOut();
        public static delegate void onRequestTimeOut();*/


		public static bool Validate(HTTPRequest request, HTTPResponse response)
		{
			switch (request.State)
			{
			case HTTPRequestStates.Finished:
				if (response.IsSuccess) {
					Debug.Log("Request Finished Successfully! " + response.DataAsText);
					return true;
				}
				
				Debug.LogWarning(
					string.Format(
					"Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
					response.StatusCode,
					response.Message,
					response.DataAsText
					)
					);
				
				break;
				
			case HTTPRequestStates.Error:
				Debug.LogWarning("Request Finished with Error! " + (request.Exception != null ? (request.Exception.Message + "\n" + request.Exception.StackTrace) : "No Exception"));
				if (onRequestError != null)
					onRequestError();
				break;
				
			case HTTPRequestStates.ConnectionTimedOut:
				Debug.LogWarning("Connection Timed Out!");
				if (onRequestConnectionTimeOut != null)
                	onRequestConnectionTimeOut();
				break;
				
			case HTTPRequestStates.TimedOut:
				Debug.LogWarning("Processing the request Timed Out!");
				if (onRequestTimeOut != null)
                	onRequestTimeOut();
				break;
			case HTTPRequestStates.Initial:
				break;
			case HTTPRequestStates.Queued:
				break;
			case HTTPRequestStates.Processing:
				break;
			case HTTPRequestStates.Aborted:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			
			return false;
		}
#endif
    }
}