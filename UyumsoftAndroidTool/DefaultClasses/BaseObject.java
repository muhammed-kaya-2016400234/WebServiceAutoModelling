

import java.io.Serializable;
import java.util.Date;

import org.ksoap2.serialization.*;
import android.util.Base64;

public abstract class BaseObject implements KvmSerializable, Serializable {

	protected static final String NAMESPACE = "http://tempuri.org/";

	public BaseObject()
	{
		super();
	}

	public void loadSoapObject(Object obj){
		if(obj instanceof SoapObject) {
			SoapObject property=(SoapObject)obj;
			if (property == null) return;
			int pr = getPropertyCount();
			PropertyInfo pro = new PropertyInfo();
			for (int i = 0; i < pr; i++) {
				getPropertyInfo(i, null, pro);
				if (property.hasProperty(pro.name))
					setProperty(i, property.getProperty(pro.name));
			}
		}else if(obj instanceof String){
			setProperty(0,obj.toString());
		}else if(obj instanceof Integer){
			setProperty(0,Integer.parseInt(obj.toString()));
		}else if(obj instanceof Double){
			setProperty(0,Double.parseDouble(obj.toString()));
		}else if(obj instanceof Float){
			setProperty(0,Float.parseFloat(obj.toString()));
		}else if(obj instanceof Date){
			setProperty(0,DateUtil.getDate(obj.toString()));
		}
	}

}
