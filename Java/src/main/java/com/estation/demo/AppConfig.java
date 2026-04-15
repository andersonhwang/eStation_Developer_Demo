package com.estation.demo;

public final class AppConfig {
    public static final String BROKER = "127.0.0.1";
    public static final int PORT = 9071;
    public static final String AP_ID = "00C8";
    public static final String USER_NAME = "test";
    public static final String PASSWORD = "123456";

    public static final String TOPIC_CONFIG = "/estation/" + AP_ID + "/configure";
    public static final String TOPIC_TASK_ESL = "/estation/" + AP_ID + "/taskESL";
    public static final String TOPIC_TASK_ESL2 = "/estation/" + AP_ID + "/taskESL2";
    public static final String TOPIC_TASK_DSL = "/estation/" + AP_ID + "/taskDSL";
    public static final String TOPIC_TASK_DSL2 = "/estation/" + AP_ID + "/taskDSL2";
    public static final String TOPIC_FIRMWARE = "/estation/" + AP_ID + "/firmware";
    public static final String TOPIC_OTA = "/estation/" + AP_ID + "/ota";

    public static final String TOPIC_INFOR = "/estation/" + AP_ID + "/infor";
    public static final String TOPIC_RESULT = "/estation/" + AP_ID + "/result";
    public static final String TOPIC_HEARTBEAT = "/estation/" + AP_ID + "/heartbeat";
    public static final String TOPIC_MESSAGE = "/estation/" + AP_ID + "/message";

    public static final String TAG_ID_LIST = "TagID.txt";

    private AppConfig() {
    }
}
