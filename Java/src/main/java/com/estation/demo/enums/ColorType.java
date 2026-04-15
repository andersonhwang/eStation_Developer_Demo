package com.estation.demo.enums;

public enum ColorType {
    BLACK(1),
    RED(2),
    YELLOW(4),
    BLACK_RED(3),
    BLACK_YELLOW(5),
    BLACK_RED_YELLOW(7);

    private final int value;

    ColorType(int value) {
        this.value = value;
    }

    public int value() {
        return value;
    }
}
