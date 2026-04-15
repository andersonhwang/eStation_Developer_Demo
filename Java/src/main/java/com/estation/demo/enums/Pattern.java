package com.estation.demo.enums;

public enum Pattern {
    UPDATE_DISPLAY(0),
    UPDATE(1),
    DISPLAY(2),
    LED(3),
    KEY(4),
    CLEAR_GIF(5),
    CLEAR_GIF_UPDATE_IMAGE(6);

    private final int value;

    Pattern(int value) {
        this.value = value;
    }

    public int value() {
        return value;
    }

    public static Pattern fromValue(int value) {
        for (Pattern pattern : values()) {
            if (pattern.value == value) {
                return pattern;
            }
        }
        throw new IllegalArgumentException("Unknown Pattern value: " + value);
    }
}
