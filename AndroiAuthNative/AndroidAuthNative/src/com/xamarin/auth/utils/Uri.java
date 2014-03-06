package com.xamarin.auth.utils;


import android.util.Log;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.io.ByteArrayOutputStream;

/**
 * Created by HomeUser on 25.02.14.
 */
public class Uri {

    private final static int MaxUriLength = 32766;

    static final char[] hexUpperChars = new char [] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

    public static final String Utf8Enc = "UTF_8";

    public static String escapeDataString(String stringToEscape) throws IllegalArgumentException {

        if(stringToEscape == null)
            Log.e("Error", "escapeDataString is null");

        if (stringToEscape.length() > MaxUriLength) {
            throw new IllegalArgumentException (String.format("Uri is longer than the maximum %s characters.", MaxUriLength));
        }

        boolean escape = false;
        for (int i = 0; i < stringToEscape.length(); i++ /*Object __dummyForeachVar2 : stringToEscape*/)
        {
            char c = stringToEscape.charAt(i);//(Character)__dummyForeachVar2;
            if (needToEscapeDataChar(c))
            {
                escape = true;
                break;
            }

        }
        if (!escape)
        {
            return stringToEscape;
        }

        StringBuilder sb = new StringBuilder();
        byte[] bytes = new byte[0];//Encoding.UTF8.GetBytes(stringToEscape);
        try {
            bytes = stringToEscape.getBytes(Utf8Enc);
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
        for (/*Object*/byte __dummyForeachVar3 : bytes)
        {
            //byte b = (Byte)__dummyForeachVar3;
            if (needToEscapeDataChar(/*(char)b*/(char)__dummyForeachVar3))
                sb.append(hexEscape(/*(char)b*/(char)__dummyForeachVar3));
            else
                sb.append(/*(char)b*/__dummyForeachVar3);
        }
        return sb.toString();
    }

    static boolean needToEscapeDataChar(char b) {
        return !((b >= 'A' && b <= 'Z') || (b >= 'a' && b <= 'z') || (b >= '0' && b <= '9') || b == '_' || b == '~' || b == '!' || b == '\'' || b == '(' || b == ')' || b == '*' || b == '-' || b == '.');
    }

    public static String hexEscape(char character) throws IllegalArgumentException /*Exception*/ {
        if (character > 255)
        {
            throw new IllegalArgumentException("character");
        }

        return "%" + hexUpperChars[((character & 0xf0) >> 4)] + hexUpperChars[((character & 0x0f))];
    }

    public static String unescapeDataString (String stringToUnescape)
    {
        return unescapeDataString (stringToUnescape, false);
    }

    protected static String unescapeDataString (String stringToUnescape, boolean safe)
    {
//        if (stringToUnescape == null)
//            throw new ArgumentNullException ("stringToUnescape");

        if (stringToUnescape.indexOf('%') == -1 && stringToUnescape.indexOf('+') == -1)
            return stringToUnescape;

        StringBuilder output = new StringBuilder ();
        long len = stringToUnescape.length();
        //MemoryStream bytes = new MemoryStream ();
        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        int xchar;

        try
        {

            for (int i = 0; i < len; i++) {
                if (stringToUnescape.charAt(i) == '%' && i + 2 < len && stringToUnescape.charAt(i + 1) != '%') {
                    if (stringToUnescape.charAt(i + 1) == 'u' && i + 5 < len) {
                        if (bytes.size() /*Length*/ > 0) {
                            output.append(new String(bytes.toByteArray(), Utf8Enc)/*GetChars(bytes, Encoding."UTF8")*/);
                            //bytes.SetLength (0);
                        }

                        xchar = getChar(stringToUnescape, i + 2, 4, safe);
                        if (xchar != -1) {
                            output.append((char) xchar);
                            i += 5;
                        }
                        else {
                            output.append('%');
                        }
                    }
                    else if ((xchar = getChar(stringToUnescape, i + 1, 2, safe)) != -1) {
                        bytes.write(xchar);
                        i += 2;
                    }
                    else {
                        output.append('%');
                    }
                    continue;
                }

                if (bytes.size()/*Length*/ > 0) {
                    output.append(new String(bytes.toByteArray(), Utf8Enc)/*GetChars(bytes, Encoding."UTF8")*/);
                    //bytes.SetLength (0);
                }

                output.append(stringToUnescape.charAt(i));
            }


            if (bytes.size()/*Length*/ > 0) {
                output.append(/*GetChars(bytes, Encoding.UTF8)*/new String(bytes.toByteArray(), Utf8Enc));
            }

            bytes.close();

        }catch (UnsupportedEncodingException uee)
        {
            uee.printStackTrace();
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }

        bytes = null;
        return output.toString();
    }

    private static int getChar(String str, int offset, int length, boolean safe)
    {
        int val = 0;
        int end = length + offset;
        for (int i = offset; i < end; i++) {
            char c = str.charAt(i);
            if (c > 127)
                return -1;

            int current = getInt((byte) c);
            if (current == -1)
                return -1;
            val = (val << 4) + current;
        }

        if (!safe)
            return val;

        switch ((char) val) {
            case '%':
            case '#':
            case '?':
            case '/':
            case '\\':
            case '@':
            case '&': // not documented
                return -1;
            default:
                return val;
        }
    }

    private static int getInt(byte b)
    {
        char c = (char) b;
        if (c >= '0' && c <= '9')
            return c - '0';

        if (c >= 'a' && c <= 'f')
            return c - 'a' + 10;

        if (c >= 'A' && c <= 'F')
            return c - 'A' + 10;

        return -1;
    }
}
