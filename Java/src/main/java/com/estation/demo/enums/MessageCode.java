package com.estation.demo.enums;

public enum MessageCode {
    OK(0),
    IDLE(1),
    RESULT(2),
    HEARTBEAT(3),
    MOD_ERROR(4),
    APP_ERROR(5),
    BUSY(6),
    MAX_LIMIT(7),
    INVALID_TASK_ESL(8),
    INVALID_TASK_DSL(9),
    INVALID_CONFIG(10),
    INVALID_OTA(11),
    INVALID_FIRMWARE(12);

    private final int value;

    MessageCode(int value) {
        this.value = value;
    }

    public int value() {
        return value;
    }

    public static MessageCode fromValue(int value) {
        for (MessageCode code : values()) {
            if (code.value == value) {
                return code;
            }
        }
        throw new IllegalArgumentException("Unknown MessageCode value: " + value);
    }
}
