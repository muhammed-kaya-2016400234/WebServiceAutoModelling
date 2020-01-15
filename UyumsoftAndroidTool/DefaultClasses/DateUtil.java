

import java.sql.Date;
import java.text.SimpleDateFormat;
import java.util.Calendar;

import android.text.format.DateFormat;
import android.util.Log;

public class DateUtil {
	
	public static String FORMAT_DATE_ISO = "yyyy-MM-dd'T'HH:mm:ss"; // "yyyy-MM-dd'T'HH:mm:ss.SSSZ"
	private static SimpleDateFormat dateFormatter = new SimpleDateFormat(FORMAT_DATE_ISO);
	
	public static java.util.Date getDate(String str){
		try{
			if(str != null && str.length() > 0)
				return dateFormatter.parse(str);
		}
		catch (Exception e) {	
			e.printStackTrace();
		}
		return new Date(1900, 1, 1);
	}
	 
	 public static String WebClear(String sData){
		 String xData = "";
		 if( sData != null ){
			 xData = sData.replace("anyType{}", "");
		 }			
		 return xData;
	 }
	 
	 public static int calculateDifference(Date a, Date b){
		 int tempDifference = 0;
	     int difference = 0;
	     Calendar earlier = Calendar.getInstance();
	     Calendar later = Calendar.getInstance();

	     if (a.compareTo(b) < 0){
	    	 earlier.setTime(a);
	         later.setTime(b);
	     }
	     else{
	    	 earlier.setTime(b);
	         later.setTime(a);
	     }

	     while (earlier.get(Calendar.YEAR) != later.get(Calendar.YEAR)){
	    	 tempDifference = 365 * (later.get(Calendar.YEAR) - earlier.get(Calendar.YEAR));
	         difference += tempDifference;
	         earlier.add(Calendar.DAY_OF_YEAR, tempDifference);
	     }

	     if (earlier.get(Calendar.DAY_OF_YEAR) != later.get(Calendar.DAY_OF_YEAR)){
	    	 tempDifference = later.get(Calendar.DAY_OF_YEAR) - earlier.get(Calendar.DAY_OF_YEAR);
	         difference += tempDifference;
	         earlier.add(Calendar.DAY_OF_YEAR, tempDifference);
	     }
	     return difference;
	 }	 	   	    	    	   

	 public static String getDate(){
		 
		 SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss"); 
		 Date date = new Date(System.currentTimeMillis());
		 return dateFormat.format(date);

		 
		 /* String[] formats = new String[] {
   "yyyy-MM-dd",
   "yyyy-MM-dd HH:mm",
   "yyyy-MM-dd HH:mmZ",
   "yyyy-MM-dd HH:mm:ss.SSSZ",
   "yyyy-MM-dd'T'HH:mm:ss.SSSZ",
 };
 for (String format : formats) {
   SimpleDateFormat sdf = new SimpleDateFormat(format, Locale.US);
   System.err.format("%30s %s\n", format, sdf.format(new Date(0)));
   sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
   System.err.format("%30s %s\n", format, sdf.format(new Date(0)));
 }
*/		 	       
	    }	    	   
	    	    
	
}