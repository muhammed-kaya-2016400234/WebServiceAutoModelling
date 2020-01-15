

import java.io.Serializable;
import org.ksoap2.serialization.*;
import android.util.Base64;

public abstract class BaseObject implements KvmSerializable, Serializable {

    protected static final String NAMESPACE = "http://tempuri.org/";

    public BaseObject()
    {
        super();
    }

	public void loadSoapObject(SoapObject property){
		if(property == null) return;
		int pr = getPropertyCount();
		PropertyInfo pro = new PropertyInfo();
		for(int i=0;i<pr;i++){
			getPropertyInfo(i, null, pro);
			if(property.hasProperty(pro.name))
				setProperty(i, property.getProperty(pro.name));
		}
	} 

}