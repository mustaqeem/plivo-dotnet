using System;
using System.Web;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Deserializers;
using dict = System.Collections.Generic.Dictionary<string, string>;

namespace Plivo.API
{
	public class PlivoException : Exception
	{
		public PlivoException(string message) : base(message) { }
	}

	public class RestAPI
	{
		private const string PlivoUrl = "https://api.plivo.com";
		public string PlivoVersion { get; set; }
		public string AuthID { get; set; }
		public string AuthToken { get; set; }
		private RestClient client;

		public RestAPI(string auth_id, string auth_token, string version = "v1")
		{
			PlivoVersion = version;
			AuthID = auth_id;
			AuthToken = auth_token;
			// Initialize the client
			client = new RestClient();
			client.Authenticator = new HttpBasicAuthenticator(AuthID, AuthToken);
			client.UserAgent = "PlivoCsharp";
			client.BaseUrl = String.Format("{0}/{1}/Account/{2}", PlivoUrl, PlivoVersion, AuthID);
		}

		private IRestResponse<T> _request<T>(string http_method, string resource, dict data)
			where T : new()
		{
			var request = new RestRequest() 
			{ Resource = resource, RequestFormat = DataFormat.Json };

			// add the parameters to the request
			foreach (KeyValuePair<string, string> kvp in data)
				request.AddParameter(kvp.Key, HttpUtility.HtmlEncode(kvp.Value));

			//set the HTTP method for this request
			switch (http_method.ToUpper())
			{
				case "GET": request.Method = Method.GET;
				break;
				case "POST": request.Method = Method.POST;
				request.Parameters.Clear();
				request.AddParameter("application/json", request.JsonSerializer.Serialize(data), ParameterType.RequestBody);
				break;
				case "DELETE": request.Method = Method.DELETE;
				break;
				default: request.Method = Method.GET;
				break;
			};

			client.AddHandler("application/json", new JsonDeserializer());
			IRestResponse<T> response = client.Execute<T>(request);
			return response;
		}

		// Accounts //
		public IRestResponse<Account> GetAccount(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Account>("GET", "/", new dict());
		}

		public IRestResponse<GenericResponse> EditAccount(string name="", string city="", string address="",
		                                                  string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(name))
				parameters.Add("name", name);
			if (!String.IsNullOrEmpty(city))
				parameters.Add("city", city);
			if (!String.IsNullOrEmpty(address))
				parameters.Add("address", address);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", "/", parameters);
		}

		public IRestResponse<SubAccountList> GetSubAccounts(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<SubAccountList>("GET", "/Subaccount/", new dict());
		}

		public IRestResponse<SubAccount> GetSubAccount(string subAccountId, string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<SubAccount>("GET", String.Format("/Subaccount/{0}/", subAccountId), parameters);
		}

		public IRestResponse<GenericResponse> CreateSubAccount(string name, bool enabled = true, 
		                                                       string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(name))
				parameters.Add("name", name);
			parameters.Add("enabled", enabled.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", "/Subaccount/", parameters);
		}

		public IRestResponse<GenericResponse> EditSubAccount(string subAccountId, string name = "", bool enabled = true,
		                                                     string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(name))
				parameters.Add("name", name);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);
			parameters.Add("enabled", enabled.ToString());

			return _request<GenericResponse>("POST", String.Format("/Subaccount/{0}/ ", subAccountId), parameters);
		}

		public IRestResponse<GenericResponse> DeleteSubAccount(string subAccountId,
		                                                       string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Subaccount/{0}/", subAccountId), parameters);
		}

		// Applications //
		public IRestResponse<ApplicationList> GetApplications(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<ApplicationList>("GET", "/Application/", parameters);
		}

		public IRestResponse<ApplicationList> GetApplications(int limit = 0, int offset = 0,
		                                                      string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (limit > 0)
				parameters.Add("limit", limit.ToString());
			if (offset > 0)
				parameters.Add("offset", offset.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<ApplicationList>("GET", "/Application/", parameters);
		}

		public IRestResponse<Application> GetApplication(string appId,
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Application>("GET", String.Format("/Application/{0}/", appId), parameters);
		}

		public IRestResponse<GenericResponse> CreateApplication(string appName, string answerUrl,
		                                                        string answerMethod = "", string hangupUrl = "", string hangupMethod = "", string fallbackAnswerUrl = "",
		                                                        string fallbackMethod = "", string messageUrl = "", string messageMethod = "", bool defaultNumberApp = false, 
		                                                        bool defaultEndpointApp = false, string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("app_name", appName);
			parameters.Add("answer_url", answerUrl);
			if (!String.IsNullOrEmpty(answerMethod))
				parameters.Add("answer_method", answerMethod);
			if (!String.IsNullOrEmpty(hangupUrl))
				parameters.Add("hangup_url", hangupUrl);
			if (!String.IsNullOrEmpty(hangupMethod))
				parameters.Add("hangup_method", hangupMethod);
			if (!String.IsNullOrEmpty(fallbackAnswerUrl))
				parameters.Add("fallback_answer_url", fallbackAnswerUrl);
			if (!String.IsNullOrEmpty(fallbackMethod))
				parameters.Add("fallback_method", fallbackMethod);
			if (!String.IsNullOrEmpty(messageUrl))
				parameters.Add("message_url", messageUrl);
			if (!String.IsNullOrEmpty(messageMethod))
				parameters.Add("message_method", messageMethod);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);
			parameters.Add("defaul_number_app", defaultNumberApp.ToString());
			parameters.Add("defaul_endpoint_app", defaultEndpointApp.ToString());

			return _request<GenericResponse>("POST", "/Application/", parameters);
		}

		public IRestResponse<GenericResponse> EditApplication(string appId, string answerUrl = "", string answerMethod = "",
		                                                      string hangupUrl = "", string hangupMethod = "", string fallbackAnswerUrl = "", string fallbackMethod = "",
		                                                      string messageUrl = "", string messageMethod = "", bool defaultNumberApp = false, bool defaultEndpointApp = false,
		                                                      string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(answerMethod))
				parameters.Add("answer_method", answerMethod);
			if (!String.IsNullOrEmpty(hangupUrl))
				parameters.Add("hangup_url", hangupUrl);
			if (!String.IsNullOrEmpty(hangupMethod))
				parameters.Add("hangup_method", hangupMethod);
			if (!String.IsNullOrEmpty(fallbackAnswerUrl))
				parameters.Add("fallback_answer_url", fallbackAnswerUrl);
			if (!String.IsNullOrEmpty(fallbackMethod))
				parameters.Add("fallback_method", fallbackMethod);
			if (!String.IsNullOrEmpty(messageUrl))
				parameters.Add("message_url", messageUrl);
			if (!String.IsNullOrEmpty(messageMethod))
				parameters.Add("message_method", messageMethod);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);
			parameters.Add("defaul_number_app", defaultNumberApp.ToString());
			parameters.Add("defaul_endpoint_app", defaultEndpointApp.ToString());

			return _request<GenericResponse>("POST", String.Format("/Application/{0}/", appId), parameters);
		}

		public IRestResponse<GenericResponse> DeleteApplication(string appId,
		                                                        string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Application/{0}/", appId), new dict());
		}


		// Numbers //
		public IRestResponse<NumberList> GetNumbers(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<NumberList>("GET", "/Number/", parameters);
		}

		[Obsolete("Use search_number_group() instead")]
		public IRestResponse<NumberList> SearchNumbers(string countryCode = "1", string numberType = "local", string prefix = "",
		                                               string digits = "", string region = "", string services = "", int limit = 0, int offset = 0,
		                                               string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("country_code", countryCode);
			if (!String.Equals(numberType, "local"))
				parameters.Add("number_type", numberType);
			if (!String.IsNullOrEmpty(prefix))
				parameters.Add("prefix", prefix);
			if (!String.IsNullOrEmpty(digits))
				parameters.Add("digits", digits);
			if (!String.IsNullOrEmpty(region))
				parameters.Add("region", region);
			if (!String.IsNullOrEmpty(services))
				parameters.Add("services", services);
			if (limit > 0)
				parameters.Add("limit", limit.ToString());
			if (offset > 0)
				parameters.Add("offset", offset.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<NumberList>("GET", "/AvailableNumber/", parameters);
		}

		public IRestResponse<NumberList> SearchNumberGroup(string countryIso, string numberType = "local", string prefix = "",
		                                                   string region = "", string services = "", int limit = 0, int offset = 0,
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("country_iso", countryIso);
			if (!String.Equals(numberType, "local"))
				parameters.Add("number_type", numberType);
			if (!String.IsNullOrEmpty(prefix))
				parameters.Add("prefix", prefix);
			if (!String.IsNullOrEmpty(region))
				parameters.Add("region", region);
			if (!String.IsNullOrEmpty(services))
				parameters.Add("services", services);
			if (limit > 0)
				parameters.Add("limit", limit.ToString());
			if (offset > 0)
				parameters.Add("offset", offset.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<NumberList>("GET", "/AvailableNumberGroup/", parameters);
		}

		public IRestResponse<Number> GetNumber(string number, string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Number>("GET", String.Format("/Number/{0}/", number), parameters);
		}

		[Obsolete("Use rent_numbers() instead")]
		public IRestResponse<GenericResponse> RentNumber(string number, string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/AvailableNumber/{0}/", number), parameters);
		}

		public IRestResponse<NumberResponse> RentNumbers(string numberGroupId, 
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<NumberResponse>("POST", String.Format("/AvailableNumberGroup/{0}/", numberGroupId), parameters);
		}

		public IRestResponse<GenericResponse> UnrentNumber(string number,
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Number/{0}/", number), parameters);
		}

		public IRestResponse<GenericResponse> EditNumber(string number, string appId = "", string subAccount = "",
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(appId))
				parameters.Add("app_id", appId);
			if (!String.IsNullOrEmpty(subAccount))
				parameters.Add("subaccount", subAccount);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Number/{0}/", number), parameters);
		}

		// Calls //
		public IRestResponse<CDRList> GetCdrs(int limit = 0, int offset = 0,
		                                      string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (limit > 0)
				parameters.Add("limit", limit.ToString());
			if (offset > 0)
				parameters.Add("offset", offset.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<CDRList>("GET", "/Call/", parameters);
		}

		public IRestResponse<CDR> GetCdr(string callUuid, string subAccount = "",
		                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(subAccount))
				parameters.Add("subaccount", subAccount);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<CDR>("GET", String.Format("/Call/{0}/", callUuid), new dict());
		}

		public IRestResponse<LiveCallList> GetLiveCalls(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("status", "live");
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<LiveCallList>("GET", "/Call/", parameters);
		}

		public IRestResponse<LiveCall> GetLiveCall(string callUuid,
		                                           string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("status", "live");
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<LiveCall>("GET", String.Format("/Call/{0}/", callUuid), parameters);
		}

		/// <summary>
		/// To make an outbound call
		/// </summary>
		/// <param name="fromNumber">The phone number to be used as the caller id (with the country code).
		/// For e.g, a USA caller id number could be, 15677654321, with '1' for the country code. 
		/// Multiple numbers can be sent by using a delimiter. For e.g. 15677654321<12077657621<12047657621.</param>
		/// <param name="toNumber">The regular number(s) or sip endpoint(s) to call. Regular number 
		/// must be prefixed with country code but without the ‘+’ sign). For e.g, to dial a number 
		/// in the USA, the number could be, 15677654321, with '1' for the country code. Multiple numbers 
		/// can be sent by using a delimiter. For e.g. 15677654321<12077657621<12047657621. Sip endpoints 
		/// must be prefixed with 'sip:' E.g., sip:john1234@phone.plivo.com. To make bulk calls, 
		/// the delimiter '<' is used. For eg. 15677654321<15673464321<sip:john1234@phone.plivo.com 
		/// Yes, you can mix regular numbers and sip endpoints !</param>
		/// <param name="answerUrl">The URL invoked by Plivo when the outbound call is answered.</param>
		/// <param name="answerMethod">The method used to call the answer_url. Defaults to POST.</param>
		/// <param name="ringUrl">The URL that is notified by Plivo when the call is ringing. Defaults not set.</param>
		/// <param name="ringMethod">The method used to call the ring_url. Defaults to POST.</param>
		/// <param name="hangupUrl">The URL that is notified by Plivo when the call hangs up. Defaults to answer_url.</param>
		/// <param name="hangupMethod">The method used to call the hangup_url. Defaults to POST.</param>
		/// <param name="callerName">Caller name to use with the call.</param>
		/// <param name="fallbackAnswerUrl">Invoked by Plivo only if answer_url is unavailable or the XML response is 
		/// invalid. Should contain a XML response.</param>
		/// <param name="fallbackMethod">The method used to call the fallback_answer_url. Defaults to POST.</param>
		/// <param name="sendDigits">Plivo plays DTMF tones when the call is answered. This is useful when dialing a 
		/// phone number and an extension. Plivo will dial the number, and when the automated system picks up, sends 
		/// the DTMF tones to connect to the extension. E.g. If you want to dial the 2410 extension after the call is 
		/// connected, and you want to wait for a few seconds before sending the extension, add a few leading 'w' characters. 
		/// Each 'w' character waits 0.5 second before sending a digit.
		/// Each 'W' character waits 1 second before sending a digit.
		/// You can also add the tone duration in ms by appending @duration after the string (default duration is 2000 ms).
		/// Eg. 1w2w3@1000 See the DTMF API for additional information.</param>
		/// <param name="sendOnPreanswer">If set to true and send_digits is also set, digits are sent when the call 
		/// is in preanswer state. Defaults to false.</param>
		/// <param name="machineDetection"></param>
		/// <param name="sipHeaders">List of SIP headers in the form of 'key=value' pairs, separated by commas. 
		/// E.g. head1=val1,head2=val2,head3=val3,...,headN=valN. The SIP headers are always prefixed with X-PH-. 
		/// The SIP headers are present for every HTTP request made by the outbound call. Only [A-Z], [a-z] and [0-9]
		/// characters are allowed for the SIP headers key and value. Additionally, the '%' character is also allowed 
		/// for the SIP headers value so that you can encode this value in the URL.</param>
		/// <param name="timeLimit">Schedules the call for hangup at a specified time after the call is answered. 
		/// Value should be an integer > 0(in seconds).</param>
		/// <param name="hangupOnRing">Time allotted to analyze if the call has been answered by a machine. 
		/// It should be an 10000 <= integer >= 2000 and the unit is ms. The default value is 5000 ms.</param>
		/// <param name="ringTimeout">Determines the time in seconds the call should ring. If the call is not answered 
		/// within the ring_timeout value or the default value of 120 s, it is canceled.</param>
		/// <returns>IRestResponse<Call></returns>
		public IRestResponse<Call> MakeCall(string fromNumber, string toNumber, string answerUrl, string answerMethod = "",
		                                    string ringUrl = "", string ringMethod = "", string hangupUrl = "", string hangupMethod = "", string callerName = "",
		                                    string fallbackAnswerUrl = "", string fallbackMethod = "", string sendDigits = "", bool sendOnPreanswer = false,
		                                    bool machineDetection = false, string sipHeaders = "", int timeLimit = 0, int hangupOnRing = 0, int ringTimeout = 0,
		                                    string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("from", fromNumber);
			parameters.Add("to", toNumber);
			parameters.Add("answer_url", answerUrl);
			if (!String.IsNullOrEmpty(answerMethod))
				parameters.Add("answer_method", answerMethod);
			if (!String.IsNullOrEmpty(ringUrl))
				parameters.Add("ring_url", ringUrl);
			if (!String.IsNullOrEmpty(ringMethod))
				parameters.Add("ring_method", ringMethod);
			if (!String.IsNullOrEmpty(hangupUrl))
				parameters.Add("hangup_url", hangupUrl);
			if (!String.IsNullOrEmpty(hangupMethod))
				parameters.Add("hangup_method", hangupMethod);
			if (!String.IsNullOrEmpty(fallbackAnswerUrl))
				parameters.Add("fallback_answer_url", fallbackAnswerUrl);
			if (!String.IsNullOrEmpty(fallbackMethod))
				parameters.Add("fallback_method", fallbackMethod);
			if (!String.IsNullOrEmpty(sendDigits))
				parameters.Add("send_digits", sendDigits);
			if (sendOnPreanswer)
				parameters.Add("send_on_preanswer", sendOnPreanswer.ToString());
			if (machineDetection)
				parameters.Add("machine_detection", machineDetection.ToString());
			if (!String.IsNullOrEmpty(sipHeaders))
				parameters.Add("sip_headers", sipHeaders);
			if (timeLimit > 0)
				parameters.Add("time_limit", timeLimit.ToString());
			if (hangupOnRing > 0)
				parameters.Add("hangup_on_ring", hangupOnRing.ToString());
			if (ringTimeout > 0)
				parameters.Add("ring_timeout", ringTimeout.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Call>("POST", "/Call/", parameters);
		}

		public IRestResponse<Call> MakeBulkCall(dict parameters, dict destNumberSipHeaders)
		{
			string destNumbers = "";
			string headerSIP = "";
			foreach (KeyValuePair<string, string> kvp in destNumberSipHeaders)
			{
				destNumbers += kvp.Key + "<";
				headerSIP += kvp.Value + "<";
			}
			parameters.Add("to", destNumbers.Substring(0, destNumbers.Length - 1));
			parameters.Add("sip_headers", headerSIP.Substring(0, headerSIP.Length - 1));
			return _request<Call>("POST", "/Call/", parameters);
		}

		/// <summary>
		/// End all ongoing calls.
		/// </summary>
		/// <returns>IRestResponse<GenericResponse></returns>
		public IRestResponse<GenericResponse> HangupAllCalls(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", "/Call/", parameters);
		}

		/// <summary>
		/// End an ongoing call.
		/// </summary>
		/// <param name="callUuid">CallUUID of the on going call.</param>
		/// <returns>IRestResponse<GenericResponse></returns>
		public IRestResponse<GenericResponse> HangupCall(string callUuid,
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Call/{0}/", callUuid), parameters);
		}

		/// <summary>
		/// Transfer the call to different XML.
		/// </summary>
		/// <param name="callUuid">CallUUID of the ongoing call.</param>
		/// <param name="legs">'aleg', 'bleg' or 'both' Defaults to 'aleg'
		/// 'aleg' will transfer call_uuid 
		/// 'bleg' will transfer the bridged leg (if found) of call_uuid 
		/// 'both' will transfer call_uuid and bridged leg of call_uuid</param>
		/// <param name="aLegUrl">URL to transfer for 'aleg', if legs is 'aleg' or 'both', 
		/// then aleg_url has to be specified.</param>
		/// <param name="aLegMethod">HTTP method to invoke aleg_url. Defaults to POST.</param>
		/// <param name="bLegUrl">URL to transfer for bridged leg, if legs is 'bleg' or 'both', 
		/// then bleg_url has to be specified.</param>
		/// <param name="bLegMethod">HTTP method to invoke bleg_url. Defaults to POST.</param>
		/// <returns></returns>
		public IRestResponse<GenericResponse> TransferCall(string callUuid, string legs = "", 
		                                                   string aLegUrl = "", string aLegMethod = "", string bLegUrl = "", string bLegMethod = "",
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(aLegUrl))
				parameters.Add("aleg_url", aLegUrl);
			if (!String.IsNullOrEmpty(aLegMethod))
				parameters.Add("aleg_method", aLegMethod);
			if (!String.IsNullOrEmpty(bLegUrl))
				parameters.Add("bleg_url", bLegUrl);
			if (!String.IsNullOrEmpty(bLegMethod))
				parameters.Add("bleg_method", bLegMethod);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Call/{0}/", callUuid), parameters);
		}

		/// <summary>
		/// Start recording an ongoing call.
		/// </summary>
		/// <param name="callUuid">CallUUID of the on going call.</param>
		/// /// <param name="callbackUrl"></param>
		/// <param name="callbackMethod"></param>
		/// <returns></returns>
		public IRestResponse<Record> StartCallRecord(string callUuid,
		                                             string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Record>("POST", String.Format("/Call/{0}/Record/", callUuid), parameters);
		}

		/// <summary>
		///  Stop recording the call
		/// </summary>
		/// <param name="callUuid">CallUUID of the on going call.</param>
		/// <param name="callbackUrl"></param>
		/// <param name="callbackMethod"></param>
		/// <returns></returns>
		public IRestResponse<GenericResponse> StopCallRecord(string callUuid,
		                                                     string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Call/{0}/Record/", callUuid), new dict());
		}

		/// <summary>
		/// Start playing audio on the call.
		/// </summary>
		/// <param name="callUuid">CallUUID of the on going call.</param>
		/// <param name="callbackUrl"></param>
		/// <param name="callbackMethod"></param>
		/// <returns></returns>
		public IRestResponse<GenericResponse> StartPlay(string callUuid,
		                                                string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Call/{0}/Play/", callUuid), parameters);
		}

		public IRestResponse<GenericResponse> StopPlay(string callUuid, 
		                                               string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Call/{0}/Play/", callUuid), parameters);
		}

		public IRestResponse<GenericResponse> Speak(string callUuid, string text,
		                                            string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("text", text);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Call/{0}/Speak/", callUuid), parameters);
		}

		public IRestResponse<GenericResponse> SendDigits(string callUuid, string digits,
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("digits", digits);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Call/{0}/DTMF/", callUuid), parameters);
		}


		// Conferences //
		public IRestResponse<LiveConferenceList> GetLiveConferences(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<LiveConferenceList>("GET", "/Conference/", new dict());
		}

		public IRestResponse<GenericResponse> HangupAllConferences(string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", "/Conference/", parameters);
		}

		public IRestResponse<Conference> GetLiveConference(string conferenceName,
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Conference>("GET", String.Format("/Conference/{0}/", conferenceName), parameters);
		}

		public IRestResponse<GenericResponse> HangupConference(string conferenceName,
		                                                       string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/", conferenceName), parameters);
		}

		public IRestResponse<GenericResponse> HangupMember(string conferenceName, string memberId,
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/", conferenceName, memberId), parameters);
		}

		public IRestResponse<GenericResponse> StartPlayToMember(string conferenceName, string memberId,
		                                                        string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Play/", conferenceName, memberId), parameters);
		}

		public IRestResponse<GenericResponse> StopPlayToMember(string conferenceName, string memberId,
		                                                       string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/Play/", conferenceName, memberId), parameters);
		}

		public IRestResponse<GenericResponse> SpeakToMember(string conferenceName, string memberId,
		                                                    string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Speak/", conferenceName, memberId), parameters);
		}

		public IRestResponse<GenericResponse> DeafMember(string conferenceName, string memberId,
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Deaf/", conferenceName, memberId), new dict());
		}

		public IRestResponse<GenericResponse> UndeafMember(string conferenceName, string memberId,
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod); 

			return _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/Deaf/", conferenceName, memberId), parameters);
		}

		public IRestResponse<GenericResponse> MuteMember(string conferenceName, string memberId,
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Mute/", conferenceName, memberId), parameters);
		}

		public IRestResponse<GenericResponse> UnmuteMember(string conferenceName, string memberId,
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/Mute/", conferenceName, memberId), parameters);
		}

		public IRestResponse<GenericResponse> KickMember(string conferenceName, string memberId, 
		                                                 string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Kick/", conferenceName, memberId), parameters);
		}

		public IRestResponse<Record> StartConferenceRecord(string conferenceName,
		                                                   string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Record>("POST", String.Format("/Conference/{0}/Record/", conferenceName), parameters);
		}

		public IRestResponse<GenericResponse> StopConferenceRecord(string conferenceName, 
		                                                           string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Record/", conferenceName), new dict());
		}


		// Endpoints //
		public IRestResponse<EndpointList> GetEndpoints(int limit = 0, int offset = 0,
		                                                string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (limit > 0)
				parameters.Add("limit", limit.ToString());
			if (offset > 0)
				parameters.Add("offset", offset.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<EndpointList>("GET", "/Endpoint//", parameters);
		}

		public IRestResponse<Endpoint> CreateEndpoint(string username, string password, string alias, 
		                                              string appId = "", string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("username", username);
			parameters.Add("password", password);
			parameters.Add("alias", alias);
			if (!String.IsNullOrEmpty(appId))
				parameters.Add("app_id", appId);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Endpoint>("POST", "/Endpoint/", parameters);
		}

		public IRestResponse<Endpoint> GetEndpoint(string endpointId, 
		                                           string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Endpoint>("GET", String.Format("/Endpoint/{0}/", endpointId), parameters);
		}

		public IRestResponse<GenericResponse> ModifyEndpoint(string endpointId, string alias = "", string password = "", 
		                                                     string appId = "", string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(alias))
				parameters.Add("alias", alias);
			if (!String.IsNullOrEmpty(password))
				parameters.Add("password", password);
			if (!String.IsNullOrEmpty(appId))
				parameters.Add("appId", appId);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", String.Format("/Endpoint/{0}/", endpointId), parameters);
		}

		public IRestResponse<GenericResponse> DeleteEndpoint(string endpointId, string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/Endpoint/{0}/", endpointId), parameters);
		}


		// Messages //
		public IRestResponse<MessageResponse> SendMessage(string srcNumber, string dstNumber, string textMessage, 
		                                                  string type = "", string notificationUrl = "", string notificationMethod = "",
		                                                  string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("src", srcNumber);
			parameters.Add("dst", dstNumber);
			parameters.Add("text", textMessage);
			if (!String.IsNullOrEmpty(type))
				parameters.Add("type", type);
			if (!String.IsNullOrEmpty(notificationUrl))
				parameters.Add("url", notificationUrl);
			if (!String.IsNullOrEmpty(notificationMethod))
				parameters.Add("method", notificationMethod);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<MessageResponse>("POST", "/Message/", parameters);
		}

		public IRestResponse<Message> GetMessage(string messageUuid,
		                                         string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			parameters.Add("record_id", messageUuid);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<Message>("GET", String.Format("/Message/{0}/", messageUuid), parameters);
		}

		public IRestResponse<MessageList> GetMessages(int limit = 0, int offset = 0, 
		                                              string callbackUrl = "", string callbackMethod="")
		{
			dict parameters = new dict();
			if (limit > 0)
				parameters.Add("limit", limit.ToString());
			if (offset > 0)
				parameters.Add("offset", offset.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<MessageList>("GET", "/Message/", parameters);
		}

		// Incoming carriers
		public IRestResponse<IncomingCarrierList> GetIncomingCarriers(string carrierName = "", int limit = 0, int offset = 0,
		                                                              string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(carrierName))
				parameters.Add("name", carrierName);
			if (limit > 0)
				parameters.Add("limit", limit.ToString());
			if (offset > 0)
				parameters.Add("offset", offset.ToString());
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<IncomingCarrierList>("GET", "/IncomingCarrier/", parameters);
		}

		public IRestResponse<IncomingCarrier> GetIncomingCarrier(string carrierId, 
		                                                         string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<IncomingCarrier>("GET", String.Format("/IncomingCarrier/{0}/", carrierId), parameters);
		}

		public IRestResponse<IncomingCarrier> EditIncomingCarrier(string carrierId, string carrierName = "", string ipSet = "",
		                                                          string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(carrierName))
				parameters.Add("name", carrierName);
			if (!String.IsNullOrEmpty(ipSet))
				parameters.Add("ip_set", ipSet);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<IncomingCarrier>("POST", String.Format("/IncomingCarrier/{0}/", carrierId), parameters);
		}

		public IRestResponse<GenericResponse> AddIncomingCarrier(string carrierName, string ipSet,
		                                                         string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(carrierName))
				parameters.Add("name", carrierName);
			if (!String.IsNullOrEmpty(ipSet))
				parameters.Add("ip_set", ipSet);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("POST", "/IncomingCarrier/", parameters);
		}

		public IRestResponse<GenericResponse> RemoveIncomingCarrier(string carrierId,
		                                                            string callbackUrl = "", string callbackMethod = "")
		{
			dict parameters = new dict();
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<GenericResponse>("DELETE", String.Format("/IncomingCarrier/{0}/", carrierId), parameters);
		}

		public IRestResponse<PlivoPricing> pricing(string country_iso,
		                                           string callbackUrl = "", string callbackMethod = "")
		{ 
			dict parameters = new dict();
			parameters.Add("country_iso", country_iso);
			if (!String.IsNullOrEmpty(callbackUrl))
				parameters.Add("callback_url", callbackUrl);
			if (!String.IsNullOrEmpty(callbackMethod))
				parameters.Add("callback_method", callbackMethod);

			return _request<PlivoPricing>("GET", "/Pricing/", parameters);
		}
	}
}