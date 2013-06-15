using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Web;
using dict = System.Collections.Generic.Dictionary<string, string>;
using list = System.Collections.Generic.List<string>;

namespace Plivo.XML
{
	/// <summary>
	/// Plivo exception.
	/// </summary>
	public class PlivoException : Exception
	{
		public PlivoException(string message) : base(message) { }
	}

	/// <summary>
	/// Plivo element.
	/// </summary>
	public abstract class PlivoElement
	{
		protected list Nestables { get; set; }
		protected XElement Element { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.PlivoElement"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		public PlivoElement(string body)
		{
			Element = new XElement(this.GetType().Name, HttpUtility.HtmlEncode(body));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.PlivoElement"/> class.
		/// </summary>
		public PlivoElement()
		{
			Element = new XElement(this.GetType().Name);
		}

		/// <summary>
		/// Adds the attributes.
		/// </summary>
		/// <param name="attributes">Attributes.</param>
		protected void AddAttributes(dict attributes)
		{
			foreach (KeyValuePair<string, string> kvp in attributes)
			{
				Element.SetAttributeValue(kvp.Key, ConvertValues(kvp.Value));
			}
		}

		/// <summary>
		/// Converts the values.
		/// </summary>
		/// <returns>The values.</returns>
		/// <param name="value">Value.</param>
		private string ConvertValues(string value)
		{
			string val = "";
			switch (value.ToLower())
			{
				case "true": val = value.ToLower();
				break;
				case "false": val = value.ToLower();
				break;
				case "get": val = value.ToUpper();
				break;
				case "post": val = value.ToUpper();
				break;
				case "man": val = value.ToUpper();
				break;
				case "woman": val = value.ToUpper();
				break;
				default: val = value;
				break;
			}
			return val;
		}

		/// <summary>
		/// Add the specified element.
		/// </summary>
		/// <param name="element">Element.</param>
		public PlivoElement Add(PlivoElement element)
		{
			int posn = Nestables.FindIndex(n => n == element.GetType().Name);
			if (posn >= 0)
			{
				Element.Add(element.Element);
				return this;
			}
			else
				throw new PlivoException(String.Format("Element {0} cannot be nested within {1}", element.GetType().Name, this.GetType().Name));
		}

		/// <summary>
		/// Adds the speak.
		/// </summary>
		/// <returns>The speak.</returns>
		/// <param name="body">Body.</param>
		/// <param name="language">Language.</param>
		/// <param name="loop">Loop.</param>
		/// <param name="voice">Voice.</param>
		public PlivoElement AddSpeak(string body, string language="en-US", int loop=1, string voice="WOMAN")
		{
			return Add(new Speak(body, language, loop, voice="WOMAN"));
		}

		/// <summary>
		/// Adds the play.
		/// </summary>
		/// <returns>The play.</returns>
		/// <param name="body">Body.</param>
		/// <param name="loop">Loop.</param>
		public PlivoElement AddPlay(string body, int loop=1)
		{
			return Add(new Play(body, loop));
		}

		/// <summary>
		/// Adds the get digits.
		/// </summary>
		/// <returns>The get digits.</returns>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="timeout">Timeout.</param>
		/// <param name="digitTimeout">Digit timeout.</param>
		/// <param name="finishOnKey">Finish on key.</param>
		/// <param name="numDigits">Number digits.</param>
		/// <param name="retries">Retries.</param>
		/// <param name="invalidDigitsSound">Invalid digits sound.</param>
		/// <param name="validDigits">Valid digits.</param>
		/// <param name="playBeep">If set to <c>true</c> play beep.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		public PlivoElement AddGetDigits(string action, string method="POST", int timeout=5, int digitTimeout=2, 
		                                 string finishOnKey="#", int numDigits=99, int retries=1, string invalidDigitsSound="", 
		                                 string validDigits="1234567890*#", bool playBeep=false, bool redirect=false)
		{
			return Add(new GetDigits(action, method, timeout, digitTimeout, finishOnKey, numDigits, retries, invalidDigitsSound,
			                         validDigits, playBeep, redirect));
		}

		/// <summary>
		/// Adds the record.
		/// </summary>
		/// <returns>The record.</returns>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="timeout">Timeout.</param>
		/// <param name="finishOnKey">Finish on key.</param>
		/// <param name="maxLength">Max length.</param>
		/// <param name="playBeep">If set to <c>true</c> play beep.</param>
		/// <param name="recordSession">If set to <c>true</c> record session.</param>
		/// <param name="startOnDialAnswer">If set to <c>true</c> start on dial answer.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		/// <param name="fileFormat">File format.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		/// <param name="transcriptionType">Transcription type.</param>
		/// <param name="transcriptionUrl">Transcription URL.</param>
		/// <param name="transcriptionMethod">Transcription method.</param>
		public PlivoElement AddRecord(string action, string method="POST", int timeout=15, string finishOnKey="", 
		                              int maxLength=60, bool playBeep=true, bool recordSession=false, bool startOnDialAnswer=false, 
		                              bool redirect=true, string fileFormat="mp3", string callbackUrl="", string callbackMethod="", 
		                              string transcriptionType="auto", string transcriptionUrl="", string transcriptionMethod="GET")
		{
			return Add(new Record(action, method, timeout, finishOnKey, maxLength, playBeep, recordSession, startOnDialAnswer,
			                      redirect, fileFormat, callbackUrl, callbackMethod, transcriptionType, transcriptionUrl,
			                      transcriptionMethod));
		}

		/// <summary>
		/// Adds the dial.
		/// </summary>
		/// <returns>The dial.</returns>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="hangupOnStar">If set to <c>true</c> hangup on star.</param>
		/// <param name="timeLimit">Time limit.</param>
		/// <param name="timeout">Timeout.</param>
		/// <param name="callerId">Caller identifier.</param>
		/// <param name="callerName">Caller name.</param>
		/// <param name="confirmSound">Confirm sound.</param>
		/// <param name="confirmKey">Confirm key.</param>
		/// <param name="dialMusic">Dial music.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		/// <param name="digitsMatch">Digits match.</param>
		/// <param name="sipHeaders">Sip headers.</param>
		public PlivoElement AddDial(string action="", string method="POST", bool hangupOnStar=false, int timeLimit=14400, int timeout=-1, 
		                            int callerId=-1, string callerName="", string confirmSound="", string confirmKey="", string dialMusic="", 
		                            string callbackUrl="", string callbackMethod="POST", bool redirect=true, string digitsMatch="", string sipHeaders="")
		{
			return Add(new Dial(action, method, hangupOnStar, timeLimit, timeout, callerId, callerName, confirmSound, confirmKey,
			                    dialMusic, callbackUrl, callbackMethod, redirect, digitsMatch, sipHeaders));
		}

		/// <summary>
		/// Adds the number.
		/// </summary>
		/// <returns>The number.</returns>
		/// <param name="body">Body.</param>
		/// <param name="sendDigits">Send digits.</param>
		/// <param name="sendDigitsMode">Send digits mode.</param>
		/// <param name="sendOnPreAnswer">If set to <c>true</c> send on pre answer.</param>
		public PlivoElement AddNumber(string body, string sendDigits="", string sendDigitsMode="", bool sendOnPreAnswer=false)
		{
			return Add(new Number(body, sendDigits, sendDigitsMode, sendOnPreAnswer));
		}

		/// <summary>
		/// Adds the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="body">Body.</param>
		/// <param name="sendDigits">Send digits.</param>
		/// <param name="sendDigitsMode">Send digits mode.</param>
		/// <param name="sendOnPreAnswer">If set to <c>true</c> send on pre answer.</param>
		/// <param name="sipHeaders">Sip headers.</param>
		public PlivoElement AddUser(string body, string sendDigits="", string sendDigitsMode="", bool sendOnPreAnswer=false, string sipHeaders="")
		{
			return Add(new User(body, sendDigits, sendDigitsMode, sendOnPreAnswer, sipHeaders));
		}

		/// <summary>
		/// Adds the redirect.
		/// </summary>
		/// <returns>The redirect.</returns>
		/// <param name="body">Body.</param>
		/// <param name="method">Method.</param>
		public PlivoElement AddRedirect(string body, string method="POST")
		{
			return Add(new Redirect(body, method));
		}

		/// <summary>
		/// Adds the wait.
		/// </summary>
		/// <returns>The wait.</returns>
		/// <param name="length">Length.</param>
		/// <param name="silence">If set to <c>true</c> silence.</param>
		public PlivoElement AddWait(string length, bool silence=false)
		{
			return Add(new Wait(length, silence));
		}

		/// <summary>
		/// Adds the hangup.
		/// </summary>
		/// <returns>The hangup.</returns>
		/// <param name="reason">Reason.</param>
		/// <param name="schedule">Schedule.</param>
		public PlivoElement AddHangup(string reason="", int schedule=0)
		{
			return Add(new Hangup(schedule, reason));
		}

		/// <summary>
		/// Adds the pre answer.
		/// </summary>
		/// <returns>The pre answer.</returns>
		public PlivoElement AddPreAnswer()
		{
			return Add(new PreAnswer());
		}

		/// <summary>
		/// Adds the conference.
		/// </summary>
		/// <returns>The conference.</returns>
		/// <param name="body">Body.</param>
		/// <param name="sendDigits">Send digits.</param>
		/// <param name="muted">If set to <c>true</c> muted.</param>
		/// <param name="enterSound">Enter sound.</param>
		/// <param name="exitSound">Exit sound.</param>
		/// <param name="startConferenceOnEnter">If set to <c>true</c> start conference on enter.</param>
		/// <param name="endConferenceOnExit">If set to <c>true</c> end conference on exit.</param>
		/// <param name="stayAlone">If set to <c>true</c> stay alone.</param>
		/// <param name="waitSound">Wait sound.</param>
		/// <param name="maxMembers">Max members.</param>
		/// <param name="timeLimit">Time limit.</param>
		/// <param name="hangupOnStar">If set to <c>true</c> hangup on star.</param>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		/// <param name="digitsMatch">Digits match.</param>
		/// <param name="floorEvent">If set to <c>true</c> floor event.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		/// <param name="record">If set to <c>true</c> record.</param>
		/// <param name="recordFileFormat">Record file format.</param>
		/// <param name="transcriptionType">Transcription type.</param>
		/// <param name="transcriptionUrl">Transcription URL.</param>
		/// <param name="transcriptionMethod">Transcription method.</param>
		public PlivoElement AddConference(string body, string sendDigits="", bool muted=false, string enterSound="", string exitSound="", 
		                                  bool startConferenceOnEnter=true, bool endConferenceOnExit=false, bool stayAlone=true, string waitSound="", 
		                                  int maxMembers=200, int timeLimit=0, bool hangupOnStar=false, string action="", string method="", 
		                                  string callbackUrl="", string callbackMethod="POST", string digitsMatch="", bool floorEvent=false, 
		                                  bool redirect=true, bool record=true, string recordFileFormat="mp3", string transcriptionType="auto", 
		                                  string transcriptionUrl="", string transcriptionMethod="GET")
		{
			return Add(new Conference(body, sendDigits, muted, enterSound, exitSound, startConferenceOnEnter, endConferenceOnExit, 
			                          stayAlone, waitSound, maxMembers, timeLimit, hangupOnStar, action, method, callbackUrl, 
			                          callbackMethod, digitsMatch, floorEvent, redirect, record, recordFileFormat, transcriptionType,
			                          transcriptionUrl, transcriptionMethod));
		}

		/// <summary>
		/// Adds the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="body">Body.</param>
		/// <param name="srcNumber">Source number.</param>
		/// <param name="dstNumber">Dst number.</param>
		/// <param name="msgType">Message type.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		public PlivoElement AddMessage(string body, string srcNumber, string dstNumber, string msgType="sms", string callbackUrl="", string callbackMethod="POST")
		{
			return Add(new Message(body, srcNumber, dstNumber, msgType, callbackUrl, callbackMethod));
		}

		/// <summary>
		/// Adds the DTMF.
		/// </summary>
		/// <returns>The DTMF.</returns>
		/// <param name="body">Body.</param>
		public PlivoElement AddDTMF(string body)
		{
			return Add(new DTMF(body));
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Plivo.XML.PlivoElement"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Plivo.XML.PlivoElement"/>.</returns>
		public override string ToString()
		{
			return SerializeToXML().ToString();
		}

		protected XDocument SerializeToXML()
		{
			return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), Element);
		}
	}

	/// <summary>
	/// Response.
	/// </summary>
	public class Response : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Response"/> class.
		/// </summary>
		public Response()
			: base()
		{
			Nestables = new list()
			{   "Speak", "Play", "GetDigits", "Record", "Dial", "Message", "Redirect",
				"Wait", "Hangup", "PreAnswer", "Conference", "DTMF"
			};
		}
	}

	/// <summary>
	/// Dial.
	/// </summary>
	public class Dial : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Dial"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="hangupOnStar">If set to <c>true</c> hangup on star.</param>
		/// <param name="timeLimit">Time limit.</param>
		/// <param name="timeout">Timeout.</param>
		/// <param name="callerId">Caller identifier.</param>
		/// <param name="callerName">Caller name.</param>
		/// <param name="confirmSound">Confirm sound.</param>
		/// <param name="confirmKey">Confirm key.</param>
		/// <param name="dialMusic">Dial music.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		/// <param name="digitsMatch">Digits match.</param>
		/// <param name="sipHeaders">Sip headers.</param>
		public Dial(string action="", string method="POST", bool hangupOnStar=false, int timeLimit=14400, int timeout=-1, 
		            int callerId=-1, string callerName="", string confirmSound="", string confirmKey="", string dialMusic="", 
		            string callbackUrl="", string callbackMethod="POST", bool redirect=true, string digitsMatch="", string sipHeaders="")
			: base()
		{
			Nestables = new list() 
			{   "Number", "User" 
			};

			dict attributes = new dict();
			if (!String.IsNullOrEmpty(action))
				attributes.Add("action", action);
			if (!String.IsNullOrEmpty(callerName))
				attributes.Add("callerName", callerName);
			if (!String.IsNullOrEmpty(confirmSound))
				attributes.Add("confirmSound", confirmSound);
			if (!String.IsNullOrEmpty(confirmKey))
				attributes.Add("confirmKey", confirmKey);
			if (!String.IsNullOrEmpty(dialMusic))
				attributes.Add("dialMusic", dialMusic);
			if (!String.IsNullOrEmpty(callbackUrl))
				attributes.Add("callbackUrl", callbackUrl);
			if (!String.IsNullOrEmpty(digitsMatch))
				attributes.Add("digitsMatch", digitsMatch);
			if (!String.IsNullOrEmpty(sipHeaders))
				attributes.Add("sipHeaders", sipHeaders);

			attributes.Add("callbackMethod", callbackMethod);
			attributes.Add("redirect", redirect.ToString());

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Number.
	/// </summary>
	public class Number : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Number"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		/// <param name="sendDigits">Send digits.</param>
		/// <param name="sendDigitsMode">Send digits mode.</param>
		/// <param name="sendOnPreAnswer">If set to <c>true</c> send on pre answer.</param>
		public Number(string body, string sendDigits="", string sendDigitsMode="", bool sendOnPreAnswer=false)
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			if (!String.IsNullOrEmpty(sendDigits))
				attributes.Add("sendDigits", sendDigits);
			if (!String.IsNullOrEmpty(sendDigitsMode))
				attributes.Add("sendDigitsMode", sendDigitsMode);
			attributes.Add("sendOnPreAnswer", sendOnPreAnswer.ToString());

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// User.
	/// </summary>
	public class User : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.User"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		/// <param name="sendDigits">Send digits.</param>
		/// <param name="sendDigitsMode">Send digits mode.</param>
		/// <param name="sendOnPreAnswer">If set to <c>true</c> send on pre answer.</param>
		/// <param name="sipHeaders">Sip headers.</param>
		public User(string body, string sendDigits="", string sendDigitsMode="", bool sendOnPreAnswer=false, string sipHeaders="")
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			if (!String.IsNullOrEmpty(sendDigits))
				attributes.Add("sendDigits", sendDigits);
			if (!String.IsNullOrEmpty(sendDigitsMode))
				attributes.Add("sendDigitsMode", sendDigitsMode);
			if (!String.IsNullOrEmpty(sipHeaders))
				attributes.Add("sipHeaders", sipHeaders);
			attributes.Add("sendOnPreAnswer", sendOnPreAnswer.ToString());

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Conference.
	/// </summary>
	public class Conference : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Conference"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		/// <param name="muted">If set to <c>true</c> muted.</param>
		/// <param name="enterSound">Enter sound.</param>
		/// <param name="exitSound">Exit sound.</param>
		/// <param name="startConferenceOnEnter">If set to <c>true</c> start conference on enter.</param>
		/// <param name="endConferenceOnExit">If set to <c>true</c> end conference on exit.</param>
		/// <param name="stayAlone">If set to <c>true</c> stay alone.</param>
		/// <param name="waitSound">Wait sound.</param>
		/// <param name="maxMembers">Max members.</param>
		/// <param name="timeLimit">Time limit.</param>
		/// <param name="hangupOnStar">If set to <c>true</c> hangup on star.</param>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		/// <param name="digitsMatch">Digits match.</param>
		/// <param name="floorEvent">If set to <c>true</c> floor event.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		/// <param name="record">If set to <c>true</c> record.</param>
		/// <param name="recordFileFormat">Record file format.</param>
		/// <param name="transcriptionType">Transcription type.</param>
		/// <param name="transcriptionUrl">Transcription URL.</param>
		/// <param name="transcriptionMethod">Transcription method.</param>
		public Conference(string body, bool muted=false, string enterSound="", string exitSound="", 
		                  bool startConferenceOnEnter=true, bool endConferenceOnExit=false, bool stayAlone=true, string waitSound="", 
		                  int maxMembers=200, int timeLimit=0, bool hangupOnStar=false, string action="", string method="", 
		                  string callbackUrl="", string callbackMethod="POST", string digitsMatch="", bool floorEvent=false, 
		                  bool redirect=true, bool record=true, string recordFileFormat="mp3", string transcriptionType="auto", 
		                  string transcriptionUrl="", string transcriptionMethod="GET")
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			if (!String.IsNullOrEmpty(enterSound))
				attributes.Add("enterSound", enterSound);
			if (!String.IsNullOrEmpty(exitSound))
				attributes.Add("exitSound", exitSound);
			if (!String.IsNullOrEmpty(waitSound))
				attributes.Add("waitSound", waitSound);
			if (!String.IsNullOrEmpty(action))
				attributes.Add("action", action);
			if (!String.IsNullOrEmpty(method))
				attributes.Add("method", method);
			if (!String.IsNullOrEmpty(callbackUrl))
				attributes.Add("callbackUrl", callbackUrl);
			if (!String.IsNullOrEmpty(digitsMatch))
				attributes.Add("digitsMatch", digitsMatch);
			if (!String.IsNullOrEmpty(transcriptionUrl))
				attributes.Add("transcriptionUrl", transcriptionUrl);

			attributes.Add("muted", muted.ToString());
			attributes.Add("startConferenceOnEnter", startConferenceOnEnter.ToString());
			attributes.Add("endConferenceOnExit", endConferenceOnExit.ToString());
			attributes.Add("stayAlone", stayAlone.ToString());
			attributes.Add("maxMembers", maxMembers.ToString());
			attributes.Add("timeLimit", timeLimit.ToString());
			attributes.Add("hangupOnStar", hangupOnStar.ToString());
			attributes.Add("callbackMethod", callbackMethod);
			attributes.Add("floorEvent", floorEvent.ToString());
			attributes.Add("redirect", redirect.ToString());
			attributes.Add("record", record.ToString());
			attributes.Add("recordFileFormat", recordFileFormat);
			attributes.Add("transcriptionType", transcriptionType);
			attributes.Add("transcriptionMethod", transcriptionMethod);

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Get digits.
	/// </summary>
	public class GetDigits : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.GetDigits"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="timeout">Timeout.</param>
		/// <param name="digitTimeout">Digit timeout.</param>
		/// <param name="finishOnKey">Finish on key.</param>
		/// <param name="numDigits">Number digits.</param>
		/// <param name="retries">Retries.</param>
		/// <param name="invalidDigitsSound">Invalid digits sound.</param>
		/// <param name="validDigits">Valid digits.</param>
		/// <param name="playBeep">If set to <c>true</c> play beep.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		public GetDigits(string action, string method="POST", int timeout=5, int digitTimeout=2, 
		                 string finishOnKey="#", int numDigits=99, int retries=1, string invalidDigitsSound="", 
		                 string validDigits="1234567890*#", bool playBeep=false, bool redirect=false)
			: base()
		{
			Nestables = new list()
			{   "Speak", "Play", "Wait"
			};
			dict attributes = new dict();
			attributes.Add("action", action);
			attributes.Add("method", method);
			attributes.Add("timeout", timeout.ToString());
			attributes.Add("digitTimeout", digitTimeout.ToString());
			attributes.Add("finishOnKey", finishOnKey);
			attributes.Add("numDigits", numDigits.ToString());
			attributes.Add("retries", retries.ToString());
			attributes.Add("timeout", timeout.ToString());
			attributes.Add("digitTimeout", digitTimeout.ToString());
			if (!String.IsNullOrEmpty(invalidDigitsSound))
				attributes.Add("invalidDigitsSound", invalidDigitsSound.ToString());

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Speak.
	/// </summary>
	public class Speak : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Speak"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		/// <param name="language">Language.</param>
		/// <param name="loop">Loop.</param>
		/// <param name="voice">Voice.</param>
		public Speak(string body, string language="en-US", int loop=1, string voice="WOMAN")
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			attributes.Add("language", language);
			attributes.Add("loop", loop.ToString());
			attributes.Add("voice", voice);

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Play.
	/// </summary>
	public class Play : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Play"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		/// <param name="loop">Loop.</param>
		public Play(string body, int loop=1)
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			attributes.Add("loop", loop.ToString());

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Wait.
	/// </summary>
	public class Wait : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Wait"/> class.
		/// </summary>
		/// <param name="length">Length.</param>
		/// <param name="silence">If set to <c>true</c> silence.</param>
		public Wait(string length, bool silence=false)
			: base()
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			if (!String.IsNullOrEmpty(length))
				attributes.Add("length", length);
			attributes.Add("silence", silence.ToString());

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Redirect.
	/// </summary>
	public class Redirect : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Redirect"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		/// <param name="method">Method.</param>
		public Redirect(string body, string method="")
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			if (!String.IsNullOrEmpty(method))
				attributes.Add("method", method);

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Hangup.
	/// </summary>
	public class Hangup : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Hangup"/> class.
		/// </summary>
		/// <param name="schedule">Schedule.</param>
		/// <param name="reason">Reason.</param>
		public Hangup(int schedule=0, string reason="")
			: base()
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			if (schedule != 0)
				attributes.Add("schedule", schedule.ToString());
			if (!String.IsNullOrEmpty(reason))
				attributes.Add("reason", reason);

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// Record.
	/// </summary>
	public class Record : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Record"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="method">Method.</param>
		/// <param name="timeout">Timeout.</param>
		/// <param name="finishOnKey">Finish on key.</param>
		/// <param name="maxLength">Max length.</param>
		/// <param name="playBeep">If set to <c>true</c> play beep.</param>
		/// <param name="recordSession">If set to <c>true</c> record session.</param>
		/// <param name="startOnDialAnswer">If set to <c>true</c> start on dial answer.</param>
		/// <param name="redirect">If set to <c>true</c> redirect.</param>
		/// <param name="fileFormat">File format.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		/// <param name="transcriptionType">Transcription type.</param>
		/// <param name="transcriptionUrl">Transcription URL.</param>
		/// <param name="transcriptionMethod">Transcription method.</param>
		public Record(string action, string method="POST", int timeout=15, string finishOnKey="", 
		              int maxLength=60, bool playBeep=true, bool recordSession=false, bool startOnDialAnswer=false, 
		              bool redirect=true, string fileFormat="mp3", string callbackUrl="", string callbackMethod="", 
		              string transcriptionType="auto", string transcriptionUrl="", string transcriptionMethod="GET")
			: base()
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			attributes.Add("action", action);
			attributes.Add("method", method);
			attributes.Add("timeout", timeout.ToString());
			if (!String.IsNullOrEmpty(finishOnKey))
				attributes.Add("finishOnKey", finishOnKey);
			attributes.Add("maxLength", maxLength.ToString());
			attributes.Add("playBeep", playBeep.ToString());
			attributes.Add("recordSession", recordSession.ToString());
			attributes.Add("startOnDialAnswer", startOnDialAnswer.ToString());
			attributes.Add("redirect", redirect.ToString());
			attributes.Add("fileFormat", fileFormat);
			if (!String.IsNullOrEmpty(callbackUrl))
				attributes.Add("callbackUrl", callbackUrl);
			attributes.Add("callbackMethod", callbackMethod);
			attributes.Add("transcriptionType", transcriptionType);
			if (!String.IsNullOrEmpty(transcriptionUrl))
				attributes.Add("transcriptionUrl", transcriptionUrl);
			attributes.Add("transcriptionMethod", transcriptionMethod);

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// PreAnswer.
	/// </summary>
	public class PreAnswer : PlivoElement
	{
		public PreAnswer()
			: base()
		{
			Nestables = new list()
			{   "Play", "Speak", "GetDigits", "Wait", "Redirect", "Message", "DTMF"
			};
		}
	}

	/// <summary>
	/// Message.
	/// </summary>
	public class Message : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.Message"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		/// <param name="srcNumber">Source number.</param>
		/// <param name="dstNumber">Dst number.</param>
		/// <param name="msgType">Message type.</param>
		/// <param name="callbackUrl">Callback URL.</param>
		/// <param name="callbackMethod">Callback method.</param>
		public Message(string body, string srcNumber, string dstNumber, string msgType="sms", string callbackUrl="", string callbackMethod="POST")
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			attributes.Add("src", srcNumber);
			attributes.Add("dst", dstNumber);
			attributes.Add("type", msgType);
			if (!String.IsNullOrEmpty(callbackUrl))
				attributes.Add("callbackUrl", callbackUrl);
			attributes.Add("callbackUrl", callbackUrl);
			attributes.Add("callbackMethod", callbackMethod);

			AddAttributes(attributes);
		}
	}

	/// <summary>
	/// DTMF.
	/// </summary>
	public class DTMF : PlivoElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Plivo.XML.DTMF"/> class.
		/// </summary>
		/// <param name="body">Body.</param>
		public DTMF(string body)
			: base(body)
		{
			Nestables = new list() { "" };
		}
	}
}