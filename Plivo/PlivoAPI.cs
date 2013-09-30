using System;
using System.Web;
using System.Collections.Generic;
using RestSharp;
using Plivo.Util;
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
            var request = new RestRequest() { Resource = resource, RequestFormat = DataFormat.Json };

            // add the parameters to the request
            foreach (KeyValuePair<string, string> kvp in data)
                request.AddParameter(kvp.Key, HtmlEntity.Convert(kvp.Value));

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

        private string get_key_value(ref dict dict, string key)
        {
            string value = "";
            try
            {
                value = dict[key];
                dict.Remove(key);
            }
            catch (KeyNotFoundException)
            {
                throw new PlivoException(String.Format("Missing mandatory parameter {0}.", key));
            }
            return value;
        }

        // Accounts //
        public Account get_account()
        {
            IRestResponse<Account> account_response = _request<Account>("GET", "/", new dict());
            Account account = new Account ();
            if (account_response.Data != null) {
                account.auth_id = account_response.Data.auth_id;
                account.auto_recharge = account_response.Data.auto_recharge;
                account.api_id = account_response.Data.api_id;
                account.billing_mode = account_response.Data.billing_mode;
                account.auth_id = account_response.Data.auth_id;
                account.cash_credits = account_response.Data.cash_credits;
                account.city = account_response.Data.city;
                account.state = account_response.Data.state;
                account.timezone = account_response.Data.timezone;
                account.resource_uri = account_response.Data.resource_uri;
            } else {
                account.error = account_response.ErrorMessage;
            }
            return account;
        }

        public GenericResponse modify_account(dict parameters)
        {
            IRestResponse<GenericResponse> generic_response = _request<GenericResponse>("POST", "/", parameters);
            GenericResponse response = new GenericResponse ();
            if (generic_response.Data != null) {
                response.api_id = generic_response.Data.api_id;
                response.message = generic_response.Data.message;
                response.error = generic_response.Data.error;
            } else {
                response.error = generic_response.ErrorMessage;
            }

            return response;
        }

        public SubAccountList get_subaccounts()
        {
            IRestResponse<SubAccountList> response = _request<SubAccountList>("GET", "/Subaccount/", new dict());
            SubAccountList response_object = new SubAccountList ();
            if (response != null) {
                response_object.api_id = response.Data.api_id;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public SubAccount get_subaccount(dict parameters)
        {
            string subauth_id = get_key_value(ref parameters, "subauth_id");
            IRestResponse<SubAccount> response = _request<SubAccount>("GET", String.Format("/Subaccount/{0}/", subauth_id), parameters);
            SubAccount response_object = new SubAccount ();
            if (response.Data != null) {
                response_object.account = response.Data.account;
                response_object.api_id = response.Data.api_id;
                response_object.auth_token = response.Data.auth_token;
                response_object.created = response.Data.created;
                response_object.modified = response.Data.modified;
                response_object.name = response.Data.name;
                response_object.resource_uri = response.Data.resource_uri;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public CreateSubAccount create_subaccount(dict parameters)
        {
            IRestResponse<CreateSubAccount> response = _request<CreateSubAccount>("POST", "/Subaccount/", parameters);
            CreateSubAccount response_object = new CreateSubAccount ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.api_id;
                response_object.message = response.Data.api_id;
                response_object.auth_id = response.Data.auth_id;
                response_object.auth_token = response.Data.auth_token;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse modify_subaccount(dict parameters)
        {
            string subauth_id = get_key_value(ref parameters, "subauth_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Subaccount/{0}/", subauth_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse delete_subaccount(dict parameters)
        {
            string subauth_id = get_key_value(ref parameters, "subauth_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Subaccount/{0}/", subauth_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                if (response.ErrorMessage == "Invalid JSON string")
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        // Applications //
        public ApplicationList get_applications()
        {
            IRestResponse<ApplicationList> response = _request<ApplicationList>("GET", "/Application/", new dict());
            ApplicationList response_object = new ApplicationList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public ApplicationList get_applications(dict parameters)
        {
            IRestResponse<ApplicationList> response = _request<ApplicationList>("GET", "/Application/", parameters);
            ApplicationList response_object = new ApplicationList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public Application get_application(dict parameters)
        {
            string app_id = get_key_value(ref parameters, "app_id");
            IRestResponse<Application> response = _request<Application>("GET", String.Format("/Application/{0}/", app_id), parameters);
            Application response_object = new Application ();
            if (response.Data != null) {
                response_object.app_name = response.Data.app_name;
                response_object.answer_url = response.Data.answer_url;
                response_object.answer_method = response.Data.answer_method;
                response_object.app_id = response.Data.app_id;
                response_object.default_app = response.Data.default_app;
                response_object.enabled = response.Data.enabled;
                response_object.fallback_answer_url = response.Data.fallback_answer_url;
                response_object.fallback_method = response.Data.fallback_method;
                response_object.hangup_url = response.Data.hangup_url;
                response_object.hangup_method = response.Data.hangup_method;
                response_object.message_url = response.Data.message_url;
                response_object.message_method = response.Data.message_method;
                response_object.production_app = response.Data.production_app;
                response_object.public_uri = response.Data.public_uri;
                response_object.resource_uri = response.Data.resource_uri;
                response_object.sip_uri = response.Data.sip_uri;
                response_object.sub_account = response.Data.sub_account;
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public CreateApplication create_application(dict parameters)
        {
            IRestResponse<CreateApplication> response = _request<CreateApplication>("POST", "/Application/", parameters);
            CreateApplication response_object = new CreateApplication ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.app_id = response.Data.app_id;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse modify_application(dict parameters)
        {
            string app_id = get_key_value(ref parameters, "app_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Application/{0}/", app_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse delete_application(dict parameters)
        {
            string app_id = get_key_value(ref parameters, "app_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Application/{0}/", app_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        // Numbers //
        public NumberList get_numbers()
        {
            IRestResponse<NumberList> response = _request<NumberList>("GET", "/Number/", new dict());
            NumberList response_object = new NumberList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        [Obsolete("Use search_number_group() instead")]
        public NumberList search_numbers(dict parameters)
        {
            IRestResponse<NumberList> response = _request<NumberList>("GET", "/AvailableNumber/", parameters);
            NumberList response_object = new NumberList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public NumberList search_number_group(dict parameters)
        {
            IRestResponse<NumberList> response = _request<NumberList>("GET", "/AvailableNumberGroup/", parameters);
            NumberList response_object = new NumberList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public Number get_number(dict parameters)
        {
            string number = get_key_value(ref parameters, "number");
            IRestResponse<Number> response = _request<Number>("GET", String.Format("/Number/{0}/", number), parameters);
            Number response_object = new Number ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.number = response.Data.number;
                response_object.carrier = response.Data.carrier;
                response_object.added_on = response.Data.added_on;
                response_object.application = response.Data.application;
                response_object.fax_enabled = response.Data.fax_enabled;
                response_object.number_type = response.Data.number_type;
                response_object.region = response.Data.region;
                response_object.resource_uri = response.Data.resource_uri;
                response_object.api_id = response.Data.api_id;
                response_object.sms_enabled = response.Data.sms_enabled;
                response_object.sms_rate = response.Data.sms_rate;
                response_object.voice_enabled = response.Data.voice_enabled;
                response_object.voice_rate = response.Data.voice_rate;
                response_object.error = response.Data.error;
                response_object.monthly_rental_rate = response.Data.monthly_rental_rate;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public NumberResponse rent_from_number_group(dict parameters)
        {
            string group_id = get_key_value(ref parameters, "group_id");
            IRestResponse<NumberResponse> response = _request<NumberResponse>("POST", String.Format("/AvailableNumberGroup/{0}/", group_id), parameters);
            NumberResponse response_object = new NumberResponse ();
            if (response.Data != null) {
                response_object.status = response.Data.status;
                response_object.numbers = response.Data.numbers;
                response_object.message = response.Data.message;
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse unrent_number(dict parameters)
        {
            string number = get_key_value(ref parameters, "number");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Number/{0}/", number), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse link_application_number(dict parameters)
        {
            string number = get_key_value(ref parameters, "number");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Number/{0}/", number), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse unlink_application_number(dict parameters)
        {
            string number = get_key_value(ref parameters, "number");
            parameters.Add("app_id", "");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Number/{0}/", number), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }


        // Calls //
        public CDRList get_cdrs()
        {
            IRestResponse<CDRList> response = _request<CDRList>("GET", "/Call/", new dict());
            CDRList response_object = new CDRList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public CDRList get_cdrs(dict parameters)
        {
            IRestResponse<CDRList> response = _request<CDRList>("GET", "/Call/", parameters);
            CDRList response_object = new CDRList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public CDR get_cdr(dict parameters)
        {
            string record_id = get_key_value(ref parameters, "record_id");
            IRestResponse<CDR> response = _request<CDR>("GET", String.Format("/Call/{0}/", record_id), parameters);
            CDR response_object = new CDR ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.bill_duration = response.Data.bill_duration;
                response_object.billed_duration = response.Data.billed_duration;
                response_object.call_duration = response.Data.call_duration;
                response_object.call_duration = response.Data.call_duration;
                response_object.call_uuid = response.Data.call_uuid;
                response_object.parent_call_uuid = response.Data.parent_call_uuid;
                response_object.end_time = response.Data.end_time;
                response_object.error = response.Data.error;
                response_object.from_number = response.Data.from_number;
                response_object.to_number = response.Data.to_number;
                response_object.total_rate = response.Data.total_rate;
                response_object.total_amount = response.Data.total_amount;
            } else {
                response_object.error = response.Data.error;
            }
            return response_object;
        }

        public LiveCallList get_live_calls()
        {
            dict parameters = new dict();
            parameters.Add("status", "live");
            IRestResponse<LiveCallList> response = _request<LiveCallList>("GET", "/Call/", parameters);
            LiveCallList response_object = new LiveCallList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.calls = response.Data.calls;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public LiveCall get_live_call(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            parameters.Add("status", "live");
            IRestResponse<LiveCall> response = _request<LiveCall>("GET", String.Format("/Call/{0}/", call_uuid), parameters);
            LiveCall response_object = new LiveCall ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.caller_name = response.Data.caller_name;
                response_object.call_status = response.Data.call_status;
                response_object.call_uuid = response.Data.call_uuid;
                response_object.from = response.Data.from;
                response_object.to = response.Data.to;
                response_object.session_start = response.Data.session_start;
                response_object.direction = response.Data.direction;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public Call make_call(dict parameters)
        {
            IRestResponse<Call> response = _request<Call>("POST", "/Call/", parameters);
            Call response_object = new Call ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
                response_object.request_uuid = response.Data.request_uuid;
            } else {
                response_object.error = response.ErrorMessage;
            }
        }

        public BulkCall make_bulk_call(dict parameters, dict destNumberSipHeaders)
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
            IRestResponse<BulkCall> response = _request<BulkCall>("POST", "/Call/", parameters);
            BulkCall response_object = new BulkCall ();
            if (response.Data != null) {
                response_object.error = response.Data.error;
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.request_uuids = response.Data.request_uuids;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse hangup_all_calls()
        {
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", "/Call/", new dict());
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse hangup_call(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Call/{0}/", call_uuid), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse transfer_call(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Call/{0}/", call_uuid), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public Record record(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<Record> response = _request<Record>("POST", String.Format("/Call/{0}/Record/", call_uuid), parameters);
            Record response_object = new Record ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.url = response.Data.url;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse stop_record(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Call/{0}/Record/", call_uuid), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse play(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Call/{0}/Play/", call_uuid), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse stop_play(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Call/{0}/Play/", call_uuid), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse speak(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Call/{0}/Speak/", call_uuid), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse send_digits(dict parameters)
        {
            string call_uuid = get_key_value(ref parameters, "call_uuid");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Call/{0}/DTMF/", call_uuid), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }


        // Conferences //
        public LiveConferenceList get_live_conferences()
        {
            IRestResponse<LiveConferenceList> response = _request<LiveConferenceList>("GET", "/Conference/", new dict());
            LiveConferenceList response_object = new LiveConferenceList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.conferences = response.Data.conferences;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse hangup_all_conferences()
        {
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", "/Conference/", new dict());
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public Conference get_live_conference(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            IRestResponse<Conference> response = _request<Conference>("GET", String.Format("/Conference/{0}/", conference_name), parameters);
            Conference response_object = new Conference ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.conference_name = response.Data.conference_name;
                response_object.conference_member_count = response.Data.conference_member_count;
                response_object.conference_run_time = response.Data.conference_run_time;
                response_object.error = response.Data.error;
                response_object.members = response.Data.members;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse hangup_conference(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/", conference_name), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse hangup_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse play_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Play/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse stop_play_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/Play/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse speak_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Speak/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse deaf_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Deaf/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse undeaf_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/Deaf/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse mute_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Mute/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse unmute_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Member/{1}/Mute/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse kick_member(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            string member_id = get_key_value(ref parameters, "member_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Conference/{0}/Member/{1}/Kick/", conference_name, member_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public Record record_conference(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            IRestResponse<Record> response = _request<Record>("POST", String.Format("/Conference/{0}/Record/", conference_name), parameters);
            Record response_object = new Record ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
                response_object.url = response.Data.url;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse stop_record_conference(dict parameters)
        {
            string conference_name = get_key_value(ref parameters, "conference_name");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Conference/{0}/Record/", conference_name), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }


        // Endpoints //
        public EndpointList get_endpoints()
        {
            IRestResponse<EndpointList> response = _request<EndpointList>("GET", "/Endpoint/", new dict());
            EndpointList response_object = new EndpointList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public EndpointList get_endpoints(dict parameters)
        {
            IRestResponse<EndpointList> response = _request<EndpointList>("GET", "/Endpoint/", parameters);
            EndpointList response_object = new EndpointList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public CreateEndpoint create_endpoint(dict parameters)
        {
            IRestResponse<CreateEndpoint> response = _request<Endpoint>("POST", "/Endpoint/", parameters);
            CreateEndpoint response_object = new CreateEndpoint ();
            if (response.Data != null) {
                response_object.alias = response.Data.alias;
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
                response_object.username = response.Data.username;
                response_object.endpoint_id = response.Data.endpoint_id;
            } else {
                response_object.error = response.Data.error;
            }
            return response_object;
        }

        public Endpoint get_endpoint(dict parameters)
        {
            string endpoint_id = get_key_value(ref parameters, "endpoint_id");
            IRestResponse<Endpoint> response = _request<Endpoint>("GET", String.Format("/Endpoint/{0}/", endpoint_id), parameters);
            Endpoint response_object = new Endpoint ();
            if (response.Data != null) {
                response_object.alias = response.Data.alias;
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.username = response.Data.username;
                response_object.endpoint_id = response.Data.endpoint_id;
                response_object.password = response.Data.password;
                response_object.resource_uri = response.Data.resource_uri;
                response_object.sip_uri = response.Data.sip_uri;
            } else {
                response_object.error = response.Data.error;
            }
            return response_object;
        }

        public GenericResponse modify_endpoint(dict parameters)
        {
            string endpoint_id = get_key_value(ref parameters, "endpoint_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/Endpoint/{0}/", endpoint_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse delete_endpoint(dict parameters)
        {
            string endpoint_id = get_key_value(ref parameters, "endpoint_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/Endpoint/{0}/", endpoint_id), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }


        // Messages //
        public MessageResponse send_message(dict parameters)
        {
            IRestResponse<MessageResponse> response = _request<MessageResponse>("POST", "/Message/", parameters);
            MessageResponse response_object = new MessageResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
                response_object.message_uuid = response.Data.message_uuid;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public Message get_message(dict parameters)
        {
            string record_id = get_key_value(ref parameters, "record_id");
            IRestResponse<Message> response = _request<Message>("GET", String.Format("/Message/{0}/", record_id), parameters);
            Message response_object = new Message ();
            if (response.Data != null) {
                response_object.total_amount = response.Data.total_amount;
                response_object.total_rate = response.Data.total_rate;
                response_object.to_number = response.Data.to_number;
                response_object.from_number = response.Data.from_number;
                response_object.message_direction = response.Data.message_direction;
                response_object.message_state = response.Data.message_state;
                response_object.message_time = response.Data.message_time;
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }


        public MessageList get_messages()
        {
            IRestResponse<MessageList> response = _request<MessageList>("GET", "/Message/", new dict());
            MessageList response_object = new MessageList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public MessageList get_messages(dict parameters)
        {
            IRestResponse<MessageList> response = _request<MessageList>("GET", "/Message/", parameters);
            MessageList response_object = new MessageList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }


        // Inbound Carriers
        public IncomingCarrierList get_incoming_carriers(dict parameters)
        {
            IRestResponse<IncomingCarrierList> response = _request<IncomingCarrierList>("GET", "/IncomingCarrier/", parameters);
            IncomingCarrierList response_object = new IncomingCarrierList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.meta = response.Data.meta;
                response_object.error = response.Data.error;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public IncomingCarrier get_incoming_carrier(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "carrier_id");
            IRestResponse<IncomingCarrier> response = _request<IncomingCarrier>("GET", String.Format("/IncomingCarrier/{0}/", carrierId), parameters);
            IncomingCarrier response_object = new IncomingCarrier ();
            if (response.Data != null) {
                response_object.carrier_id = response.Data.carrier_id;
                response_object.api_id = response.Data.api_id;
                response_object.api_id = response.Data.api_id;
                response_object.ip_set = response.Data.ip_set;
                response_object.name = response.Data.name;
                response_object.resource_uri = response.Data.resource_uri;
                response_object.sms = response.Data.sms;
                response_object.voice = response.Data.voice;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
        }

        public GenericResponse create_incoming_carrier(dict parameters)
        {
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", "/IncomingCarrier/", parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse modify_incoming_carrier(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "carrier_id");
            IRestResponse<GenericResponse> response = _request<IncomingCarrier>("POST", String.Format("/IncomingCarrier/{0}/", carrierId), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.message = response.Data.message;
                response_object.error = response.Data.error;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse delete_incoming_carrier(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "carrier_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/IncomingCarrier/{0}/", carrierId), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public PlivoPricing pricing(dict parameters)
        {
            IRestResponse<PlivoPricing> response = _request<PlivoPricing>("GET", "/Pricing/", parameters);
            PlivoPricing response_object = new PlivoPricing ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.country = response.Data.country;
                response_object.country_code = response.Data.country_code;
                response_object.country_iso = response.Data.country_iso;
                response_object.phone_numbers = response.Data.phone_numbers;
                response_object.message = response.Data.message;
                response_object.voice = response.Data.voice;
            } else {
                response_object.error = response.Data.error;
            }
            return response_object;
        }

        // Outgoing Carriers
        public OutgoingCarrierList get_outgoing_carriers()
        {
            IRestResponse<OutgoingCarrierList> response = _request<OutgoingCarrierList>("GET", "/OutgoingCarrier/", new dict());
            OutgoingCarrierList response_object = new OutgoingCarrierList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public OutgoingCarrier get_outgoing_carrier(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "carrier_id");
            IRestResponse<OutgoingCarrier> response = _request<OutgoingCarrier>("GET", String.Format("/OutgoingCarrier/{0}/", carrierId), parameters);
            OutgoingCarrier response_object = new OutgoingCarrier ();
            if (response.Data != null) {
                response_object.address = response.Data.address;
                response_object.prefix = response.Data.prefix;
                response_object.failover_address = response.Data.failover_address;
                response_object.failover_prefix = response.Data.failover_prefix;
                response_object.enabled = response.Data.enabled;
                response_object.carrier_id = response.Data.carrier_id;
                response_object.ips = response.Data.ips;
                response_object.retries = response.Data.resource_uri;
                response_object.retries = response.Data.retries;
                response_object.error = response.Data.error;
                response_object.api_id = response.Data.api_id;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse create_outgoing_carrier(dict parameters)
        {
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", "/OutgoingCarrier/", parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse modify_outgoing_carrier(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "carrier_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/OutgoingCarrier/{0}/", carrierId), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse delete_outgoing_carrier(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "carrier_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/OutgoingCarrier/{0}/", carrierId), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        // Outgoing Carrier Routings
        public OutgoingCarrierRoutingList get_outgoing_carrier_routings()
        {
            IRestResponse<OutgoingCarrierRoutingList> response = _request<OutgoingCarrierRoutingList>("GET", "/OutgoingCarrierRouting/", new dict());
            OutgoingCarrierRoutingList response_object = new OutgoingCarrierRoutingList ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.meta = response.Data.meta;
                response_object.objects = response.Data.objects;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public OutgoingCarrierRouting get_outgoing_carrier_routing(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "routing_id");
            IRestResponse<OutgoingCarrierRouting> response = _request<OutgoingCarrierRouting>("GET", String.Format("/OutgoingCarrierRouting/{0}/", carrierId), parameters);
            OutgoingCarrierRouting response_object = new OutgoingCarrierRouting ();
            if (response.Data != null) {
                response_object.digits = response.Data.digits;
                response_object.outgoing_carrier = response.Data.outgoing_carrier;
                response_object.resource_uri = response.Data.resource_uri;
                response_object.routing_id = response.Data.routing_id;
                response_object.priority = response.Data.priority;
                response_object.error = response.Data.error;
                response_object.api_id = response.Data.api_id;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse create_outgoing_carrier_routing(dict parameters)
        {
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", "/OutgoingCarrierRouting/", parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse modify_outgoing_carrier_routing(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "routing_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("POST", String.Format("/OutgoingCarrierRouting/{0}/", carrierId), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                response_object.error = response.ErrorMessage;
            }
            return response_object;
        }

        public GenericResponse delete_outgoing_carrier_routing(dict parameters)
        {
            string carrierId = get_key_value(ref parameters, "routing_id");
            IRestResponse<GenericResponse> response = _request<GenericResponse>("DELETE", String.Format("/OutgoingCarrierRouting/{0}/", carrierId), parameters);
            GenericResponse response_object = new GenericResponse ();
            if (response.Data != null) {
                response_object.api_id = response.Data.api_id;
                response_object.error = response.Data.error;
                response_object.message = response.Data.message;
            } else {
                if (response.ErrorMessage.Equals("Invalid JSON string"))
                    response_object.error = "";
                else
                    response_object.error = response.ErrorMessage;
            }
            return response_object;
        }
    }
}