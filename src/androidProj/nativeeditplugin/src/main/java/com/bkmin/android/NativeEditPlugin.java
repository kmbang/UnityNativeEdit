package com.bkmin.android;
import com.unity3d.player.*;

import android.app.Activity;
import android.graphics.Rect;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewTreeObserver;
import android.widget.RelativeLayout;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.concurrent.atomic.AtomicBoolean;

/// UnityEditBox Plugin
/// Written by bkmin 2015/5 (kmin.bang@gmail.com)

public class NativeEditPlugin {
    public static Activity unityActivity;
    public static RelativeLayout mainLayout;
    private static ViewGroup	topViewGroup;
    private static boolean		pluginInitialized = false;
    private static final Object Lock = new Object() {};
    private static int		keyboardHeight = 0;
    private static String   unityName = "";
    private static String MSG_SHOW_KEYBOARD = "ShowKeyboard";

    public static String LOG_TAG = "NativeEditPlugin";

    private static View getLeafView(View view) {
        if (view instanceof ViewGroup) {
            ViewGroup vg = (ViewGroup)view;
            for (int i = 0; i < vg.getChildCount(); ++i) {
                View chview = vg.getChildAt(i);
                View result = getLeafView(chview);
                if (result != null)
                    return result;
            }
            return null;
        }
        else {
            Log.i(LOG_TAG, "Found leaf view");
            return view;
        }
    }

    private static void SetInitialized()
    {
        synchronized(Lock)
        {
            pluginInitialized = true;
        }
    }

    public static boolean IsPluginInitialized()
    {
        synchronized(Lock)
        {
            return pluginInitialized;
        }
    }

    public static void InitPluginMsgHandler(final String _unityName)
    {
        unityActivity = UnityPlayer.currentActivity;
        unityName = _unityName;

        unityActivity.runOnUiThread(new Runnable() {
            public void run() {
                final ViewGroup rootView = (ViewGroup) unityActivity.findViewById (android.R.id.content);
                View topMostView = getLeafView(rootView);
                topViewGroup = (ViewGroup) topMostView.getParent();
                mainLayout = new RelativeLayout(unityActivity);
                RelativeLayout.LayoutParams rlp = new RelativeLayout.LayoutParams(
                        RelativeLayout.LayoutParams.MATCH_PARENT,
                        RelativeLayout.LayoutParams.MATCH_PARENT);
                topViewGroup.addView(mainLayout, rlp);
                SetInitialized();

                rootView.getViewTreeObserver().addOnGlobalLayoutListener(new ViewTreeObserver.OnGlobalLayoutListener() {
                    @Override
                    public void onGlobalLayout() {

                        Rect r = new Rect();
                        rootView.getWindowVisibleDisplayFrame(r);
                        int screenHeight = rootView.getRootView().getHeight();

                        // r.bottom is the position above soft keypad or device button.
                        // if keypad is shown, the r.bottom is smaller than that before.
                        keyboardHeight = screenHeight - r.bottom;
                        boolean bKeyOpen = (keyboardHeight > screenHeight * 0.15);

                        float fKeyHeight = (float) keyboardHeight / (float) screenHeight;

                        JSONObject json = new JSONObject();
                        try {
                            json.put("msg", MSG_SHOW_KEYBOARD);
                            json.put("show", bKeyOpen);
                            json.put("keyheight", fKeyHeight);
                        } catch (JSONException e) {
                        }
                        SendUnityMessage(json);
                    }
                });
                Log.i(LOG_TAG, "InitEditBoxPlugin okay");
            }
        });
    }

    public static void ClosePluginMsgHandler()
    {
        unityActivity.runOnUiThread(new Runnable() {
            public void run() {
                topViewGroup.removeView(mainLayout);
            }
        });
    }

    public static void SendUnityMessage(JSONObject jsonMsg)
    {
        UnityPlayer.UnitySendMessage(unityName, "OnMsgFromPlugin", jsonMsg.toString());
    }

    static JSONObject jsonStaticRet = null;
    public static String SendUnityMsgToPlugin(final int nSenderId, final String jsonMsg) {
        final Runnable task = new Runnable() {
            public void run() {
                jsonStaticRet = EditBox.processRecvJsonMsg(nSenderId, jsonMsg);
                synchronized (this) {
                    this.notify();
                }
            }
        };
        synchronized (task) {
            unityActivity.runOnUiThread(task);
            try
            {
                task.wait();
            }
            catch ( InterruptedException e )
            {
                e.printStackTrace();
            }
        }

        return jsonStaticRet.toString();
    }
}
