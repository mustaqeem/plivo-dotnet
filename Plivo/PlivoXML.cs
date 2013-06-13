using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Web;
using dict = System.Collections.Generic.Dictionary<string, string>;
using list = System.Collections.Generic.List<string>;

namespace Plivo.XML
{
	public class PlivoException : Exception
	{
		public PlivoException(string message) : base(message) { }
	}

	public abstract class PlivoElement
	{
		protected list Nestables { get; set; }
		protected XElement Element { get; set; }

		public PlivoElement(string body)
		{
			Element = new XElement(this.GetType().Name, HttpUtility.HtmlEncode(body));
			//Element = new XElement(this.GetType().Name, body);
		}

		public PlivoElement()
		{
			Element = new XElement(this.GetType().Name);
		}

		protected void AddAttributes(dict attributes)
		{
			foreach (KeyValuePair<string, string> kvp in attributes)
			{
				Element.SetAttributeValue(kvp.Key, ConvertValues(kvp.Value));
			}
		}

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

		public PlivoElement AddSpeak(string body, string language="en-US", int loop=1, string voice="WOMAN")
		{
			return Add(new Speak(body, language, loop, voice="WOMAN"));
		}

		public PlivoElement AddPlay(string body, int loop=1)
		{
			return Add(new Play(body, loop));
		}

		public PlivoElement AddGetDigits(string action, string method="POST", int timeout=5, int digitTimeout=2, 
		                                 string finishOnKey="#", int numDigits=99, int retries=1, string invalidDigitsSound="", 
		                                 string validDigits="1234567890*#", bool playBeep=false, bool redirect=false)
		{
			return Add(new GetDigits(action, method, timeout, digitTimeout, finishOnKey, numDigits, retries, invalidDigitsSound,
			                         validDigits, playBeep, redirect));
		}

		public PlivoElement AddRecord(string action, string method="POST", int timeout=15, string finishOnKey="", 
		                              int maxLength=60, bool playBeep=true, bool recordSession=false, bool startOnDialAnswer=false, 
		                              bool redirect=true, string fileFormat="mp3", string callbackUrl="", string callbackMethod="", 
		                              string transcriptionType="auto", string transcriptionUrl="", string transcriptionMethod="GET")
		{
			return Add(new Record(action, method, timeout, finishOnKey, maxLength, playBeep, recordSession, startOnDialAnswer,
			                      redirect, fileFormat, callbackUrl, callbackMethod, transcriptionType, transcriptionUrl,
			                      transcriptionMethod));
		}

		public PlivoElement AddDial(string action="", string method="POST", bool hangupOnStar=false, int timeLimit=14400, int timeout=-1, 
		                            int callerId=-1, string callerName="", string confirmSound="", string confirmKey="", string dialMusic="", 
		                            string callbackUrl="", string callbackMethod="POST", bool redirect=true, string digitsMatch="", string sipHeaders="")
		{
			return Add(new Dial(action, method, hangupOnStar, timeLimit, timeout, callerId, callerName, confirmSound, confirmKey,
			                    dialMusic, callbackUrl, callbackMethod, redirect, digitsMatch, sipHeaders));
		}

		public PlivoElement AddNumber(string body, string sendDigits="", string sendDigitsMode="", bool sendOnPreAnswer=false)
		{
			return Add(new Number(body, sendDigits, sendDigitsMode, sendOnPreAnswer));
		}

		public PlivoElement AddUser(string body, string sendDigits="", string sendDigitsMode="", bool sendOnPreAnswer=false, string sipHeaders="")
		{
			return Add(new User(body, sendDigits, sendDigitsMode, sendOnPreAnswer, sipHeaders));
		}

		public PlivoElement AddRedirect(string body, string method="POST")
		{
			return Add(new Redirect(body, method));
		}

		public PlivoElement AddWait(string length, bool silence=false)
		{
			return Add(new Wait(length, silence));
		}

		public PlivoElement AddHangup(string reason="", int schedule=0)
		{
			return Add(new Hangup(schedule, reason));
		}

		public PlivoElement AddPreAnswer()
		{
			return Add(new PreAnswer());
		}

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

		public PlivoElement AddMessage(string body, string srcNumber, string dstNumber, string msgType="sms", string callbackUrl="", string callbackMethod="POST")
		{
			return Add(new Message(body, srcNumber, dstNumber, msgType, callbackUrl, callbackMethod));
		}

		public PlivoElement AddDTMF(string body)
		{
			return Add(new DTMF(body));
		}

		public override string ToString()
		{
			return SerializeToXML().ToString();
		}

		protected XDocument SerializeToXML()
		{
			return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), Element);
		}
	}

	public class Response : PlivoElement
	{
		public Response()
			: base()
		{
			Nestables = new list()
			{   "Speak", "Play", "GetDigits", "Record", "Dial", "Message", "Redirect",
				"Wait", "Hangup", "PreAnswer", "Conference", "DTMF"
			};
		}
	}

	public class Dial : PlivoElement
	{
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

	public class Number : PlivoElement
	{
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

	public class User : PlivoElement
	{
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

	public class Conference : PlivoElement
	{
		public Conference(string body, string sendDigits="", bool muted=false, string enterSound="", string exitSound="", 
		                  bool startConferenceOnEnter=true, bool endConferenceOnExit=false, bool stayAlone=true, string waitSound="", 
		                  int maxMembers=200, int timeLimit=0, bool hangupOnStar=false, string action="", string method="", 
		                  string callbackUrl="", string callbackMethod="POST", string digitsMatch="", bool floorEvent=false, 
		                  bool redirect=true, bool record=true, string recordFileFormat="mp3", string transcriptionType="auto", 
		                  string transcriptionUrl="", string transcriptionMethod="GET")
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			if (!String.IsNullOrEmpty(sendDigits))
				attributes.Add("sendDigits", sendDigits);
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

	public class GetDigits : PlivoElement
	{
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

	public class Speak : PlivoElement
	{
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

	public class Play : PlivoElement
	{
		public Play(string body, int loop=1)
			: base(body)
		{
			Nestables = new list() { "" };
			dict attributes = new dict();
			attributes.Add("loop", loop.ToString());

			AddAttributes(attributes);
		}
	}

	public class Wait : PlivoElement
	{
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

	public class Redirect : PlivoElement
	{
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

	public class Hangup : PlivoElement
	{
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

	public class Record : PlivoElement
	{
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

	public class Message : PlivoElement
	{
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

	public class DTMF : PlivoElement
	{
		public DTMF(string body)
			: base(body)
		{
			Nestables = new list() { "" };
		}
	}
}