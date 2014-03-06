package com.xamarin.auth;

import com.xamarin.auth.utils.Uri;

import java.util.HashMap;
import java.util.Map;

/**
 * Created by HomeUser on 25.02.14.
 */
public class Account
{
    private String _username;

    public String getUsername() {
        return _username;
    }

    public void setUsername(String username) {
        this._username = username;
    }

    private HashMap<String, String> _properties;

    public HashMap<String, String> getProperties() {
        return _properties;
    }

    public Account(String username, HashMap<String, String> properties) {
        this._username = username;
        if(properties == null)
            this._properties = new HashMap<String, String>();
        else
            this._properties = properties;
    }

    public Account(String username) {
        this(username, null);
    }

    public Account(HashMap<String, String> properties) {
        this(null, properties);
    }

    public Account()
    {
        this(null, null);
    }

    public String Serialize ()
    {
        StringBuilder sb = new StringBuilder();

        sb.append("__username__=");
        sb.append (Uri.escapeDataString(_username));

        for (Map.Entry<String, String> p : _properties.entrySet())
        {
            sb.append("&");
            sb.append(Uri.escapeDataString(p.getKey()));
            sb.append("=");
            sb.append(Uri.escapeDataString(p.getValue()));
        }

        return sb.toString();
    }

    public static Account Deserialize (String serializedString)
    {
        Account acct = new Account();

        for (String p : serializedString.split ("&")) {
        String[] kv = p.split ("=");

        String key = Uri.unescapeDataString (kv[0]);
        String val = kv.length > 1 ? Uri.unescapeDataString (kv[1]) : "";

        if (key.compareTo("__cookies__") == 0) {
            //TODO: Conflict between System.dll and System.Net.dll (type CookieContainer)
            //acct.Cookies = DeserializeCookies (val);
        } else if (key.compareTo("__username__") == 0) {
            acct._username = val;
        } else {
            acct._properties.put(key, val); //[key] = val;
        }
    }

        return acct;
    }
}
