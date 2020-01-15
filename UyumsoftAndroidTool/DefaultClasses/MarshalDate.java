
import org.kobjects.isodate.IsoDate;
import org.ksoap2.serialization.Marshal;
import org.ksoap2.serialization.PropertyInfo;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlSerializer;

import java.io.IOException;
import java.util.Date;

public class MarshalDate implements Marshal {
	
	public static Class DATE_CLASS = new Date(0).getClass();

	@Override
	public Object readInstance(XmlPullParser parser, String namespace, String name, PropertyInfo expected) throws IOException, XmlPullParserException{
			//IsoDate.DATE_TIME=3
			String Test1 = parser.nextText();
			return IsoDate.stringToDate(parser.nextText(), IsoDate.DATE_TIME);
	}

	@Override
	public void register(SoapSerializationEnvelope cm) {
		cm.addMapping(cm.xsd, "dateTime", MarshalDate.DATE_CLASS, this);
		// "DateTime" is wrong use "dateTime" ok
		
	}

	@Override
	public void writeInstance(XmlSerializer writer, Object obj) throws IOException{
			String Test="";
			Test = IsoDate.dateToString((Date) obj, IsoDate.DATE_TIME);
			writer.text(IsoDate.dateToString((Date) obj, IsoDate.DATE_TIME));
	}

}