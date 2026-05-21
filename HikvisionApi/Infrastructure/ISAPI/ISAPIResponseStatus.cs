using System.Xml.Serialization;

namespace Infrastructure.ISAPI;

public class ISAPIResponseStatus
{
	public int statusCode { get; set; }

	public string statusString { get; set; } = string.Empty;

	public string subStatusCode { get; set; } = string.Empty;
}

[XmlRoot(ElementName = "ResponseStatus", Namespace = "http://www.hikvision.com/ver10/XMLSchema")]
public class ISAPIXMLHostResponseStatus
{
	[XmlElement(ElementName = "requestURL")]
	public string RequestURL { get; set; } = string.Empty;

	[XmlElement(ElementName = "statusCode")]
	public int StatusCode { get; set; }

	[XmlElement(ElementName = "statusString")]
	public string StatusString { get; set; } = string.Empty;

	[XmlElement(ElementName = "subStatusCode")]
	public string SubStatusCode { get; set; } = string.Empty;

	[XmlAttribute(AttributeName = "version")]
	public string Version { get; set; } = string.Empty;
}

[XmlRoot(ElementName = "HttpHostNotificationList", Namespace = "http://www.isapi.org/ver20/XMLSchema")]
public class HttpHostNotificationList
{
	[XmlAttribute(AttributeName = "version")]
	public string Version { get; set; } = string.Empty;

	[XmlElement("HttpHostNotification")]
	public List<HttpHostNotification> Notifications { get; set; }	= [];
}

public class HttpHostNotification
{
	[XmlElement("id")]
	public string Id { get; set; } = string.Empty;

	[XmlElement("url")]
	public string Url { get; set; } = string.Empty;

	[XmlElement("protocolType")]
	public string ProtocolType { get; set; } = string.Empty;

	[XmlElement("parameterFormatType")]
	public string ParameterFormatType { get; set; } = string.Empty;

	[XmlElement("addressingFormatType")]
	public string AddressingFormatType { get; set; } = string.Empty;

	[XmlElement("ipAddress")]
	public string IpAddress { get; set; } = string.Empty;

	[XmlElement("portNo")]
	public int PortNo { get; set; }

	[XmlElement("httpAuthenticationMethod")]
	public string HttpAuthenticationMethod { get; set; } = string.Empty;

	[XmlElement("SubscribeEvent")]
	public SubscribeEvent SubscribeEvent { get; set; } = new();
}

public class SubscribeEvent
{
	[XmlElement("heartbeat")]
	public int Heartbeat { get; set; }

	[XmlElement("eventMode")]
	public string EventMode { get; set; } = string.Empty;

	[XmlArray("EventList")]
	[XmlArrayItem("Event")]
	public List<EventItem> Events { get; set; } = [];
}

public class EventItem
{
	[XmlElement("type")]
	public string Type { get; set; } = string.Empty;

	[XmlElement("minorAlarm")]
	public string MinorAlarm { get; set; } = string.Empty;

	[XmlElement("minorException")]
	public string MinorException { get; set; } = string.Empty;

	[XmlElement("minorOperation")]
	public string MinorOperation { get; set; } = string.Empty;

	[XmlElement("minorEvent")]
	public string MinorEvent { get; set; } = string.Empty;

	[XmlElement("pictureURLType")]
	public string PictureURLType { get; set; } = string.Empty;
}