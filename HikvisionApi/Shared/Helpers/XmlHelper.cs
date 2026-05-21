using System.Xml.Serialization;

namespace Shared.Helpers;

public static class XmlHelper<T>
{
	public static T DeserializeResponseStatus(string xml)
	{
		try
		{
			var serializer = new XmlSerializer(typeof(T));

			using var reader = new StringReader(xml);

			return (T)serializer.Deserialize(reader)!;
		}
		catch (Exception ex)
		{
			throw new Exception($"Error deserializing xml: {ex.Message}", ex);
		}
	}
}
